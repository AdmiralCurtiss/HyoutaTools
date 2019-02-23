using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTEMEG {
	public class T8BTEMEG {
		public T8BTEMEG( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTEMEG failed!" );
				}
			}
		}

		public T8BTEMEG( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTEMEG failed!" );
			}
		}

		public List<EncounterGroup> EncounterGroupList;
		public Dictionary<uint, EncounterGroup> EncounterGroupIdDict;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTEMEG" ) {
				throw new Exception( "Invalid magic." );
			}
			uint encounterGroupCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			EncounterGroupList = new List<EncounterGroup>( (int)encounterGroupCount );
			for ( uint i = 0; i < encounterGroupCount; ++i ) {
				EncounterGroup s = new EncounterGroup( stream, refStringStart, endian, bits );
				EncounterGroupList.Add( s );
			}

			EncounterGroupIdDict = new Dictionary<uint, EncounterGroup>( EncounterGroupList.Count );
			foreach ( EncounterGroup e in EncounterGroupList ) {
				EncounterGroupIdDict.Add( e.InGameID, e );
			}

			return true;
		}
	}
}
