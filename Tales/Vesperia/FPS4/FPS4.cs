using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.FPS4 {
	public struct ContentInfo {
		public ushort Value { get; private set; }
		public ContentInfo( ushort contentBitmask ) {
			Value = contentBitmask;
		}

		// -- bitmask examples --
		// 0x000F -> loc, end, size, name
		// 0x0007 -> loc, end, size
		// 0x0047 -> loc, end, size, ptr to path? attributes? something like that
		public bool ContainsStartPointers { get { return ( Value & 0x0001 ) == 0x0001; } }
		public bool ContainsSectorSizes { get { return ( Value & 0x0002 ) == 0x0002; } }
		public bool ContainsFileSizes { get { return ( Value & 0x0004 ) == 0x0004; } }
		public bool ContainsFilenames { get { return ( Value & 0x0008 ) == 0x0008; } }
		public bool ContainsFiletypes { get { return ( Value & 0x0020 ) == 0x0020; } }
		public bool ContainsFileMetadata { get { return ( Value & 0x0040 ) == 0x0040; } }
		public bool Contains0x0080 { get { return ( Value & 0x0080 ) == 0x0080; } }

		public bool HasUnknownDataTypes { get { return ( Value & 0xFF10 ) != 0; } }
	}

	public class FileInfo {
		public uint FileIndex;
		public uint? Location = null;
		public uint? SectorSize = null;
		public uint? FileSize = null;
		public string FileName = null;
		public string FileType = null;
		public List<(string Key, string Value)> Metadata = null;
		public uint? Unknown0x0080 = null;

		public bool ShouldSkip => ( Location != null && Location == 0xFFFFFFFF ) || ( Unknown0x0080 != null && Unknown0x0080 > 0 );

		public FileInfo( Stream stream, uint fileIndex, ContentInfo bitmask, Util.Endianness endian, Util.GameTextEncoding encoding ) {
			FileIndex = fileIndex;
			if ( bitmask.ContainsStartPointers ) {
				Location = stream.ReadUInt32().FromEndian( endian );
			}
			if ( bitmask.ContainsSectorSizes ) {
				SectorSize = stream.ReadUInt32().FromEndian( endian );
			}
			if ( bitmask.ContainsFileSizes ) {
				FileSize = stream.ReadUInt32().FromEndian( endian );
			}
			if ( bitmask.ContainsFilenames ) {
				FileName = stream.ReadSizedString( 0x20, encoding ).TrimNull();
			}
			if ( bitmask.ContainsFiletypes ) {
				FileType = stream.ReadSizedString( 0x04, encoding ).TrimNull();
			}
			if ( bitmask.ContainsFileMetadata ) {
				uint pathLocation = stream.ReadUInt32().FromEndian( endian );
				if ( pathLocation != 0 ) {
					string md = stream.ReadNulltermStringFromLocationAndReset( pathLocation, encoding );
					Metadata = new List<(string Key, string Value)>();
					foreach ( string m in md.Split( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries ) ) {
						if ( m.Contains( "=" ) ) {
							var s = m.Split( new char[] { '=' }, 2 );
							Metadata.Add( (s[0], s[1]) );
						} else {
							Metadata.Add( (null, m) );
						}
					}
				}
			}
			if ( bitmask.Contains0x0080 ) {
				Unknown0x0080 = stream.ReadUInt32().FromEndian( endian );
			}
		}

		public uint? GuessFileSize( List<FileInfo> files ) {
			uint? r = FileSize ?? SectorSize;
			if ( r != null ) {
				return r;
			}

			if ( Location != null && files != null ) {
				for ( int i = (int)( FileIndex + 1 ); i < files.Count; ++i ) {
					if ( !files[i].ShouldSkip ) {
						return files[i].Location - Location;
					}
				}
			}

			return null;
		}

		public (string Path, string Name) GuessFilePathName() {
			string path = Metadata?.FirstOrDefault( x => x.Key == null ).Value;
			if ( string.IsNullOrWhiteSpace( path ) ) {
				path = null;
			}

			if ( !string.IsNullOrWhiteSpace( FileName ) ) {
				return (path, FileName);
			}

			string nameFromMetadata = Metadata?.FirstOrDefault( x => x.Key == "name" ).Value;
			if ( !string.IsNullOrWhiteSpace( nameFromMetadata ) ) {
				return (path, nameFromMetadata);
			}

			string indexString = FileIndex.ToString( "D4" );
			string indexStringWithType = string.IsNullOrWhiteSpace( FileType ) ? indexString : indexString + "." + FileType;
			if ( path == null ) {
				return (null, indexStringWithType);
			}

			int finaldir = path.LastIndexOf( '/' );
			if ( finaldir == -1 ) {
				return (null, path + "." + indexStringWithType);
			} else {
				return (path.Substring( 0, finaldir ), path.Substring( finaldir + 1 ) + "." + indexStringWithType);
			}
		}
	}

	public class FPS4 : IDisposable {
		public FPS4() {
			ContentBitmask = new ContentInfo( 0x000F );
			Alignment = 0x800;
		}
		public FPS4( string inFilename ) {
			if ( !LoadFile( inFilename ) ) {
				throw new Exception( "Failed loading FPS4: " + inFilename );
			}
		}
		public FPS4( string headerFilename, string contentFilename ) {
			if ( !LoadFile( headerFilename, contentFilename ) ) {
				throw new Exception( "Failed loading FPS4: " + headerFilename + " + " + contentFilename );
			}
		}
		~FPS4() {
			Close();
		}

		FileStream contentFile = null;
		uint FileCount;
		uint HeaderSize;
		public uint FirstFileStart;
		ushort EntrySize;
		public ContentInfo ContentBitmask;
		public uint Unknown2;
		uint ArchiveNameLocation;

		public string ArchiveName = null;
		public uint Alignment;
		public uint FileLocationMultiplier;
		public bool ShouldGuessFilesizeFromNextFile;
		public Util.Endianness Endian = Util.Endianness.BigEndian;

		public List<FileInfo> Files;

		private bool LoadFile( string headerFilename, string contentFilename = null ) {
			FileStream infile = null;
			try {
				infile = new FileStream( headerFilename, FileMode.Open );
				if ( contentFilename != null ) {
					contentFile = new FileStream( contentFilename, FileMode.Open );
				} else {
					contentFile = infile;
				}
			} catch ( Exception ) {
				Console.WriteLine( "ERROR: can't open " + headerFilename );
				return false;
			}

			infile.Seek( 0x00, SeekOrigin.Begin );
			string magic = infile.ReadAscii( 4 );
			if ( magic != "FPS4" ) {
				Console.WriteLine( "Not an FPS4 file!" );
				return false;
			}

			Endian = Util.Endianness.BigEndian;
			FileCount = infile.ReadUInt32().FromEndian( Endian );
			HeaderSize = infile.ReadUInt32().FromEndian( Endian );

			// if header seems huge then we probably have assumed the wrong endianness
			if ( HeaderSize > 0xFFFF ) {
				Endian = Util.Endianness.LittleEndian;
				FileCount = FileCount.ToEndian( Util.Endianness.BigEndian ).FromEndian( Endian );
				HeaderSize = HeaderSize.ToEndian( Util.Endianness.BigEndian ).FromEndian( Endian );
			}

			FirstFileStart = infile.ReadUInt32().FromEndian( Endian );
			EntrySize = infile.ReadUInt16().FromEndian( Endian );
			ContentBitmask = new ContentInfo( infile.ReadUInt16().FromEndian( Endian ) );
			Unknown2 = infile.ReadUInt32().FromEndian( Endian );
			ArchiveNameLocation = infile.ReadUInt32().FromEndian( Endian );
			infile.Position = ArchiveNameLocation;
			if ( ArchiveNameLocation > 0 ) {
				ArchiveName = infile.ReadShiftJisNullterm();
			}

			Alignment = FirstFileStart;

			Console.WriteLine( "Content Bitmask: 0x" + ContentBitmask.Value.ToString( "X4" ) );
			if ( ContentBitmask.HasUnknownDataTypes ) {
				Console.WriteLine( "WARNING: Bitmask identifies unknown data types, data interpretation will probably be incorrect." );
			}

			Files = new List<FileInfo>( (int)FileCount );
			for ( uint i = 0; i < FileCount; ++i ) {
				infile.Position = HeaderSize + ( i * EntrySize );
				Files.Add( new FileInfo( infile, i, ContentBitmask, Endian, Util.GameTextEncoding.ASCII ) );
			}

			FileLocationMultiplier = CalculateFileLocationMultiplier();
			ShouldGuessFilesizeFromNextFile = !ContentBitmask.ContainsFileSizes && !ContentBitmask.ContainsSectorSizes && CalculateIsLinear();

			if ( infile != contentFile ) {
				infile.Close();
			}
			return true;
		}

		public bool CalculateIsLinear() {
			if ( ContentBitmask.ContainsStartPointers ) {
				uint lastFilePosition = Files[0].Location.Value;
				for ( int i = 1; i < FileCount; ++i ) {
					FileInfo fi = Files[i];
					if ( fi.ShouldSkip ) {
						continue;
					}
					if ( fi.Location.Value <= lastFilePosition ) {
						return false;
					}
					lastFilePosition = fi.Location.Value;
				}
				return true;
			}
			return false;
		}

		public uint CalculateFileLocationMultiplier() {
			if ( ContentBitmask.ContainsStartPointers ) {
				uint smallestFileLoc = uint.MaxValue;
				for ( int i = 0; i < Files.Count - 1; ++i ) {
					FileInfo fi = Files[i];
					if ( !fi.ShouldSkip && fi.Location.Value > 0 ) {
						smallestFileLoc = Math.Min( fi.Location.Value, smallestFileLoc );
					}
				}
				if ( smallestFileLoc == uint.MaxValue || smallestFileLoc == FirstFileStart ) {
					return 1;
				}
				if ( FirstFileStart % smallestFileLoc == 0 ) {
					return FirstFileStart / smallestFileLoc;
				}
			}
			return 1;
		}

		public void Extract( string dirname, bool noMetadataParsing = false ) {
			System.IO.Directory.CreateDirectory( dirname );

			for ( int i = 0; i < Files.Count - 1; ++i ) {
				FileInfo fi = Files[i];

				if ( fi.ShouldSkip ) {
					Console.WriteLine( "Skipped #" + i.ToString( "D4" ) );
					continue;
				}

				if ( fi.Location == null ) {
					throw new Exception( "FPS4 extraction failure: Doesn't contain file start pointers!" );
				}
				uint? maybeFilesize = fi.GuessFileSize( ShouldGuessFilesizeFromNextFile ? Files : null );
				if ( maybeFilesize == null ) {
					throw new Exception( "FPS4 extraction failure: Doesn't contain filesize information!" );
				}

				uint fileloc = fi.Location.Value * FileLocationMultiplier;
				uint filesize = maybeFilesize.Value;
				(string path, string filename) = fi.GuessFilePathName();

				if ( path != null ) {
					Directory.CreateDirectory( dirname + '/' + path );
					path = path + '/' + filename;
				} else {
					path = filename;
				}
				FileStream outfile = new FileStream( Path.Combine( dirname, path ), FileMode.Create );

				Console.WriteLine( "Extracting #" + i.ToString( "D4" ) + ": " + path );

				contentFile.Seek( fileloc, SeekOrigin.Begin );
				Util.CopyStream( contentFile, outfile, (int)filesize );
				outfile.Close();
			}
		}

		public static void Pack(
			string[] files,
			string outFilename,
			ContentInfo ContentBitmask,
			Util.Endianness Endian,
			uint Unknown2,
			Stream infile,
			string ArchiveName,
			uint FirstFileStart,
			uint Alignment,
			string headerName = null,
			string metadata = null
		) {
			uint FileCount = (uint)files.Length + 1;
			uint HeaderSize = 0x1C;

			ushort EntrySize = 0;

			if ( ContentBitmask.ContainsStartPointers ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsSectorSizes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFileSizes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFilenames ) { EntrySize += 0x20; }
			if ( ContentBitmask.ContainsFiletypes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFileMetadata ) { EntrySize += 4; }

			bool headerToSeparateFile = false;
			if ( headerName != null ) { headerToSeparateFile = true; }

			using ( FileStream f = new FileStream( headerToSeparateFile ? headerName : outFilename, FileMode.Create ) ) {
				// header
				f.Write( Encoding.ASCII.GetBytes( "FPS4" ), 0, 4 );
				f.WriteUInt32( FileCount.ToEndian( Endian ) );
				f.WriteUInt32( HeaderSize.ToEndian( Endian ) );
				f.WriteUInt32( 0 ); // start of first file goes here, will be filled in later
				f.WriteUInt16( EntrySize.ToEndian( Endian ) );
				f.WriteUInt16( ContentBitmask.Value.ToEndian( Endian ) );
				f.WriteUInt32( Unknown2.ToEndian( Endian ) );
				f.WriteUInt32( 0 ); // ArchiveNameLocation, will be filled in later

				// file list
				for ( int i = 0; i < files.Length; ++i ) {
					var fi = new System.IO.FileInfo( files[i] );
					if ( ContentBitmask.ContainsStartPointers ) { f.WriteUInt32( 0 ); } // properly written later
					if ( ContentBitmask.ContainsSectorSizes ) { f.WriteUInt32( 0 ); } // properly written later
					if ( ContentBitmask.ContainsFileSizes ) { f.WriteUInt32( ( (uint)( fi.Length ) ).ToEndian( Endian ) ); }
					if ( ContentBitmask.ContainsFilenames ) {
						string filename = fi.Name.Truncate( 0x1F );
						byte[] fnbytes = Util.ShiftJISEncoding.GetBytes( filename );
						f.Write( fnbytes, 0, fnbytes.Length );
						for ( int j = fnbytes.Length; j < 0x20; ++j ) {
							f.WriteByte( 0 );
						}
					}
					if ( ContentBitmask.ContainsFiletypes ) {
						string extension = fi.Extension.TrimStart( '.' ).Truncate( 4 );
						byte[] extbytes = Util.ShiftJISEncoding.GetBytes( extension );
						f.Write( extbytes, 0, extbytes.Length );
						for ( int j = extbytes.Length; j < 4; ++j ) {
							f.WriteByte( 0 );
						}
					}
					if ( ContentBitmask.ContainsFileMetadata ) {
						if ( infile != null ) {
							// copy this from original file, very hacky when filenames/filecount/metadata changes
							infile.Position = f.Position;
							f.WriteUInt32( infile.ReadUInt32() );
						} else {
							// place a null for now and go back to fix later
							f.WriteUInt32( 0 );
						}
					}
				}

				// at the end of the file list, reserve space for a final entry pointing to the end of the container
				for ( int j = 0; j < EntrySize; ++j ) {
					f.WriteByte( 0 );
				}

				// the idea is to write a pointer here
				// and at the target of the pointer you have a nullterminated string
				// with all the metadata in a param=data format separated by spaces
				// maybe including a filepath at the start without a param=
				// strings should be after the filelist block but before the actual files
				if ( ContentBitmask.ContainsFileMetadata && metadata != null ) {
					for ( int i = 0; i < files.Length; ++i ) {
						var fi = new System.IO.FileInfo( files[i] );
						long ptrPos = 0x1C + ( ( i + 1 ) * EntrySize ) - 4;

						// remember pos
						uint oldPos = (uint)f.Position;
						// jump to metaptr
						f.Position = ptrPos;
						// write remembered pos
						f.WriteUInt32( oldPos.ToEndian( Endian ) );
						// jump to remembered pos
						f.Position = oldPos;
						// write meta + nullterm
						if ( metadata.Contains( 'p' ) ) {
							string relativePath = GetRelativePath( f.Name, fi.FullName );
							f.Write( Util.ShiftJISEncoding.GetBytes( relativePath ) );
						}
						if ( metadata.Contains( 'n' ) ) {
							f.Write( Util.ShiftJISEncoding.GetBytes( "name=" + Path.GetFileNameWithoutExtension( fi.FullName ) ) );
						}
						f.WriteByte( 0 );
					}
				}

				// write original archive filepath
				if ( ArchiveName != null ) {
					// put the location into the header
					uint ArchiveNameLocation = Convert.ToUInt32( f.Position );
					f.Position = 0x18;
					f.WriteUInt32( ArchiveNameLocation.ToEndian( Endian ) );
					f.Position = ArchiveNameLocation;

					// and actually write it
					byte[] archiveNameBytes = Util.ShiftJISEncoding.GetBytes( ArchiveName );
					f.Write( archiveNameBytes, 0, archiveNameBytes.Length );
					f.WriteByte( 0 );
				}

				// fix up file pointers
				{
					long pos = f.Position;

					// location of first file
					if ( headerToSeparateFile ) {
						FirstFileStart = 0;
					} else {
						if ( infile != null ) {
							// keep FirstFileStart from loaded file, this is a hack
							// will break if file metadata (names, count, etc.) changes!
						} else {
							FirstFileStart = ( (uint)( f.Position ) ).Align( Alignment );
						}
					}
					f.Position = 0xC;
					f.WriteUInt32( FirstFileStart.ToEndian( Endian ) );

					// file entries
					uint ptr = FirstFileStart;
					for ( int i = 0; i < files.Length; ++i ) {
						f.Position = 0x1C + ( i * EntrySize );
						var fi = new System.IO.FileInfo( files[i] );
						if ( ContentBitmask.ContainsStartPointers ) { f.WriteUInt32( ptr.ToEndian( Endian ) ); }
						if ( ContentBitmask.ContainsSectorSizes ) { f.WriteUInt32( ( (uint)( fi.Length.Align( (int)Alignment ) ) ).ToEndian( Endian ) ); }
						ptr += (uint)fi.Length.Align( (int)Alignment );
					}
					f.Position = 0x1C + ( files.Length * EntrySize );
					f.WriteUInt32( ptr.ToEndian( Endian ) );

					f.Position = pos;
				}

				if ( !headerToSeparateFile ) {
					// pad until files
					if ( infile != null ) {
						infile.Position = f.Position;
						for ( long i = f.Length; i < FirstFileStart; ++i ) {
							f.WriteByte( (byte)infile.ReadByte() );
						}
					} else {
						for ( long i = f.Length; i < FirstFileStart; ++i ) {
							f.WriteByte( 0 );
						}
					}
				}

				// actually write files
				if ( headerToSeparateFile ) {
					using ( FileStream dataStream = new FileStream( outFilename, FileMode.Create ) ) {
						WriteFilesToFileStream( files, dataStream, Alignment );
						dataStream.Close();
					}
				} else {
					WriteFilesToFileStream( files, f, Alignment );
				}
			}
		}

		private static void WriteFilesToFileStream( string[] files, FileStream f, uint Alignment ) {
			for ( int i = 0; i < files.Length; ++i ) {
				using ( var fs = new System.IO.FileStream( files[i], FileMode.Open, FileAccess.Read ) ) {
					Console.WriteLine( "Packing #" + i.ToString( "D4" ) + ": " + files[i] );
					Util.CopyStream( fs, f, (int)fs.Length );
					while ( f.Length % Alignment != 0 ) { f.WriteByte( 0 ); }
				}
			}
		}

		public void Close() {
			if ( contentFile != null ) {
				contentFile.Close();
				contentFile = null;
			}
		}

		public void Dispose() {
			Close();
		}

		// FIXME: this works only with pretty specific input, good enough for what I need but certainly not usable for generic cases
		private static string GetRelativePath( string fromDirectory, string toFile ) {
			fromDirectory = System.IO.Path.GetDirectoryName( System.IO.Path.GetFullPath( fromDirectory ) );
			string relative = toFile.Substring( fromDirectory.Length );
			relative = String.Join( "/", relative.Split( new char[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries ).Skip( 1 ).ToArray() );
			if ( relative.Contains( '.' ) ) {
				relative = relative.Substring( 0, relative.LastIndexOf( '.' ) );
			}

			return relative;
		}
	}
}
