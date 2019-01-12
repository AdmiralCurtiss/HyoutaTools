using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class SkitInfo : IComparable<SkitInfo> {
		// always 1
		public ushort Unknown2;

		// flag that must be set to allow skit to show
		public uint FlagTrigger;
		// but if this flag is set skit won't show anymore
		public uint FlagCancel;
		public uint Category;
		// similar to an ID; presumably the game sets SkitsActivated[SkitFlag] = true; when a skit triggers
		// and checks those flags for if a skit should show or not if the conditions are met
		// multiple skits may have the same flag so triggering one sets all as watched, such as the Heliord costume choices
		public uint SkitFlag;
		// similar to the skit flag, except gives unique values for the Heliord stuff (and similar in theory?)
		public ushort SkitFlagUnique;
		public uint CharacterBitmask;
		public uint Unknown4;
		// ID of the data structure holding this skit's trigger condition, -1 if none
		public long SkitConditionForwarderReference;
		public uint SkitConditionRelated;

		public string RefString;
		public uint StringDicIdName;
		public uint StringDicIdCondition;

		public string CategoryString {
			get {
				switch ( Category ) {
					case 0: return "Scenario";
					case 2: return "Chat";
					case 3: return "Battle";
					case 5: return "Cooking";
					case 7: return "Generic";
					default: return "[UNKNOWN CATEGORY " + Category + "]";
				}
			}
		}

		public SkitInfo( System.IO.Stream stream, uint refStringStart, Util.Endianness endian, Util.Bitness bits ) {
			// first 16 bytes are always null in the existing files
			stream.DiscardBytes( 0x10 );

			SkitFlagUnique = stream.ReadUInt16().FromEndian( endian );
			Unknown2 = stream.ReadUInt16().FromEndian( endian );

			FlagTrigger = stream.ReadUInt32().FromEndian( endian );
			FlagCancel = stream.ReadUInt32().FromEndian( endian );
			Category = stream.ReadUInt32().FromEndian( endian );
			CharacterBitmask = stream.ReadUInt32().FromEndian( endian );
			Unknown4 = stream.ReadUInt32().FromEndian( endian );
			SkitConditionForwarderReference = stream.ReadInt( bits ).FromEndian( endian );
			SkitConditionRelated = stream.ReadUInt32().FromEndian( endian );
			SkitFlag = stream.ReadUInt32().FromEndian( endian );
			ulong refStringPos = stream.ReadUInt( bits ).FromEndian( endian );
			StringDicIdName = stream.ReadUInt32().FromEndian( endian );
			StringDicIdCondition = stream.ReadUInt32().FromEndian( endian );

			RefString = stream.ReadAsciiNulltermFromLocationAndReset( (long)( refStringStart + refStringPos ) );

			return;
		}

		public override string ToString() {
			return RefString;
		}

		public string GetIndexDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TO8CHLI skits, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();

			string url = HyoutaTools.Tales.Vesperia.Website.WebsiteGenerator.GetUrl( Website.WebsiteSection.Skit, version, versionPostfix, locale, websiteLanguage, phpLinks, extra: RefString );

			sb.Append( "<tr>" );

			sb.Append( "<td>" );
			sb.Append( CategoryString );
			sb.Append( "</td>" );
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<td>" );
				sb.Append( "<a href=\"" + url + "\">" );
				sb.Append( inGameIdDict[StringDicIdName].StringJpnHtml( version ) );
				sb.Append( "</a>" );
				sb.Append( "</td>" );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<td>" );
				sb.Append( "<a href=\"" + url + "\">" );
				sb.Append( inGameIdDict[StringDicIdName].StringEngHtml( version ) );
				sb.Append( "</a>" );
				sb.Append( "</td>" );
			}
			sb.Append( "<td>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharacterBitmask );
			sb.Append( "</td>" );

			sb.Append( "</tr>" );

			return sb.ToString();
		}
		public string GetDataAsHtml( GameVersion version, string versionPostfix, GameLocale locale, WebsiteLanguage websiteLanguage, TO8CHLI skits, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( RefString );
			if ( websiteLanguage.WantsJp() ) {
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[StringDicIdName].StringJpnHtml( version ) );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[StringDicIdCondition].StringJpnHtml( version ) );
			}
			if ( websiteLanguage.WantsEn() ) {
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[StringDicIdName].StringEngHtml( version ) );
				sb.Append( "<br>" );
				sb.Append( inGameIdDict[StringDicIdCondition].StringEngHtml( version ) );
			}
			sb.Append( "<br>Category: " );
			sb.Append( CategoryString );
			sb.Append( "<br>Available after event: " );
			sb.Append( FlagTrigger );
			if ( FlagCancel == 9999999 ) {
				sb.Append( "<br>Never expires." );
			} else {
				sb.Append( "<br>Expires after event: " );
				sb.Append( FlagCancel );
			}
			if ( SkitConditionForwarderReference == -1 ) {
				Util.Assert( SkitConditionRelated == 0 );
				sb.Append( "<br>No special condition." );
			} else {
				Util.Assert( SkitConditionRelated > 0 );
				var fw = skits.SkitConditionForwarderList[(int)SkitConditionForwarderReference];
				/*
				sb.AppendLine();
				sb.Append( "<br>Trigger Condition #" + SkitConditionForwarderReference );
				sb.Append( " / Condition: " + fw.SkitConditionReference );
				sb.Append( " / Count: " + fw.SkitConditionCount );
				//*/
				for ( uint i = 0; i < fw.SkitConditionCount; ++i ) {
					var c = skits.SkitConditionList[(int)( fw.SkitConditionReference + i )];
					sb.Append( "<br>" );
					c.GetDataAsHtml( sb, version );
				}
				sb.Append( "~SkitConditionRelated: " + SkitConditionRelated );
			}

			sb.Append( "<br>" );
			Website.WebsiteGenerator.AppendCharacterBitfieldAsImageString( sb, version, CharacterBitmask );
												  
			sb.Append( "<br>" );
			sb.Append( "~4: 0x" + Unknown4.ToString("X8") );
			sb.Append( "<br>" );

			return sb.ToString();
		}

		public int CompareTo( SkitInfo other ) {
			return this.SkitFlagUnique.CompareTo( other.SkitFlagUnique );
		}
	}
}
