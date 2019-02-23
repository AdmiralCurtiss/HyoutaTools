using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTSK {
	public class T8BTSK {
		public T8BTSK( String filename, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading T8BTSK failed!" );
				}
			}
		}

		public T8BTSK( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading T8BTSK failed!" );
			}
		}

		public List<Skill> SkillList;
		public Dictionary<uint, Skill> SkillIdDict;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTSK  " ) {
				throw new Exception( "Invalid magic." );
			}
			uint skillCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			SkillList = new List<Skill>( (int)skillCount );
			for ( uint i = 0; i < skillCount; ++i ) {
				Skill s = new Skill( stream, refStringStart, endian, bits );
				SkillList.Add( s );
			}

			SkillIdDict = new Dictionary<uint, Skill>( SkillList.Count );
			foreach ( Skill s in SkillList ) {
				SkillIdDict.Add( s.InGameID, s );
			}

			return true;
		}
	}
}
