using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.Scenario {
	public class ScenarioDatEntry {
		public uint Offset;
		public uint FilesizeCompressed;
		public uint FilesizeUncompressed;

		public Stream Data = null;

		public ScenarioDatEntry() { }
		public ScenarioDatEntry( Stream data, uint FilesOffset ) {
			Offset = data.ReadUInt32().SwapEndian();
			FilesizeCompressed = data.ReadUInt32().SwapEndian();
			FilesizeUncompressed = data.ReadUInt32().SwapEndian();

			if ( FilesizeCompressed > 0 ) {
				data.Position = Offset + FilesOffset;
				Data = new MemoryStream( (int)FilesizeCompressed );
				Util.CopyStream( data, Data, (int)FilesizeCompressed );
			}
		}
	}

	public class ScenarioDat : IDisposable {
		public string Magic;
		public uint Filesize;
		public uint Unknown;

		public uint Filecount;
		public uint FilesOffset;

		public List<ScenarioDatEntry> Entries;

		public ScenarioDat() {
			Magic = "TO8SCEL\0";
			Unknown = 0x20;
			Entries = new List<ScenarioDatEntry>();
		}
		public ScenarioDat( Stream data ) {
			Magic = data.ReadAscii( 8 );
			Filesize = data.ReadUInt32().SwapEndian();
			Unknown = data.ReadUInt32().SwapEndian();

			Filecount = data.ReadUInt32().SwapEndian();
			FilesOffset = data.ReadUInt32().SwapEndian();
			uint FilesizeAgain = data.ReadUInt32();
			uint Padding = data.ReadUInt32();

			Entries = new List<ScenarioDatEntry>( (int)Filecount );
			for ( uint i = 0; i < Filecount; ++i ) {
				data.Position = 0x20u + i * 0x20u;
				var e = new ScenarioDatEntry( data, FilesOffset );
				Entries.Add( e );
			}
		}

		public void Extract( string outPath ) {
			System.IO.Directory.CreateDirectory( outPath );
			for ( int i = 0; i < Entries.Count; ++i ) {
				var e = Entries[i];
				if ( e.FilesizeCompressed > 0 ) {
					var fs = new System.IO.FileStream( System.IO.Path.Combine( outPath, i.ToString() ), FileMode.Create );
					e.Data.Position = 0;
					Util.CopyStream( e.Data, fs, (int)e.Data.Length );
					fs.Close();
				}
			}
		}

		public void Import( string inDirectory ) {
			string[] files = System.IO.Directory.GetFiles( inDirectory );

			Entries = new List<ScenarioDatEntry>( files.Length );
			foreach ( string f in files ) {
				int fileNumber = Int32.Parse( System.IO.Path.GetFileNameWithoutExtension( f ) );

				// make sure an entry for that file exists
				for ( int i = Entries.Count; i <= fileNumber; ++i ) { Entries.Add( new ScenarioDatEntry() ); }

				Entries[fileNumber].Data = new FileStream( f, FileMode.Open );
				Entries[fileNumber].FilesizeCompressed = (uint)Entries[fileNumber].Data.Length;

				Entries[fileNumber].Data.Position = 0x05;
				Entries[fileNumber].FilesizeUncompressed = Entries[fileNumber].Data.ReadUInt32();
				Entries[fileNumber].Data.Position = 0;
			}

			Filecount = (uint)Entries.Count;
			FilesOffset = Filecount * 0x20 + 0x20;
		}

		public void Write( string outFile, int align = 0x10 ) {
			var f = new FileStream( outFile, FileMode.Create );

			// create space for header and filelist stuff
			for ( int i = 0; i < FilesOffset; ++i ) { f.WriteByte( 0 ); }

			// create dummy entry
			f.Write( new byte[] { 0x44, 0x55, 0x4D, 0x4D, 0x59, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, 0, 0x10 );

			// write files
			for ( int i = 0; i < Entries.Count; ++i ) {
				if ( Entries[i].Data != null ) {
					// special case: if multiple files in a row are identical, only add them once and point the offsets at the same data
					// oddly enough for the two times it happens in the original files, it only does it once, so no idea what is actually
					// the proper way to handle it, faking it with that last condition
					if ( i >= 1 && Entries[i - 1].Data != null && Entries[i - 1].Data.Length == Entries[i].Data.Length && Entries[i].Data.Length > 0x30 ) {
						Entries[i - 1].Data.Position = 0;
						Entries[i].Data.Position = 0;
						if ( Entries[i - 1].Data.IsIdentical( Entries[i].Data, Entries[i].Data.Length ) ) {
							Entries[i].Offset = Entries[i - 1].Offset;
							continue;
						}
					}

					Entries[i].Offset = (uint)f.Position - FilesOffset;
					Entries[i].Data.Position = 0;
					Util.CopyStream( Entries[i].Data, f, (int)Entries[i].Data.Length );
					while ( f.Length % align != 0 ) { f.WriteByte( 0 ); }
				}
			}

			Filesize = (uint)f.Length;

			// write header
			f.Position = 0;
			f.Write( Encoding.ASCII.GetBytes( Magic ), 0, 8 );
			f.WriteUInt32( Filesize.SwapEndian() );
			f.WriteUInt32( Unknown.SwapEndian() );
			f.WriteUInt32( Filecount.SwapEndian() );
			f.WriteUInt32( FilesOffset.SwapEndian() );
			f.WriteUInt32( Filesize );
			f.WriteUInt32( 0 );

			// write file list
			for ( int i = 0; i < Entries.Count; ++i ) {
				f.Position = 0x20u + i * 0x20u;
				f.WriteUInt32( Entries[i].Offset.SwapEndian() );
				f.WriteUInt32( Entries[i].FilesizeCompressed.SwapEndian() );
				f.WriteUInt32( Entries[i].FilesizeUncompressed.SwapEndian() );
			}

			f.Close();
		}

		public void Dispose() {
			if ( Entries != null ) {
				for ( int i = 0; i < Entries.Count; ++i ) {
					if ( Entries[i].Data != null ) {
						Entries[i].Data.Close();
					}
				}
			}
		}
	}
}
