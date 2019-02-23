using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class TOVNPCT {
		// NPC dialogue definition files, I think?
		public TOVNPCT( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading TOVNPCT failed!" );
				}
			}
		}

		public TOVNPCT( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading TOVNPCT failed!" );
			}
		}

		public List<NpcDialogueDefinition> NpcDefList;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "TOVNPCT\0" ) {
				throw new Exception( "Invalid magic." );
			}
			uint fileSize = stream.ReadUInt32().FromEndian( endian );
			uint unknownDataStart = stream.ReadUInt32().FromEndian( endian );
			uint unknownDataCount = stream.ReadUInt32().FromEndian( endian );
			uint npcDefStart = stream.ReadUInt32().FromEndian( endian );
			uint npcDefCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			stream.Position = npcDefStart;
			NpcDefList = new List<NpcDialogueDefinition>( (int)npcDefCount );
			for ( uint i = 0; i < npcDefCount; ++i ) {
				NpcDialogueDefinition n = new NpcDialogueDefinition( stream, refStringStart, endian, bits );
				NpcDefList.Add( n );
			}

			return true;
		}
	}
}
