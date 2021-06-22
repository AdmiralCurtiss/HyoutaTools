using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using HyoutaUtils;

namespace HyoutaTools.Other.LuxPain {
	static class LuxPainUtil {
		public static String GetTextLuxPain( int Pointer, byte[] File ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;

				StringBuilder Text = new StringBuilder();
				int CommandParametersLeft = 0;

				while ( true ) {
					// who wrote this ingame text parsing algo? uugh
					if ( CommandParametersLeft <= 0 && File[i] == 0x00 && File[i + 1] == 0x00 ) { // 0000 indicates end of text, except when a command with parameters is active
						break;
					}

					if ( CommandParametersLeft <= 0 && File[i] == 0xFF ) { //0xFFxx == command, command may have arguments
						switch ( File[i + 1] ) {
							case 0x02: CommandParametersLeft = 1; break;
							case 0x04: CommandParametersLeft = 1; break;
							case 0x06: CommandParametersLeft = 1; break;
							case 0x07: CommandParametersLeft = 2; break;
							case 0x0a: CommandParametersLeft = 1; break;
							case 0x0e: CommandParametersLeft = 2; break;
							case 0x0f: CommandParametersLeft = 2; break;
							case 0x10: CommandParametersLeft = 2; break;
						}
					} else {
						--CommandParametersLeft;
					}

					String tmp;
					if ( File[i] < 0x80 || File[i] >= 0xF0 ) {
						tmp = string.Format( "<{0:x2}{1:x2}>", File[i], File[i + 1] );
					} else {
						tmp = TextUtils.ShiftJISEncoding.GetString( File, i, 2 );
					}
					Text.Append( tmp );
					i += 2;
				}
				return Text.ToString();
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				return null;
			}
		}

		public static String FormatForEditing( String Text ) {
			for ( int i = 0; i < 26; ++i ) {
				Text = Text.Replace( (char)( 'Ａ' + i ), (char)( 'A' + i ) );
				Text = Text.Replace( (char)( 'ａ' + i ), (char)( 'a' + i ) );
			}
			for ( int i = 0; i < 10; ++i ) {
				Text = Text.Replace( (char)( '０' + i ), (char)( '0' + i ) );
			}

			Text = Text.Replace( '　', ' ' );
			Text = Text.Replace( '！', '!' );
			Text = Text.Replace( '．', '.' );
			Text = Text.Replace( '？', '?' );
			Text = Text.Replace( '’', '\'' );
			Text = Text.Replace( '，', ',' );
			Text = Text.Replace( '”', '"' );
			Text = Text.Replace( '‐', '-' );
			Text = Text.Replace( '：', ':' );
			Text = Text.Replace( '；', ';' );
			Text = Text.Replace( '（', '(' );
			Text = Text.Replace( '）', ')' );
			Text = Text.Replace( "<ff00>", "\n" );

			return Text;
		}

		public static byte[] BackConvertLuxPainText( String Text ) {
			String tmp = Text;
			List<byte> bytes = new List<byte>( tmp.Length * 2 + 512 );

			while ( tmp.Length > 0 ) {
				if ( tmp.StartsWith( "<" ) ) {
					int cnt = tmp.IndexOf( '>' );
					String s = tmp.Substring( 1, cnt - 1 );
					tmp = tmp.Substring( cnt + 1 );

					byte[] b = HexUtils.HumanReadableHexStringToByteArray( FormatForEditing( s ) );
					bytes.AddRange( b );
				} else {
					String s = tmp.Substring( 0, 1 );
					tmp = tmp.Substring( 1 );
					byte[] b = TextUtils.ShiftJISEncoding.GetBytes( s );
					bytes.AddRange( b );
				}
			}

			bytes.Add( 0x00 );
			bytes.Add( 0x00 );

			return bytes.ToArray();
		}
	}
}
