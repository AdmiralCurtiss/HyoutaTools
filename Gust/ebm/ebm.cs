using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Gust.ebm {
	public class ebmEntry {
		public uint Ident;
		public uint Unknown2;
		public uint Unknown3;
		public int CharacterId;

		public uint Unknown5;
		public uint Unknown6;
		public uint Unknown7;
		public uint Unknown8;

		public uint TextLength;

		public string Text;

		public ebmEntry( Stream stream, Util.GameTextEncoding encoding ) {
			Ident = stream.ReadUInt32();
			Unknown2 = stream.ReadUInt32();
			Unknown3 = stream.ReadUInt32();
			CharacterId = (int)stream.ReadUInt32();
			Unknown5 = stream.ReadUInt32();
			Unknown6 = stream.ReadUInt32();
			Unknown7 = stream.ReadUInt32();
			Unknown8 = stream.ReadUInt32();
			TextLength = stream.ReadUInt32();

			long pos = stream.Position;
			Text = stream.ReadNulltermString( encoding );
			stream.Position = pos + TextLength;
		}

		public override string ToString() {
			return Text;
		}
	}

	public class ebm {
		public ebm( String filename, Util.GameTextEncoding encoding ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, encoding ) ) {
					throw new Exception( "Loading ebm failed!" );
				}
			}
		}

		public ebm( Stream stream, Util.GameTextEncoding encoding ) {
			if ( !LoadFile( stream, encoding ) ) {
				throw new Exception( "Loading ebm failed!" );
			}
		}

		public List<ebmEntry> EntryList;

		private bool LoadFile( Stream stream, Util.GameTextEncoding encoding ) {
			uint entryCount = stream.ReadUInt32();

			EntryList = new List<ebmEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				ebmEntry e = new ebmEntry( stream, encoding );
				EntryList.Add( e );
			}

			return true;
		}
	}
}
