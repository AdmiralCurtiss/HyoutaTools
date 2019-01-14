using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMGP {
	public class EnemyGroup {
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

		public EnemyGroup( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			uint entryLength = stream.ReadUInt32().FromEndian( endian );

			ID = stream.ReadUInt32().FromEndian( endian );
			StringDicID = stream.ReadUInt32().FromEndian( endian );
			InGameID = stream.ReadUInt32().FromEndian( endian );
			ulong refLoc = stream.ReadUInt( bits, endian );

			EnemyIDs = new int[8];
			for ( int i = 0; i < EnemyIDs.Length; ++i ) {
				EnemyIDs[i] = (int)stream.ReadUInt32().FromEndian( endian );
			}

			UnknownFloats = new float[8];
			for ( int i = 0; i < UnknownFloats.Length; ++i ) {
				UnknownFloats[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}
			PosX = new float[8];
			for ( int i = 0; i < PosX.Length; ++i ) {
				PosX[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}
			PosY = new float[8];
			for ( int i = 0; i < PosY.Length; ++i ) {
				PosY[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}
			Scale = new float[8];
			for ( int i = 0; i < Scale.Length; ++i ) {
				Scale[i] = stream.ReadUInt32().FromEndian( endian ).UIntToFloat();
			}

			SomeFlag = stream.ReadUInt32().FromEndian( endian );
			UnknownInts = new uint[8];
			for ( int i = 0; i < UnknownInts.Length; ++i ) {
				UnknownInts[i] = stream.ReadUInt32().FromEndian( endian );
			}

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refLoc ) );
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( T8BTEMST.T8BTEMST enemies, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage ) {
			var sb = new StringBuilder();

			sb.Append( "<tr id=\"egroup" + InGameID + "\">" );

			sb.Append( "<td>" );
			sb.Append( RefString );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicID].StringEngOrJpnHtml( version, websiteLanguage ) );
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
					sb.Append( inGameIdDict[enemies.EnemyIdDict[(uint)EnemyIDs[i]].NameStringDicID].StringEngOrJpnHtml( version, websiteLanguage ) );
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
