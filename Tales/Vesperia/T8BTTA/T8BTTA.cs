using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTTA {
	public class T8BTTA {
		public T8BTTA( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading T8BTTA failed!" );
				}
			}
		}

		public T8BTTA( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading T8BTTA failed!" );
			}
		}

		public List<StrategySet> StrategySetList;
		public List<StrategyOption> StrategyOptionList;
		public Dictionary<uint, StrategyOption> StrategyOptionDict;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint strategySetCount = stream.ReadUInt32().FromEndian( endian );
			uint strategyOptionCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			StrategySetList = new List<StrategySet>( (int)strategySetCount );
			for ( uint i = 0; i < strategySetCount; ++i ) {
				StrategySet ss = new StrategySet( stream, refStringStart, endian );
				StrategySetList.Add( ss );
			}
			StrategyOptionList = new List<StrategyOption>( (int)strategyOptionCount );
			for ( uint i = 0; i < strategyOptionCount; ++i ) {
				StrategyOption so = new StrategyOption( stream, refStringStart, endian );
				StrategyOptionList.Add( so );
			}

			StrategyOptionDict = new Dictionary<uint, StrategyOption>( StrategyOptionList.Count );
			foreach ( var option in StrategyOptionList ) {
				StrategyOptionDict.Add( option.InGameID, option );
			}

			return true;
		}
	}
}
