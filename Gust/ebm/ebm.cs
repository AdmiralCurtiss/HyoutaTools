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

		public ebmEntry( Stream stream, bool isUtf8 = false ) {
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
			if ( isUtf8 ) {
				Text = stream.ReadUTF8Nullterm();
			} else {
				Text = stream.ReadShiftJisNullterm();
			}
			stream.Position = pos + TextLength;
		}

		public override string ToString() {
			return Text;
		}
	}

	public class ebm {
		public ebm( String filename, bool isUtf8 = false ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, isUtf8 ) ) {
					throw new Exception( "Loading ebm failed!" );
				}
			}
		}

		public ebm( Stream stream, bool isUtf8 = false ) {
			if ( !LoadFile( stream, isUtf8 ) ) {
				throw new Exception( "Loading ebm failed!" );
			}
		}

		public List<ebmEntry> EntryList;

		private bool LoadFile( Stream stream, bool isUtf8 = false ) {
			uint entryCount = stream.ReadUInt32();

			EntryList = new List<ebmEntry>( (int)entryCount );
			for ( uint i = 0; i < entryCount; ++i ) {
				ebmEntry e = new ebmEntry( stream, isUtf8 );
				EntryList.Add( e );
			}

			return true;
		}
	}
}
