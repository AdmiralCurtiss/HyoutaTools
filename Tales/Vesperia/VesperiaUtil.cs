using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HyoutaTools.Tales.Vesperia {
	public enum GameVersion {
		None, X360, PS3
	}
	public enum ControllerButton {
		Start, Select, RightButton, LowerButton, LeftButton, UpperButton, L1, L2, L3, R1, R2, R3, LeftStick, RightStick, DPad, System
	}

	public class VesperiaUtil {
		public static String RemoveTags( String s, bool useJapaneseNames = false, bool replaceFuriganaWithHtmlRuby = false ) {
			s = s.Replace( "''", "'" );
			if ( useJapaneseNames ) {
				s = s.Replace( "\x04(YUR)", "ユーリ" );
				s = s.Replace( "\x04(EST)", "エステリーゼ" );
				s = s.Replace( "\x04(EST_P)", "エステル" );
				s = s.Replace( "\x04(KAR)", "カロル" );
				s = s.Replace( "\x04(RIT)", "リタ" );
				s = s.Replace( "\x04(RAV)", "レイヴン" );
				s = s.Replace( "\x04(JUD)", "ジュディス" );
				s = s.Replace( "\x04(JUD_P)", "ジュディ" );
				s = s.Replace( "\x04(RAP)", "ラピード" );
				s = s.Replace( "\x04(FRE)", "フレン" );
				s = s.Replace( "\x04(PAT)", "パティ" );
				s = s.Replace( "\x04(BAU)", "バウル" );
			} else {
				s = s.Replace( "\x04(YUR)", "Yuri" );
				s = s.Replace( "\x04(EST)", "Estellise" );
				s = s.Replace( "\x04(EST_P)", "Estelle" );
				s = s.Replace( "\x04(KAR)", "Karol" );
				s = s.Replace( "\x04(RIT)", "Rita" );
				s = s.Replace( "\x04(RAV)", "Raven" );
				s = s.Replace( "\x04(JUD)", "Judith" );
				s = s.Replace( "\x04(JUD_P)", "Judy" );
				s = s.Replace( "\x04(RAP)", "Repede" );
				s = s.Replace( "\x04(FRE)", "Flynn" );
				s = s.Replace( "\x04(PAT)", "Patty" );
				s = s.Replace( "\x04(BAU)", "Ba'ul" );
			}
			s = s.Replace( ""/*0xFF*/, "\n\n" );
			if ( replaceFuriganaWithHtmlRuby ) {
				s = ReplaceFuriganaWithHtmlRuby( s );
			}
			s = Regex.Replace( s, "\t[(][A-Za-z0-9_]+[)]", "" ); // audio/voice commands
			s = Regex.Replace( s, "\x03[(][0-9]+[)]", "" ); // color commands
			s = s.Replace( '‡', 'é' );
			s = s.Replace( '†', 'í' );
			return s;
		}

		public static string ReplaceFuriganaWithHtmlRuby( string str ) {
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

		private static String[][] InsaneNames = null;
		public static String ReplaceWithInsaneNames( String input ) {
			if ( String.IsNullOrEmpty( input ) ) return input;

			if ( InsaneNames == null ) {
				InsaneNames = new String[][] {
					new String[] { "Commandant Alexei", "Emperor Peony Wesker-Dumbledore" } ,
					new String[] { "commandant Alexei", "Emperor Peony Wesker-Dumbledore" } ,
					new String[] { "Commandant\nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "commandant\nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "Commandant \nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "commandant \nAlexei", "Emperor Peony\nWesker-Dumbledore" } ,
					new String[] { "Alexei", "Wesker-Dumbledore" } ,
					new String[] { "Zaude", "Hogwarts" } ,
					new String[] { "Ioder", "Yoda" } ,
					new String[] { "Repede", "MC Weihnachtsdog" } ,
					new String[] { "Tricorne", "Partyhut" } ,
					new String[] { "Technical Diver", "Beach Party Outfit" } ,
					new String[] { "Fireball", "Imperial Fireball" } ,
					new String[] { "fireball", "Imperial Fireball" } ,
					new String[] { "Fire ball", "Imperial Fireball" } ,
					new String[] { "Fire Ball", "Imperial Fireball" } ,
					new String[] { "fire ball", "Imperial Fireball" } ,
					new String[] { "fire Ball", "Imperial Fireball" } ,
					new String[] { "Thunder Blade", "Handymasten" } ,
					new String[] { "Traitor to Heaven", "Tipp dir den Kick" } ,
					new String[] { "Traitor To Heaven", "Tipp dir den Kick" } ,
					new String[] { "Zagi", "Sarkli" } ,
				};
			}

			foreach ( String[] s in InsaneNames ) {
				input = input.Replace( s[0], s[1] );
			}
			return input;
		}

		public static String GetButtonName( GameVersion version, ControllerButton button ) {
			switch ( version ) {
				case GameVersion.X360:
					switch ( button ) {
						case ControllerButton.Start: return "Start";
						case ControllerButton.Select: return "Back";
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
					}
					break;
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
					}
					break;
			}

			return "None";
		}
	}
}
