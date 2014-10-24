using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMGP {
	public class EnemyGroup {
		uint[] Data;

		public uint ID;
		public uint StringDicID;
		public uint InGameID;
		public string RefString;
		public int[] EnemyIDs;
		public float[] UnknownFloats;
		public float[] PosX;
		public float[] PosY;
		public float[] Scale;
		public uint SomeFlag;
		public uint[] UnknownInts;

		public EnemyGroup( System.IO.Stream stream, uint refStringStart ) {
			uint entryLength = stream.PeekUInt32().SwapEndian();
			Data = new uint[entryLength / 4];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}

			ID = Data[1];
			StringDicID = Data[2];
			InGameID = Data[3];

			EnemyIDs = new int[8];
			for ( int i = 0; i < EnemyIDs.Length; ++i ) {
				EnemyIDs[i] = (int)Data[5 + i];
			}

			UnknownFloats = new float[8];
			for ( int i = 0; i < UnknownFloats.Length; ++i ) {
				UnknownFloats[i] = Data[13 + i].UIntToFloat();
			}
			PosX = new float[8];
			for ( int i = 0; i < PosX.Length; ++i ) {
				PosX[i] = Data[21 + i].UIntToFloat();
			}
			PosY = new float[8];
			for ( int i = 0; i < PosY.Length; ++i ) {
				PosY[i] = Data[29 + i].UIntToFloat();
			}
			Scale = new float[8];
			for ( int i = 0; i < Scale.Length; ++i ) {
				Scale[i] = Data[37 + i].UIntToFloat();
			}

			SomeFlag = Data[45];
			UnknownInts = new uint[8];
			for ( int i = 0; i < UnknownInts.Length; ++i ) {
				UnknownInts[i] = Data[46 + i];
			}

			long pos = stream.Position;
			stream.Position = refStringStart + Data[4];
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( T8BTEMST.T8BTEMST enemies, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			var sb = new StringBuilder();

			sb.Append( "<tr id=\"egroup" + InGameID + "\">" );

			sb.Append( "<td>" );
			sb.Append( RefString );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicID].StringEngOrJpnHtml( GameVersion.PS3 ) );
			sb.Append( "<br>" );
			sb.Append( "ID: " );
			sb.Append( ID );
			sb.Append( "<br>" );
			sb.Append( "InGameId: " );
			sb.Append( InGameID );
			sb.Append( "<br>" );
			sb.Append( "Flag: " );
			sb.Append( SomeFlag );
			sb.Append( "</td>" );

			for ( int i = 0; i < EnemyIDs.Length; ++i ) {
				sb.Append( "<td>" );
				if ( EnemyIDs[i] >= 0 ) {
					sb.Append( inGameIdDict[enemies.EnemyIdDict[(uint)EnemyIDs[i]].NameStringDicID].StringEngOrJpnHtml( GameVersion.PS3 ) );
					sb.Append( "<br>" );
					sb.Append( "~1: " );
					sb.Append( UnknownFloats[i] );
					sb.Append( "<br>" );
					sb.Append( "X Pos: " );
					sb.Append( PosX[i] );
					sb.Append( "<br>" );
					sb.Append( "Y Pos: " );
					sb.Append( PosY[i] );
					sb.Append( "<br>" );
					sb.Append( "Scale: " );
					sb.Append( Scale[i] );
					sb.Append( "<br>" );
					sb.Append( "~5: " );
					sb.Append( UnknownInts[i] );
				}
				sb.Append( "</td>" );
			}

			sb.Append( "</tr>" );

			return sb.ToString();
		}
	}
}
