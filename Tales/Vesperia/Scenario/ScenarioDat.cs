using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools.FileContainer;
using HyoutaTools.Streams;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.Scenario {
	public class ScenarioDatEntry : IFile {
		public uint Offset;
		public uint FilesizeCompressed;
		public uint FilesizeUncompressed;

		public DuplicatableStream DataStream { get; set; }
		public bool HasData => DataStream != null;

		public ScenarioDatEntry() { }
		public ScenarioDatEntry( DuplicatableStream data, uint FilesOffset ) {
			Offset = data.ReadUInt32().SwapEndian();
			FilesizeCompressed = data.ReadUInt32().SwapEndian();
			FilesizeUncompressed = data.ReadUInt32().SwapEndian();

			if ( FilesizeCompressed > 0 ) {
				DataStream = new PartialStream( data, Offset + FilesOffset, FilesizeCompressed );
			} else {
				DataStream = null;
			}
		}

		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public void Dispose() {
			DataStream.Dispose();
		}
	}

	public class ScenarioDat : ContainerBase {
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
		public ScenarioDat( HyoutaPluginBase.DuplicatableStream data ) {
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
				if ( e.HasData ) {
					using ( var fs = new System.IO.FileStream( System.IO.Path.Combine( outPath, i.ToString() ), FileMode.Create ) ) {
						e.DataStream.ReStart();
						StreamUtils.CopyStream( e.DataStream, fs, (int)e.DataStream.Length );
						e.DataStream.End();
					}
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

				Entries[fileNumber].DataStream = new DuplicatableFileStream( f );
				Entries[fileNumber].FilesizeCompressed = (uint)Entries[fileNumber].DataStream.Length;

				Entries[fileNumber].DataStream.Position = 0x05;
				Entries[fileNumber].FilesizeUncompressed = Entries[fileNumber].DataStream.ReadUInt32();
				Entries[fileNumber].DataStream.Position = 0;
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
				if ( Entries[i].DataStream != null ) {
					// special case: if multiple files in a row are identical, only add them once and point the offsets at the same data
					// oddly enough for the two times it happens in the original files, it only does it once, so no idea what is actually
					// the proper way to handle it, faking it with that last condition
					if ( i >= 1 && Entries[i - 1].DataStream != null && Entries[i - 1].DataStream.Length == Entries[i].DataStream.Length && Entries[i].DataStream.Length > 0x30 ) {
						Entries[i - 1].DataStream.Position = 0;
						Entries[i].DataStream.Position = 0;
						if ( Entries[i - 1].DataStream.IsIdentical( Entries[i].DataStream, Entries[i].DataStream.Length ) ) {
							Entries[i].Offset = Entries[i - 1].Offset;
							continue;
						}
					}

					Entries[i].Offset = (uint)f.Position - FilesOffset;
					Entries[i].DataStream.Position = 0;
					StreamUtils.CopyStream( Entries[i].DataStream, f, (int)Entries[i].DataStream.Length );
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

		public override void Dispose() {
			if ( Entries != null ) {
				for ( int i = 0; i < Entries.Count; ++i ) {
					if ( Entries[i].DataStream != null ) {
						Entries[i].DataStream.Dispose();
						Entries[i].DataStream = null;
					}
				}
			}
		}

		public override INode GetChildByIndex( long index ) {
			if ( index >= 0 && index < Entries.Count ) {
				var e = Entries[(int)index];
				if ( e.HasData ) {
					return e;
				}
			}
			return null;
		}

		public override INode GetChildByName( string name ) {
			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			return new List<string>();
		}
	}
}
