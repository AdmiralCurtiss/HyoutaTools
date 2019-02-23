using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TOVNPC {
	public class TOVNPCL {
		// NPC file list
		public TOVNPCL( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading TOVNPCL failed!" );
				}
			}
		}

		public TOVNPCL( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading TOVNPCL failed!" );
			}
		}

		public List<NpcFileReference> NpcFileList;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "TOVNPCL\0" ) {
				throw new Exception( "Invalid magic." );
			}
			uint fileSize = stream.ReadUInt32().FromEndian( endian );
			uint dataStart = stream.ReadUInt32().FromEndian( endian );
			uint dataCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			stream.Position = dataStart;
			NpcFileList = new List<NpcFileReference>( (int)dataCount );
			for ( uint i = 0; i < dataCount; ++i ) {
				NpcFileReference n = new NpcFileReference( stream, refStringStart, endian, bits );
				NpcFileList.Add( n );
			}

			return true;
		}
	}
}
