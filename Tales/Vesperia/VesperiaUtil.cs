using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia {
	public enum GameVersion {
		X360_US, X360_EU, PS3, PC
	}
	public enum GameLocale {
		J, US, UK, DE, FR, BRA, DEU, ENG, ESN, ESP, FRA, ITA, KOR, RUS, ZHT
	}
	public enum ControllerButton {
		Start, Select, RightButton, LowerButton, LeftButton, UpperButton, L1, L2, L3, R1, R2, R3, LeftStick, RightStick, DPad, System
	}
	public enum WebsiteLanguage { // which of the two ToV strings to display in generated website data
		Jp, En, BothWithJpLinks, BothWithEnLinks
	}

	public static class VesperiaUtil {
		public static bool Is360( this GameVersion version ) {
			return version == GameVersion.X360_US || version == GameVersion.X360_EU;
		}
		public static bool HasPS3Content( this GameVersion version ) {
			return !version.Is360();
		}
		public static bool SwapsConfirmAndCancelDependingOnRegion( this GameVersion version ) {
			return version == GameVersion.PS3;
		}

		public static bool WantsJp( this WebsiteLanguage lang ) {
			return lang != WebsiteLanguage.En;
		}
		public static bool WantsEn( this WebsiteLanguage lang ) {
			return lang != WebsiteLanguage.Jp;
		}
		public static bool WantsBoth( this WebsiteLanguage lang ) {
			return lang.WantsJp() && lang.WantsEn();
		}
		public static bool MainJp( this WebsiteLanguage lang ) {
			return lang == WebsiteLanguage.Jp || lang == WebsiteLanguage.BothWithJpLinks;
		}
		public static bool MainEn( this WebsiteLanguage lang ) {
			return lang == WebsiteLanguage.En || lang == WebsiteLanguage.BothWithEnLinks;
		}

		public static Util.Endianness GetEndian( this GameVersion version ) {
			return version == GameVersion.PC ? Util.Endianness.LittleEndian : Util.Endianness.BigEndian;
		}
		public static Util.Bitness GetBitness( this GameVersion version ) {
			return version == GameVersion.PC ? Util.Bitness.B64 : Util.Bitness.B32;
		}
		public static Util.GameTextEncoding GetEncoding( this GameVersion version ) {
			return version == GameVersion.X360_EU || version == GameVersion.PC ? Util.GameTextEncoding.UTF8 : Util.GameTextEncoding.ShiftJIS;
		}
		public static GameLocale[] GetValidLocales( this GameVersion version ) {
			switch ( version ) {
				case GameVersion.X360_US: return new GameLocale[] { GameLocale.US };
				case GameVersion.X360_EU: return new GameLocale[] { GameLocale.UK, GameLocale.FR, GameLocale.DE };
				case GameVersion.PS3: return new GameLocale[] { GameLocale.J };
				case GameVersion.PC: return new GameLocale[] { GameLocale.ENG, GameLocale.FRA, GameLocale.DEU, GameLocale.ITA, GameLocale.ESP, GameLocale.ESN, GameLocale.BRA, GameLocale.RUS, GameLocale.KOR, GameLocale.ZHT };
				default: throw new Exception( "Invalid version." );
			}
		}

		public static string RemoveTags( string s, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool useJapaneseNames = false, bool outputAsHtml = false, bool removeKanjiWithFurigana = false ) {
			s = s.Replace( "\x04(YUR)",   inGameIdDict[33902112].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(EST)",   inGameIdDict[33902115].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(EST_P)", inGameIdDict[33902117].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(KAR)",   inGameIdDict[33902118].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(RIT)",   inGameIdDict[33902121].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(RAV)",   inGameIdDict[33902124].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(JUD)",   inGameIdDict[33902127].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(JUD_P)", inGameIdDict[33902129].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(RAP)",   inGameIdDict[33902130].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(FRE)",   inGameIdDict[33902133].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(PAT)",   inGameIdDict[33902136].GetString( useJapaneseNames ? 0 : 1 ) );
			s = s.Replace( "\x04(BAU)",   inGameIdDict[33902139].GetString( useJapaneseNames ? 0 : 1 ) );
			if ( inGameIdDict.ContainsKey( 33902199 ) ) { // not in 360
				s = s.Replace( "\x04(ALL)", inGameIdDict[33902199].GetString( useJapaneseNames ? 0 : 1 ) );
			}

			s = s.Replace( "\f", "\n\n" );
			s = Regex.Replace( s, "\t[(][A-Za-z0-9_]+[)]", "" ); // audio/voice commands

			if ( outputAsHtml ) {
				s = ReplaceFuriganaWithHtmlRuby( s );
				s = ReplaceColorCommandsWithSpansHtml( s );
			} else {
				s = RemoveFurigana( s, removeKanjiWithFurigana );
				s = Regex.Replace( s, "\x03[(][0-9]+[)]", "" ); // color commands
				s = Regex.Replace( s, "\x06[(]([A-Za-z0-9]+)[)]", "[Icon: $1]" );
			}

			s = Regex.Replace( s, "\x02[(]([0-9]+)[)]", "[Unknown: $1]" ); // unknown, is in some system strings
			s = Regex.Replace( s, "\x0B[(]([0-9]+)[)]", "[Parameter: $1]" );
			s = Regex.Replace( s, "\x01[(]([0-9]+)[,]([0-9]+)[)]", "[Metrics: $1, $2]" );

			return s;
		}

		public static string ReplaceColorCommandsWithSpansHtml( string s ) {
			bool currentlyDefaultColor = true;

			while ( s.Contains( '\x03' ) ) {
				int commandStart = s.IndexOf( '\x03' );
				string textFromCommandStart = s.Substring( commandStart );
				int commandLength = textFromCommandStart.IndexOf( ')' );
				string dataString = s.Substring( commandStart + 2, commandLength - 2 );

				string textPreCommand = s.Substring( 0, commandStart );
				string textPostCommand = s.Substring( commandStart + commandLength + 1 );

				// figure out which color to change to
				string colorClass = "";
				bool toDefaultColor = false;
				try {
					int colorId = Int32.Parse( dataString );
					switch ( colorId ) {
						case 0: toDefaultColor = true; break;
						case 2: colorClass = "textColorWhite"; break;
						case 4: colorClass = "textColorRed"; break;
						case 5: colorClass = "textColorGreen"; break;
						case 6: colorClass = "textColorBlue"; break;
						default: colorClass = "textColor" + colorId; break;
					}
				} catch ( System.FormatException ) {
					colorClass = "textColorIllegal";
				}

				// figure out if this menas we need to open and/or close span tags
				string colorSpan = "";
				if ( toDefaultColor ) {
					colorSpan = "</span>";
					currentlyDefaultColor = true;
				} else {
					if ( !currentlyDefaultColor ) {
						colorSpan = "</span>";
					}
					colorSpan += "<span class=\"" + colorClass + "\">";
					currentlyDefaultColor = false;
				}

				// and actually add the span tag
				s = textPreCommand + colorSpan + textPostCommand;
			}

			// if we're not the default at the end, close the open span tag
			if ( !currentlyDefaultColor ) {
				s += "</span>";
			}

			return s;
		}

		public static string RemoveFurigana( string str, bool keepFuriganaInstead ) {
			str = str.TrimEnd( '\r' ); // 360 skit files have this for some odd reason
			while ( str.Contains( '\r' ) ) {
				int furiStart = str.IndexOf( '\r' );
				string textFromFuriStart = str.Substring( furiStart );
				int furiEnd = furiStart + textFromFuriStart.IndexOf( ')' );
				string furiDataString = str.Substring( furiStart + 2, furiEnd - furiStart - 2 );

				string[] furiData = furiDataString.Split( ',' );
				int kanjiLength = Int32.Parse( furiData[0] );
				string furigana = furiData[1];

				string textPreFurigana = str.Substring( 0, furiStart - kanjiLength );
				string textKanji = str.Substring( furiStart - kanjiLength, kanjiLength );
				string textPostFurigana = str.Substring( furiEnd + 1 );

				if ( keepFuriganaInstead ) {
					str = textPreFurigana + furigana + textPostFurigana;
				} else {
					str = textPreFurigana + textKanji + textPostFurigana;
				}
			}
			return str;
		}
		public static string ReplaceFuriganaWithHtmlRuby( string str ) {
			str = str.TrimEnd( '\r' ); // 360 skit files have this for some odd reason
			while ( str.Contains( '\r' ) ) {
				int furiStart = str.IndexOf( '\r' );
				string textFromFuriStart = str.Substring( furiStart );
				int furiEnd = furiStart + textFromFuriStart.IndexOf( ')' );
				string furiDataString = str.Substring( furiStart + 2, furiEnd - furiStart - 2 );

				string[] furiData = furiDataString.Split( ',' );
				int kanjiLength = Int32.Parse( furiData[0] );
				string furigana = furiData[1];

				string textPreFurigana = str.Substring( 0, furiStart - kanjiLength );
				string textKanji = str.Substring( furiStart - kanjiLength, kanjiLength );
				string textPostFurigana = str.Substring( furiEnd + 1 );

				str =
					textPreFurigana + "<ruby>" + textKanji + "<rp>（</rp><rt>" +
					furigana + "</rt><rp>）</rp></ruby>" + textPostFurigana;

			}
			return str;
		}

		public static string ToHtmlJpn( this string str, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version ) {
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( str ), inGameIdDict, version, true ).ToString(), inGameIdDict, true, true ).Replace( "\n", "<br />" );
		}
		public static string ToHtmlEng( this string str, Dictionary<uint, TSS.TSSEntry> inGameIdDict, GameVersion version ) {
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( str ), inGameIdDict, version, false ).ToString(), inGameIdDict, false, true ).Replace( "\n", "<br />" );
		}

		public static String GetButtonName( GameVersion version, ControllerButton button ) {
			switch ( version ) {
				case GameVersion.X360_US:
				case GameVersion.X360_EU:
				case GameVersion.PC:
					switch ( button ) {
						case ControllerButton.Start: return version == GameVersion.PC ? "Menu" : "Start";
						case ControllerButton.Select: return version == GameVersion.PC ? "View" : "Back";
						case ControllerButton.RightButton: return "B";
						case ControllerButton.LowerButton: return "A";
						case ControllerButton.LeftButton: return "X";
						case ControllerButton.UpperButton: return "Y";
						case ControllerButton.L1: return "LB";
						case ControllerButton.L2: return "LT";
						case ControllerButton.L3: return "Push LS";
						case ControllerButton.R1: return "RB";
						case ControllerButton.R2: return "RT";
						case ControllerButton.R3: return "Push RS";
						case ControllerButton.LeftStick: return "Left Stick";
						case ControllerButton.RightStick: return "Right Stick";
						case ControllerButton.DPad: return "D-Pad";
						case ControllerButton.System: return "Guide";
						default: throw new Exception( "Unknown button " + button );
					}
				case GameVersion.PS3:
					switch ( button ) {
						case ControllerButton.Start: return "Start";
						case ControllerButton.Select: return "Select";
						case ControllerButton.RightButton: return "Circle";
						case ControllerButton.LowerButton: return "Cross";
						case ControllerButton.LeftButton: return "Square";
						case ControllerButton.UpperButton: return "Triangle";
						case ControllerButton.L1: return "L1";
						case ControllerButton.L2: return "L2";
						case ControllerButton.L3: return "L3";
						case ControllerButton.R1: return "R1";
						case ControllerButton.R2: return "R2";
						case ControllerButton.R3: return "R3";
						case ControllerButton.LeftStick: return "Left Stick";
						case ControllerButton.RightStick: return "Right Stick";
						case ControllerButton.DPad: return "D-Pad";
						case ControllerButton.System: return "PS";
						default: throw new Exception( "Unknown button " + button );
					}
				default:
					throw new Exception( "Unknown game version " + version );
			}
		}
	}
}
