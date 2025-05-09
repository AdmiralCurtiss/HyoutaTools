﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools.FileContainer;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaUtils;
using HyoutaUtils.Streams;
using HyoutaTools.FinalFantasyCrystalChronicles.FileSections;
using System.Text.Json;
using System.Text.Json.Nodes;

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
		public bool Contains0x0080 { get { return (Value & 0x0080) == 0x0080; } }
		public bool Contains0x0100 { get { return (Value & 0x0100) == 0x0100; } }

		public bool HasUnknownDataTypes { get { return ( Value & 0xFE10 ) != 0; } }
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
		public uint? Unknown0x0100 = null;

		public bool ShouldSkip => ( Location != null && Location == 0xFFFFFFFF ) || ( Unknown0x0080 != null && Unknown0x0080 > 0 );

		public FileInfo( Stream stream, uint fileIndex, ContentInfo bitmask, EndianUtils.Endianness endian, TextUtils.GameTextEncoding encoding ) {
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
			if (bitmask.Contains0x0100) {
				Unknown0x0100 = stream.ReadUInt32(endian);
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

		public (string Path, string Name) GuessFilePathName(bool noMetadataParsing = false) {
			return GuessFilePathName(FileIndex, FileName, FileType, noMetadataParsing ? null : Metadata);
		}
		public string GuessFullFilePath() {
			(string path, string filename) = GuessFilePathName();
			if (path != null) {
				return path + '/' + filename;
			} else {
				return filename;
			}
		}

		public static (string Path, string Name) GuessFilePathName(
			uint FileIndex, string FileName, string FileType, List<(string Key, string Value)> Metadata
		) {
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

		public override string ToString() {
			return FileName + " at 0x" + Location?.ToString( "X8" ) + ", " + FileSize + " bytes";
		}
	}

	public class PackFileInfo {
		public uint FileIndex;
		public long FileSize;
		public string FileName; // just the filename, no path
		public string FileType;
		public List<(string Key, string Value)> Metadata;
		public uint? Unknown0x0080 = null;
		public uint? Unknown0x0100 = null;

		public string SourcePath;
		public DuplicatableStream DataStream;

		// index of file in List<PackFileInfo> that should be used instead of this one
		// this points both files at the same data, saving space in the container
		public ulong? DuplicateOf = null;

		public PackFileInfo() { }

		public PackFileInfo(PackFileInfo other) {
			FileIndex = other.FileIndex;
			FileSize = other.FileSize;
			FileName = other.FileName;
			FileType = other.FileType;
			Metadata = other.Metadata != null ? new List<(string Key, string Value)>(other.Metadata) : null;
			Unknown0x0080 = other.Unknown0x0080;
			Unknown0x0100 = other.Unknown0x0100;

			SourcePath = other.SourcePath;
			DataStream = other.DataStream != null ? other.DataStream.Duplicate() : null;
			DuplicateOf = other.DuplicateOf;
		}

		public (string Path, string Name) GuessFilePathName() {
			return FileInfo.GuessFilePathName(FileIndex, FileName, FileType, Metadata);
		}
		public string GuessFullFilePath() {
			(string path, string filename) = GuessFilePathName();
			if (path != null) {
				return path + '/' + filename;
			} else {
				return filename;
			}
		}
	}

	public class FPS4 : ContainerBase {
		public FPS4() {
			ContentBitmask = new ContentInfo( 0x000F );
			Alignment = 0x800;
		}
		public FPS4(string headerFilename, string contentFilename = null, bool printProgressToConsole = false) {
			DuplicatableFileStream headerStream = new DuplicatableFileStream(headerFilename);
			if (!LoadFile(headerStream, contentFilename != null ? new DuplicatableFileStream(contentFilename) : headerStream, printProgressToConsole)) {
				throw new Exception("Failed loading FPS4.");
			}
		}
		public FPS4(DuplicatableStream headerStream, DuplicatableStream contentStream = null, bool printProgressToConsole = false) {
			if (!LoadFile(headerStream, contentStream != null ? contentStream : headerStream, printProgressToConsole)) {
				throw new Exception("Failed loading FPS4.");
			}
		}
		~FPS4() {
			Close();
		}

		DuplicatableStream contentFile = null;
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
		public EndianUtils.Endianness Endian = EndianUtils.Endianness.BigEndian;

		public List<FileInfo> Files;

		private bool LoadFile(DuplicatableStream headerStream, DuplicatableStream contentStream, bool printProgressToConsole) {
			bool separateContentFile = headerStream != contentStream;
			DuplicatableStream infile = headerStream.Duplicate();
			contentFile = contentStream.Duplicate();

			infile.Seek( 0x00, SeekOrigin.Begin );
			string magic = infile.ReadAscii( 4 );
			if ( magic != "FPS4" ) {
				if (printProgressToConsole) {
					Console.WriteLine("Not an FPS4 file!");
				}
				return false;
			}

			Endian = EndianUtils.Endianness.BigEndian;
			FileCount = infile.ReadUInt32().FromEndian( Endian );
			HeaderSize = infile.ReadUInt32().FromEndian( Endian );

			// if header seems huge then we probably have assumed the wrong endianness
			if ( HeaderSize > 0xFFFF ) {
				Endian = EndianUtils.Endianness.LittleEndian;
				FileCount = FileCount.ToEndian( EndianUtils.Endianness.BigEndian ).FromEndian( Endian );
				HeaderSize = HeaderSize.ToEndian( EndianUtils.Endianness.BigEndian ).FromEndian( Endian );
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

			if (ContentBitmask.HasUnknownDataTypes && printProgressToConsole) {
				Console.WriteLine("WARNING: Content Bitmask (" + "0x" + ContentBitmask.Value.ToString("X4") + ") identifies unknown data types, data interpretation will probably be incorrect.");
			}

			Files = new List<FileInfo>( (int)FileCount );
			for ( uint i = 0; i < FileCount; ++i ) {
				infile.Position = HeaderSize + ( i * EntrySize );
				Files.Add( new FileInfo( infile, i, ContentBitmask, Endian, TextUtils.GameTextEncoding.ASCII ) );
			}

			FileLocationMultiplier = separateContentFile ? 1 : CalculateFileLocationMultiplier();
			ShouldGuessFilesizeFromNextFile = !ContentBitmask.ContainsFileSizes && !ContentBitmask.ContainsSectorSizes && CalculateIsLinear();

			if (FileLocationMultiplier != 1 && printProgressToConsole) {
				Console.WriteLine( "Guessed FPS4 file location multiplier of " + FileLocationMultiplier + "; extraction may be incorrect if this is a wrong guess." );
			}

			infile.Dispose();
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

		// This is used for the potentially >4GB containers in the Vesperia rerelease.
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

		// TODO: Clean up code duplication between Extract(), GetChildByIndex(), GetChildByName(), GetChildNames()!

		public void Extract(string dirname, bool noMetadataParsing = false, bool printProgressToConsole = false, string jsonOutputPath = null, bool absoluteJson = false) {
			if (printProgressToConsole) {
				Console.WriteLine("Content Bitmask: 0x" + ContentBitmask.Value.ToString("X4"));
			}

			System.IO.Directory.CreateDirectory( dirname );

			var json = new JsonObject();
			json.Add("ContentBitmask", JsonValue.Create(ContentBitmask.Value));
			json.Add("Unknown2", JsonValue.Create(Unknown2));
			if (ArchiveName != null) {
				json.Add("Comment", JsonValue.Create(ArchiveName));
			}
			json.Add("FileLocationMultiplier", JsonValue.Create(FileLocationMultiplier));
			json.Add("Endian", JsonValue.Create(Endian.ToString()));

			if (Files.Count == 0) {
				json.Add("NoFileTerminator", true);
			} else if (Files[Files.Count - 1].Location != null && Files[Files.Count - 1].Location.Value != contentFile.Length) {
				json.Add("FileTerminatorLocationValue", Files[Files.Count - 1].Location.Value);
			}
			List<JsonObject> jsonFiles = new List<JsonObject>();

			for (int i = 0; i < Files.Count - 1; ++i) {
				FileInfo fi = Files[i];

				JsonObject jsonFile = new JsonObject();
				jsonFiles.Add(jsonFile);

				if (fi.FileName != null) {
					jsonFile.Add("FileName", JsonValue.Create(fi.FileName));
				}
				if (fi.FileType != null) {
					jsonFile.Add("FileType", JsonValue.Create(fi.FileType));
				}
				if (fi.Unknown0x0080 != null) {
					jsonFile.Add("Unknown0x0080", JsonValue.Create(fi.Unknown0x0080));
				}
				if (fi.Unknown0x0100 != null) {
					jsonFile.Add("Unknown0x0100", JsonValue.Create(fi.Unknown0x0100));
				}
				if (fi.Metadata != null) {
					JsonArray jsonMetadata = new JsonArray();
					for (int j = 0; j < fi.Metadata.Count; ++j) {
						JsonObject jo = new JsonObject();
						if (fi.Metadata[j].Key != null) {
							jo.Add("Key", JsonValue.Create(fi.Metadata[j].Key));
						}
						jo.Add("Value", JsonValue.Create(fi.Metadata[j].Value));
						jsonMetadata.Add(jo);
					}
					jsonFile.Add("Metadata", jsonMetadata);
				}

				if ( fi.ShouldSkip ) {
					if (printProgressToConsole) {
						Console.WriteLine("Skipped #" + i.ToString("D4"));
					}
					continue;
				}

				if ( fi.Location == null ) {
					throw new Exception( "FPS4 extraction failure: Doesn't contain file start pointers!" );
				}
				uint? maybeFilesize = fi.GuessFileSize( ShouldGuessFilesizeFromNextFile ? Files : null );
				if ( maybeFilesize == null ) {
					throw new Exception( "FPS4 extraction failure: Doesn't contain filesize information!" );
				}

				long fileloc = (long)(fi.Location.Value) * FileLocationMultiplier;
				uint filesize = maybeFilesize.Value;
				(string path, string filename) = fi.GuessFilePathName(noMetadataParsing);

				if ( path != null ) {
					Directory.CreateDirectory( dirname + '/' + path );
					path = path + '/' + filename;
				} else {
					path = filename;
				}
				string outpath = Path.Combine(dirname, path);
				jsonFile.Add("PathOnDisk", JsonValue.Create(absoluteJson ? Path.GetFullPath(outpath) : outpath));
				FileStream outfile = new FileStream(outpath, FileMode.Create );

				if (printProgressToConsole) {
					Console.WriteLine("Extracting #" + i.ToString("D4") + ": " + path);
				}

				contentFile.Seek( fileloc, SeekOrigin.Begin );
				StreamUtils.CopyStream( contentFile, outfile, (int)filesize );
				outfile.Close();
			}

			json.Add("Files", new JsonArray(jsonFiles.ToArray()));

			if (jsonOutputPath != null) {
				File.WriteAllText(jsonOutputPath, JsonSerializer.Serialize(json, new JsonSerializerOptions { WriteIndented = true }));
			}
		}

		public override INode GetChildByIndex( long index ) {
			if ( index >= 0 && index < Files.Count - 1 ) {
				FileInfo fi = Files[(int)index];
				if ( fi.ShouldSkip ) {
					return null;
				}
				if ( fi.Location == null ) {
					return null;
				}
				uint? maybeFilesize = fi.GuessFileSize( ShouldGuessFilesizeFromNextFile ? Files : null );
				if ( maybeFilesize == null ) {
					return null;
				}

				long fileloc = (long)(fi.Location.Value) * FileLocationMultiplier;
				uint filesize = maybeFilesize.Value;
				return new FileFromStream( new PartialStream( contentFile, fileloc, filesize ) );
			}

			return null;
		}

		public override INode GetChildByName( string name ) {
			for ( int i = 0; i < Files.Count - 1; ++i ) {
				FileInfo fi = Files[i];
				if ( fi.ShouldSkip ) {
					continue;
				}
				if ( fi.Location == null ) {
					continue;
				}
				uint? maybeFilesize = fi.GuessFileSize( ShouldGuessFilesizeFromNextFile ? Files : null );
				if ( maybeFilesize == null ) {
					continue;
				}

				(string path, string filename) = fi.GuessFilePathName();
				if ( path != null ) {
					path = path + '/' + filename;
				} else {
					path = filename;
				}

				if ( path == name ) {
					return GetChildByIndex( i );
				}
			}

			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			var l = new List<string>( Files.Count - 1 );
			for ( int i = 0; i < Files.Count - 1; ++i ) {
				FileInfo fi = Files[i];
				if ( fi.ShouldSkip ) {
					continue;
				}
				if ( fi.Location == null ) {
					continue;
				}
				uint? maybeFilesize = fi.GuessFileSize( ShouldGuessFilesizeFromNextFile ? Files : null );
				if ( maybeFilesize == null ) {
					continue;
				}

				(string path, string filename) = fi.GuessFilePathName();
				if ( path != null ) {
					path = path + '/' + filename;
				} else {
					path = filename;
				}

				l.Add( path );
			}
			return l;
		}

		private static void DoPadding(byte[] padhelper, long padcount, Stream s) {
			while (padcount > 0) {
				if (padcount >= padhelper.Length) {
					s.Write(padhelper);
					padcount -= padhelper.Length;
				} else {
					s.Write(padhelper, 0, (int)padcount);
					return;
				}
			}
		}

		public static void Pack(
			List<PackFileInfo> files,
			Stream outputContentStream,
			ContentInfo ContentBitmask,
			EndianUtils.Endianness Endian,
			uint Unknown2,
			Stream infile,
			string ArchiveName,
			uint FirstFileStart,
			uint Alignment,
			Stream outputHeaderStream = null,
			uint? alignmentFirstFile = null,
			uint fileLocationMultiplier = 1,
			bool printProgressToConsole = false,
			uint? lastEntryPtrOverride = null,
			bool setSectorSizeSameAsFileSize = false
		) {
			if (Alignment == 0) {
				Alignment = 1;
			}

			byte[] padhelper = new byte[0x4000];
			uint alignmentFirstFileInternal = alignmentFirstFile ?? Alignment;
			if ( ( Alignment % fileLocationMultiplier ) != 0 ) {
				throw new Exception( "Invalid multiplier." );
			}
			if ( ( alignmentFirstFileInternal % fileLocationMultiplier ) != 0 ) {
				throw new Exception( "Invalid multiplier." );
			}

			uint FileCount = (uint)files.Count + 1;
			uint HeaderSize = 0x1C;

			ushort EntrySize = 0;

			if ( ContentBitmask.ContainsStartPointers ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsSectorSizes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFileSizes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFilenames ) { EntrySize += 0x20; }
			if ( ContentBitmask.ContainsFiletypes ) { EntrySize += 4; }
			if ( ContentBitmask.ContainsFileMetadata ) { EntrySize += 4; }
			if (ContentBitmask.Contains0x0080) { EntrySize += 4; }
			if (ContentBitmask.Contains0x0100) { EntrySize += 4; }

			bool headerToSeparateFile = false;
			if ( outputHeaderStream != null ) { headerToSeparateFile = true; }

			{
				Stream f = headerToSeparateFile ? outputHeaderStream : outputContentStream;
				f.Position = 0;

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
				for ( int i = 0; i < files.Count; ++i ) {
					var fi = files[i];
					if ( ContentBitmask.ContainsStartPointers ) { f.WriteUInt32( 0 ); } // properly written later
					if ( ContentBitmask.ContainsSectorSizes ) { f.WriteUInt32( 0 ); } // properly written later
					if ( ContentBitmask.ContainsFileSizes ) { f.WriteUInt32( ( (uint)( fi.FileSize ) ).ToEndian( Endian ) ); }
					if ( ContentBitmask.ContainsFilenames ) {
						string filename = fi.FileName.Truncate( 0x1F );
						byte[] fnbytes = TextUtils.ShiftJISEncoding.GetBytes( filename );
						f.Write( fnbytes, 0, fnbytes.Length );
						DoPadding(padhelper, 0x20 - fnbytes.Length, f);
					}
					if ( ContentBitmask.ContainsFiletypes ) {
						string extension = (fi.FileType != null ? fi.FileType : Path.GetExtension(fi.FileName)).TrimStart( '.' ).Truncate( 4 );
						byte[] extbytes = TextUtils.ShiftJISEncoding.GetBytes( extension );
						f.Write( extbytes, 0, extbytes.Length );
						DoPadding(padhelper, 4 - extbytes.Length, f);
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
					if (ContentBitmask.Contains0x0080) {
						if (infile != null) {
							infile.Position = f.Position;
							f.WriteUInt32(infile.ReadUInt32());
						} else {
							f.WriteUInt32(0);
						}
					}
					if (ContentBitmask.Contains0x0100) {
						if (infile != null) {
							infile.Position = f.Position;
							f.WriteUInt32(infile.ReadUInt32());
						} else {
							f.WriteUInt32(0);
						}
					}
				}

				// at the end of the file list, reserve space for a final entry pointing to the end of the container
				DoPadding(padhelper, EntrySize, f);

				// the idea is to write a pointer here
				// and at the target of the pointer you have a nullterminated string
				// with all the metadata in a param=data format separated by spaces
				// maybe including a filepath at the start without a param=
				// strings should be after the filelist block but before the actual files
				if (ContentBitmask.ContainsFileMetadata) {
					for ( int i = 0; i < files.Count; ++i ) {
						var fi = files[i];
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
						if (fi.Metadata != null) {
							foreach (var pair in fi.Metadata) {
								if (pair.Key == null) {
									f.Write(TextUtils.ShiftJISEncoding.GetBytes(pair.Value));
								} else {
									f.Write(TextUtils.ShiftJISEncoding.GetBytes(pair.Key + "=" + pair.Value));
								}
							}
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
					byte[] archiveNameBytes = TextUtils.ShiftJISEncoding.GetBytes( ArchiveName );
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
							FirstFileStart = ( (uint)( f.Position ) ).Align( alignmentFirstFileInternal );
						}
					}
					f.Position = 0xC;
					f.WriteUInt32( FirstFileStart.ToEndian( Endian ) );

					// file entries
					ulong ptr = FirstFileStart;
					ulong[] fileStarts = new ulong[files.Count];
					for (int i = 0; i < files.Count; ++i) {
						var fi = files[i];
						if (fi.DuplicateOf == null) {
							fileStarts[i] = ptr;
							ptr += (ulong)fi.FileSize.Align((int)Alignment);
						}
					}
					for (int i = 0; i < files.Count; ++i) {
						var fi = files[i];
						if (fi.DuplicateOf != null) {
							fileStarts[i] = fileStarts[fi.DuplicateOf.Value];
						}
					}

					for (int i = 0; i < files.Count; ++i) {
						f.Position = 0x1C + (i * EntrySize);
						var fi = files[i];
						if (ContentBitmask.ContainsStartPointers) {
							if (fi.DataStream == null) {
								f.WriteUInt32((uint)(0xffffffff), Endian);
							} else {
								f.WriteUInt32((uint)(fileStarts[i] / fileLocationMultiplier), Endian);
							}
						}
						if (ContentBitmask.ContainsSectorSizes) {
							if (fi.DataStream == null) {
								f.WriteUInt32((uint)(0), Endian);
							} else {
								f.WriteUInt32((uint)(setSectorSizeSameAsFileSize ? fi.FileSize : fi.FileSize.Align((int)Alignment)), Endian);
							}
						}
					}

					f.Position = 0x1C + ( files.Count * EntrySize );
					if (lastEntryPtrOverride == null) {
						f.WriteUInt32((uint)(ptr / fileLocationMultiplier), Endian);
					} else {
						f.WriteUInt32(lastEntryPtrOverride.Value, Endian);
					}

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
						DoPadding(padhelper, FirstFileStart - f.Length, f);
					}
				}

				// actually write files
				if ( headerToSeparateFile ) {
					Stream dataStream = outputContentStream;
					dataStream.Position = 0;
					WriteFilesToFileStream(padhelper, files, dataStream, Alignment, printProgressToConsole);
				} else {
					WriteFilesToFileStream(padhelper, files, f, Alignment, printProgressToConsole);
				}
			}
		}

		private static void WriteFilesToFileStream(byte[] padhelper, List<PackFileInfo> files, Stream f, uint Alignment, bool printProgressToConsole) {
			for ( int i = 0; i < files.Count; ++i ) {
				if (files[i].DuplicateOf != null) {
					continue;
				}

				if (files[i].DataStream == null) {
					continue;
				}

				using ( var fs = files[i].DataStream.Duplicate() ) {
					if (printProgressToConsole) {
						Console.WriteLine("Packing #" + i.ToString("D4") + ": " + (files[i].SourcePath != null ? files[i].SourcePath : files[i].FileName));
					}
					StreamUtils.CopyStream( fs, f, (int)fs.Length );
					if (Alignment > 1) {
						DoPadding(padhelper, f.Length.Align(Alignment) - f.Length, f);
					}
				}
			}
		}

		public static List<PackFileInfo> DetectDuplicates(List<PackFileInfo> files) {
			List<PackFileInfo> newlist = new List<PackFileInfo>(files.Count);
			for (int i = 0; i < files.Count; ++i) {
				var npfi = new PackFileInfo(files[i]);
				npfi.DuplicateOf = null;
				for (int j = 0; j < i; ++j) {
					if (files[i].DataStream != null && files[j].DataStream != null) {
						using (var istr = files[i].DataStream.Duplicate())
						using (var jstr = files[j].DataStream.Duplicate()) {
							if (StreamUtils.IsIdentical(istr, jstr)) {
								npfi.DuplicateOf = (ulong)j;
								break;
							}
						}
					}
				}
				newlist.Add(npfi);
			}
			return newlist;
		}

		public void Close() {
			if ( contentFile != null ) {
				contentFile.Close();
				contentFile = null;
			}
		}

		public override void Dispose() {
			Close();
		}

		// FIXME: this works only with pretty specific input, good enough for what I need but certainly not usable for generic cases
		public static string GetRelativePath( string fromDirectory, string toFile ) {
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
