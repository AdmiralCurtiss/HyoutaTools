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

	public class ScenarioDat {
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
	}
}
