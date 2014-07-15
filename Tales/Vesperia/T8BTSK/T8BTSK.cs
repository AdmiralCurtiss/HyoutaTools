using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTSK {
	public class T8BTSK {
		public T8BTSK( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTSK failed!" );
				}
			}
		}

		public T8BTSK( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTSK failed!" );
			}
		}

		public List<Skill> SkillList;
		public Dictionary<uint, Skill> SkillIdDict;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint skillCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			SkillList = new List<Skill>( (int)skillCount );
			for ( uint i = 0; i < skillCount; ++i ) {
				Skill s = new Skill( stream, refStringStart );
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
