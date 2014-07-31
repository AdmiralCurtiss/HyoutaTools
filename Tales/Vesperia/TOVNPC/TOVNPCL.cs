using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class TOVNPCL {
		// NPC file list
		public TOVNPCL( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TOVNPCL failed!" );
				}
			}
		}

		public TOVNPCL( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TOVNPCL failed!" );
			}
		}

		public List<NpcFileReference> NpcFileList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint fileSize = stream.ReadUInt32().SwapEndian();
			uint dataStart = stream.ReadUInt32().SwapEndian();
			uint dataCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			stream.Position = dataStart;
			NpcFileList = new List<NpcFileReference>( (int)dataCount );
			for ( uint i = 0; i < dataCount; ++i ) {
				NpcFileReference n = new NpcFileReference( stream, refStringStart );
				NpcFileList.Add( n );
			}

			return true;
		}
	}
}
