using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitInfo {
		uint[] Data;

		// always 1
		public ushort Unknown2;

		// flag that must be set to allow skit to show
		public uint FlagTrigger;
		// but if this flag is set skit won't show anymore
		public uint FlagCancel;
		// similar to an ID; presumably the game sets SkitsActivated[SkitFlag] = true; when a skit triggers
		// and checks those flags for if a skit should show or not if the conditions are met
		// multiple skits may have the same flag so triggering one sets all as watched, such as the Heliord costume choices
		public uint SkitFlag;
		// similar to the skit flag, except gives unique values for the Heliord stuff (and similar in theory?)
		public ushort SkitFlagUnique;
		public uint CharacterBitmask;
		// ID of the data structure holding this skit's trigger condition, -1 if none
		public int SkitConditionForwarderReference;

		public string RefString;
		public uint StringDicIdName;
		public uint StringDicIdCondition;

		public SkitInfo( System.IO.Stream stream, uint refStringStart ) {
			// first 16 bytes are always null in the existing files
			stream.DiscardBytes( 0x10 );

			SkitFlagUnique = stream.ReadUInt16().SwapEndian();
			Unknown2 = stream.ReadUInt16().SwapEndian();

			Data = new uint[11];
			for ( int i = 0; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
			}


			FlagTrigger = Data[0];
			FlagCancel = Data[1];
			CharacterBitmask = Data[3];
			SkitConditionForwarderReference = (int)Data[5];
			SkitFlag = Data[7];
			uint refStringPos = Data[8];
			StringDicIdName = Data[9];
			StringDicIdCondition = Data[10];

			long pos = stream.Position;
			stream.Position = refStringStart + refStringPos;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;

			return;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetDataAsHtml( GameVersion version, TO8CHLI skits, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( RefString );
			sb.Append( "<br>" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[StringDicIdName].StringJPN, true, true ) );
			sb.Append( "<br>" );
			sb.Append( VesperiaUtil.RemoveTags( inGameIdDict[StringDicIdCondition].StringJPN, true, true ) );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicIdName].StringENG );
			sb.Append( "<br>" );
			sb.Append( inGameIdDict[StringDicIdCondition].StringENG );
			sb.Append( "<br>Available after event: " );
			sb.Append( FlagTrigger );
			if ( FlagCancel == 9999999 ) {
				sb.Append( "<br>Never expires." );
			} else {
				sb.Append( "<br>Expires after event: " );
				sb.Append( FlagCancel );
			}
			if ( SkitConditionForwarderReference == -1 ) {
				sb.Append( "<br>No special condition." );
			} else {
				var fw = skits.SkitConditionForwarderList[SkitConditionForwarderReference];
				sb.AppendLine();
				sb.Append( "<br>Trigger Condition #" + SkitConditionForwarderReference );
				sb.Append( " / Condition: " + fw.SkitConditionReference );
				sb.Append( " / Count: " + fw.SkitConditionCount );
				for ( int i = 0; i < fw.SkitConditionCount; ++i ) {
					var c = skits.SkitConditionList[(int)( fw.SkitConditionReference + i )];
					sb.AppendLine();
					sb.Append( "<br>#1a: " + c.Type );
					sb.Append( " / #1b: " + c.MathOp );
					sb.Append( " / #2a: " + c.Unknown2a );
					sb.Append( " / #2b: " + c.Value1 );
					sb.Append( " / #3: " + c.Value2 );
					sb.Append( " / #4: " + c.Value3 );
				}
			}

			sb.Append( "<br>" );
			Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, version, CharacterBitmask );

			for ( int i = 2; i < 7; ++i ) {
				if ( i == 3 || i == 5 ) { continue; }
				sb.AppendLine( "<br>" );
				sb.AppendLine( "~" + i + ": " + Data[i] );
			}
			sb.Append( "<br>" );

			return sb.ToString();
		}
	}
}
