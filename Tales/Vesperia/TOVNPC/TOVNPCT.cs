using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class TOVNPCT {
		// NPC dialogue definition files, I think?
		public TOVNPCT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TOVNPCT failed!" );
				}
			}
		}

		public TOVNPCT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TOVNPCT failed!" );
			}
		}

		public List<NpcDialogueDefinition> NpcDefList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint fileSize = stream.ReadUInt32().SwapEndian();
			uint unknownDataStart = stream.ReadUInt32().SwapEndian();
			uint unknownDataCount = stream.ReadUInt32().SwapEndian();
			uint npcDefStart = stream.ReadUInt32().SwapEndian();
			uint npcDefCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			stream.Position = npcDefStart;
			NpcDefList = new List<NpcDialogueDefinition>( (int)npcDefCount );
			for ( uint i = 0; i < npcDefCount; ++i ) {
				NpcDialogueDefinition n = new NpcDialogueDefinition( stream, refStringStart );
				NpcDefList.Add( n );
			}

			return true;
		}
	}
}
