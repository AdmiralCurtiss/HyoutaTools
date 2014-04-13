using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.DestinyRemake {
	public static class ToDrUtil {
		public static String GetTextPseudoShiftJis( byte[] File, int Pointer ) {
			if ( Pointer == -1 ) return null;

			int i = Pointer;
			while ( i < File.Length && File[i] != 0x00 ) {
				i++;
			}
			String Text = GetStringPseudoShiftJis( File, Pointer, i - Pointer );
			return Text;
		}
		public static String GetStringPseudoShiftJis( byte[] Bytes, int index, int count ) {
			StringBuilder sb = new StringBuilder( count );
			byte[] bytearray = new byte[2];
			for ( int i = 0; i < count; ++i ) {
				byte b = Bytes[index + i];
				if ( b < 0x80 ) {
					sb.Append( (char)b );
				} else {
					i++;
					byte b2 = Bytes[index + i];
					ushort twoByteJp = (ushort)( b << 8 | b2 );
					sb.Append( GetCharPseudoShiftJis( twoByteJp ) );
					//ushort shiftJisEquivalent = twoByteJp;
					//if ( twoByteJp < 0x99FA ) {
					//    shiftJisEquivalent -= 0x1709; // hiragana
					//} else if ( twoByteJp < 0x9B00 ) {
					//    shiftJisEquivalent -= 0x16FF; // katakana
					//}

					//bytearray[0] = (byte)( shiftJisEquivalent >> 8 & 0xFF );
					//bytearray[1] = (byte)( shiftJisEquivalent & 0xFF );

					//char[] chars = ShiftJISEncoding.GetChars( bytearray, 0, 2 );
					//foreach ( char c in chars ) {
					//    sb.Append( c );
					//}
				}
			}
			return sb.ToString();

			//foreach ( char c in str ) {
			//    if ( (int)c < 0x8000 ) {
			//        b.Append( c );
			//    } else {
			//        b.Append( (int)c - 0x1709 );
			//    }
			//}		
		}
		private static Dictionary<ushort, char> PseudoShiftJisMap = null;
		public static char GetCharPseudoShiftJis( ushort character ) {
			if ( PseudoShiftJisMap == null ) {
				PseudoShiftJisMap = new Dictionary<ushort, char>();

				byte[] tbl = HyoutaTools.Properties.Resources.todr_char_table_without_header;
				byte[] shiftJisChar = new byte[2];
				ushort TodChar = 0x9940;
				for ( int i = 0; i < tbl.Length; i += 2 ) {
					shiftJisChar[1] = tbl[i];
					shiftJisChar[0] = tbl[i + 1];

					char[] chars = Util.ShiftJISEncoding.GetChars( shiftJisChar, 0, 2 );
					PseudoShiftJisMap.Add( TodChar, chars[0] );

					if ( TodChar == 0x99A0 ) { TodChar += 2; }

					TodChar++;
				}


				/*


			  //PseudoShiftJisMap.Add( 'ぁ', 0x011F );

			  for ( int i = 0; i < 0x4A; ++i ) { // あ to る
				  PseudoShiftJisMap.Add( (ushort)( 0x99A9 + i ), (char)( 'あ' + i ) );
			  }
			  PseudoShiftJisMap.Add( 0x9960, '？' );
			  PseudoShiftJisMap.Add( 0x9941, '！' );
			  PseudoShiftJisMap.Add( 0x994D, '－' );
			  PseudoShiftJisMap.Add( 0x9952, '２' );	 // '０'
			  PseudoShiftJisMap.Add( 0x99A5, '、' );

			  //PseudoShiftJisMap.Add( 'れ', 0x016A );
			  //PseudoShiftJisMap.Add( 'ろ', 0x016B );

			  //PseudoShiftJisMap.Add( 'ゎ', 0x016C );
			  //PseudoShiftJisMap.Add( 'わ', 0x016D );
			  //PseudoShiftJisMap.Add( 'ゐ', 0x016E );
			  //PseudoShiftJisMap.Add( 'ゑ', 0x016F );


			  //PseudoShiftJisMap.Add( 'を', 0x0170 );
			  //PseudoShiftJisMap.Add( 'ん', 0x0171 );


			  //PseudoShiftJisMap.Add( 'ァ', 0x0180 );
			  PseudoShiftJisMap.Add( 0x99FC, 'ア' );
			  //PseudoShiftJisMap.Add( 'ィ', 0x0182 );
			  //PseudoShiftJisMap.Add( 'イ', 0x0183 );

			  //PseudoShiftJisMap.Add( 'ゥ', 0x0184 );
			  //PseudoShiftJisMap.Add( 'ウ', 0x0185 );
			  //PseudoShiftJisMap.Add( 'ェ', 0x0186 );
			  //PseudoShiftJisMap.Add( 'エ', 0x0187 );

			  //PseudoShiftJisMap.Add( 'ォ', 0x0188 );
			  //PseudoShiftJisMap.Add( 'オ', 0x0189 );
			  //PseudoShiftJisMap.Add( 'カ', 0x018A );
			  //PseudoShiftJisMap.Add( 'ガ', 0x018B );

			  //PseudoShiftJisMap.Add( 'キ', 0x018C );
			  //PseudoShiftJisMap.Add( 'ギ', 0x018D );
			  //PseudoShiftJisMap.Add( 'ク', 0x018E );
			  //PseudoShiftJisMap.Add( 'グ', 0x018F );


			  //PseudoShiftJisMap.Add( 'ケ', 0x0190 );
			  //PseudoShiftJisMap.Add( 'ゲ', 0x0191 );
			  //PseudoShiftJisMap.Add( 'コ', 0x0192 );
			  //PseudoShiftJisMap.Add( 'ゴ', 0x0193 );

			  //PseudoShiftJisMap.Add( 'サ', 0x0194 );
			  //PseudoShiftJisMap.Add( 'ザ', 0x0195 );
			  //PseudoShiftJisMap.Add( 'シ', 0x0196 );
			  PseudoShiftJisMap.Add( 0x9A55, 'ジ' );

			  //PseudoShiftJisMap.Add( 'ス', 0x0198 );
			  //PseudoShiftJisMap.Add( 'ズ', 0x0199 );
			  //PseudoShiftJisMap.Add( 'セ', 0x019A );
			  //PseudoShiftJisMap.Add( 'ゼ', 0x019B );

			  //PseudoShiftJisMap.Add( 'ソ', 0x019C );
			  //PseudoShiftJisMap.Add( 'ゾ', 0x019D );
			  //PseudoShiftJisMap.Add( 'タ', 0x019E );
			  //PseudoShiftJisMap.Add( 'ダ', 0x019F );


			  //PseudoShiftJisMap.Add( 'チ', 0x01A0 );
			  //PseudoShiftJisMap.Add( 'ヂ', 0x01A1 );
			  //PseudoShiftJisMap.Add( 'ッ', 0x01A2 );
			  //PseudoShiftJisMap.Add( 'ツ', 0x01A3 );

			  //PseudoShiftJisMap.Add( 'ヅ', 0x01A4 );
			  //PseudoShiftJisMap.Add( 'テ', 0x01A5 );
			  //PseudoShiftJisMap.Add( 'デ', 0x01A6 );
			  //PseudoShiftJisMap.Add( 'ト', 0x01A7 );

			  //PseudoShiftJisMap.Add( 'ド', 0x01A8 );
			  //PseudoShiftJisMap.Add( 'ナ', 0x01A9 );
			  //PseudoShiftJisMap.Add( 'ニ', 0x01AA );
			  //PseudoShiftJisMap.Add( 'ヌ', 0x01AB );

			  //PseudoShiftJisMap.Add( 'ネ', 0x01AC );
			  PseudoShiftJisMap.Add( 0x9A6C, 'ノ' );
			  // interpolate
			  PseudoShiftJisMap.Add( 0x9A72, 'ピ' );

			  //PseudoShiftJisMap.Add( 'フ', 0x01B4 );
			  //PseudoShiftJisMap.Add( 'ブ', 0x01B5 );
			  //PseudoShiftJisMap.Add( 'プ', 0x01B6 );
			  //PseudoShiftJisMap.Add( 'ヘ', 0x01B7 );

			  //PseudoShiftJisMap.Add( 'ベ', 0x01B8 );
			  //PseudoShiftJisMap.Add( 'ペ', 0x01B9 );
			  //PseudoShiftJisMap.Add( 'ホ', 0x01BA );
			  //PseudoShiftJisMap.Add( 'ボ', 0x01BB );

			  //PseudoShiftJisMap.Add( 'ポ', 0x01BC );
			  //PseudoShiftJisMap.Add( 'マ', 0x01BD );
			  //PseudoShiftJisMap.Add( 'ミ', 0x01BE );


			  //PseudoShiftJisMap.Add( 'ム', 0x01C0 );
			  //PseudoShiftJisMap.Add( 'メ', 0x01C1 );
			  //PseudoShiftJisMap.Add( 'モ', 0x01C2 );
			  //PseudoShiftJisMap.Add( 'ャ', 0x01C3 );

			  //PseudoShiftJisMap.Add( 'ヤ', 0x01C4 );
			  //PseudoShiftJisMap.Add( 'ュ', 0x01C5 );
			  //PseudoShiftJisMap.Add( 'ユ', 0x01C6 );
			  //PseudoShiftJisMap.Add( 'ョ', 0x01C7 );

			  //PseudoShiftJisMap.Add( 'ヨ', 0x01C8 );
			  //PseudoShiftJisMap.Add( 'ラ', 0x01C9 );
			  //PseudoShiftJisMap.Add( 'リ', 0x01CA );
			  //PseudoShiftJisMap.Add( 'ル', 0x01CB );

			  PseudoShiftJisMap.Add( 0x9A8B, 'レ' );
			  //PseudoShiftJisMap.Add( 'ロ', 0x01CD );
			  //PseudoShiftJisMap.Add( 'ヮ', 0x01CE );
			  //PseudoShiftJisMap.Add( 'ワ', 0x01CF );


			  //PseudoShiftJisMap.Add( 'ヰ', 0x01D0 );
			  //PseudoShiftJisMap.Add( 'ヱ', 0x01D1 );
			  //PseudoShiftJisMap.Add( 'ヲ', 0x01D2 );
			  PseudoShiftJisMap.Add( 0x9A92, 'ン' );

			  //PseudoShiftJisMap.Add( 'ヴ', 0x01D4 );
			  //PseudoShiftJisMap.Add( 'ヵ', 0x01D5 );
			  //PseudoShiftJisMap.Add( 'ヶ', 0x01D6 );


			  // Kanji
			  //PseudoShiftJisMap.Add( 0x9C73, '易' );
			  PseudoShiftJisMap.Add( 0x9C74, '易' );
			  //PseudoShiftJisMap.Add( 0x9C75, '易' );
			  PseudoShiftJisMap.Add( 0xE15D, '高' );
			  PseudoShiftJisMap.Add( 0xE997, '夢' );

			  // 率
			  */
			}
			return PseudoShiftJisMap[character];
		}
	}
}
