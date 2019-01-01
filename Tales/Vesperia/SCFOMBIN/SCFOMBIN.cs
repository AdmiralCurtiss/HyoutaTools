using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools.Tales.Vesperia.ScenarioFile;

namespace HyoutaTools.Tales.Vesperia.SCFOMBIN {
	public class SCFOMBIN {
		public SCFOMBIN() { }

		public SCFOMBIN( String filename, Util.Endianness endian, Util.GameTextEncoding encoding, uint textPointerLocationDiff = 0x1888 ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian, encoding, textPointerLocationDiff ) ) {
					throw new Exception( "Loading SCFOMBIN failed!" );
				}
			}
		}

		public SCFOMBIN( Stream stream, Util.Endianness endian, Util.GameTextEncoding encoding, uint textPointerLocationDiff = 0x1888 ) {
			if ( !LoadFile( stream, endian, encoding, textPointerLocationDiff ) ) {
				throw new Exception( "Loading SCFOMBIN failed!" );
			}
		}

		public List<ScenarioFileEntry> EntryList;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.GameTextEncoding encoding, uint textPointerLocationDiff ) {
			string magic = stream.ReadAscii( 8 );
			uint alwaysSame = stream.ReadUInt32().FromEndian( endian );
			uint filesize = stream.ReadUInt32().FromEndian( endian );

			uint lengthSection1 = stream.ReadUInt32().FromEndian( endian );

			stream.Position = 0x50;
			int textPointerDiffDiff = (int)stream.ReadUInt32().FromEndian( endian );
			stream.Position = 0x20;
			uint textStart = stream.ReadUInt32().FromEndian( endian );
			int textPointerDiff = (int)stream.ReadUInt32().FromEndian( endian ) - textPointerDiffDiff;

			EntryList = new List<ScenarioFileEntry>();

			// i wonder what the actual logic behind this is...
			uint textPointersLocation = ( lengthSection1 + 0x80 ).Align( 0x10 ) + textPointerLocationDiff;
			// + 0x1888; // + 0x1B4C // diff of 2C4 // Actually this isn't constant, dammit.

			if ( textStart != textPointersLocation ) {
				stream.Position = textPointersLocation;

				while ( true ) {
					long loc = stream.Position;
					stream.DiscardBytes( 8 );
					uint[] ptrs = new uint[4];
					ptrs[0] = stream.ReadUInt32().FromEndian( endian );
					ptrs[1] = stream.ReadUInt32().FromEndian( endian );
					ptrs[2] = stream.ReadUInt32().FromEndian( endian );
					ptrs[3] = stream.ReadUInt32().FromEndian( endian );

					if ( stream.Position > textStart ) { break; }
					if ( ptrs.Any( x => x == 0 ) ) { break; }
					if ( ptrs.Any( x => x + textPointerDiff < textStart ) ) { break; }
					if ( ptrs.Any( x => x + textPointerDiff >= filesize ) ) { break; }

					var s = new ScenarioFileEntry();
					s.Pointer = (uint)loc;
					s.JpName = stream.ReadNulltermStringFromLocationAndReset( ptrs[0] + textPointerDiff, encoding );
					s.JpText = stream.ReadNulltermStringFromLocationAndReset( ptrs[1] + textPointerDiff, encoding );
					s.EnName = stream.ReadNulltermStringFromLocationAndReset( ptrs[2] + textPointerDiff, encoding );
					s.EnText = stream.ReadNulltermStringFromLocationAndReset( ptrs[3] + textPointerDiff, encoding );
					EntryList.Add( s );

					stream.Position = loc + 0x18;
				}
			}

			return true;
		}

		public override string ToString() {
			return EntryList.Count + " entries";
		}
	}
}
