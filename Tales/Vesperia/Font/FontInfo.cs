using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.Font {
	public class FontInfo {
		public int[] CharacterLengths;
		private Dictionary<char, int> CharacterMap;


		public FontInfo( byte[] File, int Offset ) {
			CharacterLengths = new int[0x220];

			for ( int i = 0; i < 0x220; i++ ) {
				CharacterLengths[i] = Util.SwapEndian( BitConverter.ToInt32( File, Offset + i * 4 ) );
			}

			BuildCharacterMap();
		}

		public void WriteCharacterLengths( String Filename, int Offset ) {
			byte[] File = System.IO.File.ReadAllBytes( Filename );

			for ( int i = 0; i < CharacterLengths.Length; i++ ) {
				int c = Util.SwapEndian( CharacterLengths[i] );
				byte[] b = BitConverter.GetBytes( c );
				b.CopyTo( File, Offset + i * 4 );
			}

			System.IO.File.WriteAllBytes( Filename, File );
		}

		public List<char> tmp = null;

		public int GetCharacterIdFromCharacter( char c ) {
			try {
				return CharacterMap[c];
			} catch ( Exception ) {
				if ( tmp == null ) tmp = new List<char>();
				tmp.Add( c );
				return CharacterMap[' '];
			}
		}

		private void BuildCharacterMap() {
			CharacterMap = new Dictionary<char, int>( 0x2000 );

			CharacterMap.Add( ' ', 0x0000 );
			CharacterMap.Add( '　', 0x0000 );
			CharacterMap.Add( '、', 0x0001 );
			CharacterMap.Add( '。', 0x0002 );
			CharacterMap.Add( ',', 0x0003 );
			CharacterMap.Add( '.', 0x0004 );

			CharacterMap.Add( ':', 0x0006 );
			CharacterMap.Add( ';', 0x0007 );
			CharacterMap.Add( '?', 0x0008 );
			CharacterMap.Add( '？', 0x0008 );
			CharacterMap.Add( '!', 0x0009 );
			CharacterMap.Add( '！', 0x0009 );

			CharacterMap.Add( '゛', 0x0009 );
			CharacterMap.Add( '゜', 0x000A );
			CharacterMap.Add( 'ゝ', 0x0014 );
			CharacterMap.Add( 'ゞ', 0x0015 );

			CharacterMap.Add( '・', 0x0005 );
			CharacterMap.Add( 'ー', 0x001C );
			CharacterMap.Add( 'ヽ', 0x0012 );
			CharacterMap.Add( 'ヾ', 0x0013 );

			CharacterMap.Add( '_', 0x0011 );
			CharacterMap.Add( '々', 0x0018 );
			CharacterMap.Add( '○', 0x001A );
			CharacterMap.Add( '/', 0x001E );
			CharacterMap.Add( '\\', 0x001F );

			CharacterMap.Add( '~', 0x0020 );
			CharacterMap.Add( '～', 0x0020 );
			CharacterMap.Add( '|', 0x0022 );
			CharacterMap.Add( '…', 0x0023 );
			CharacterMap.Add( '\'', 0x0026 );
			CharacterMap.Add( '’', 0x0026 );
			CharacterMap.Add( '\"', 0x0028 );
			CharacterMap.Add( '“', 0x0027 );
			CharacterMap.Add( '”', 0x0028 );
			CharacterMap.Add( '(', 0x0029 );
			CharacterMap.Add( '（', 0x0029 );
			CharacterMap.Add( ')', 0x002A );
			CharacterMap.Add( '）', 0x002A );
			CharacterMap.Add( '[', 0x002D );
			CharacterMap.Add( ']', 0x002E );
			CharacterMap.Add( '{', 0x002F );
			CharacterMap.Add( '}', 0x0030 );

			CharacterMap.Add( '『', 0x0037 );
			CharacterMap.Add( '』', 0x0038 );

			CharacterMap.Add( '+', 0x003B );
			CharacterMap.Add( '＋', 0x003B );
			CharacterMap.Add( '-', 0x003C );

			CharacterMap.Add( '=', 0x0041 );
			CharacterMap.Add( '<', 0x0043 );
			CharacterMap.Add( '>', 0x0044 );

			CharacterMap.Add( '%', 0x0053 );
			CharacterMap.Add( '#', 0x0054 );
			CharacterMap.Add( '&', 0x0055 );
			CharacterMap.Add( '*', 0x0056 );

			CharacterMap.Add( '☆', 0x0059 );

			CharacterMap.Add( '∀', 0x008D );

			CharacterMap.Add( '♪', 0x00B4 );

			CharacterMap.Add( '0', 0x00CF );
			CharacterMap.Add( '1', 0x00D0 );
			CharacterMap.Add( '2', 0x00D1 );
			CharacterMap.Add( '3', 0x00D2 );
			CharacterMap.Add( '4', 0x00D3 );
			CharacterMap.Add( '5', 0x00D4 );
			CharacterMap.Add( '6', 0x00D5 );
			CharacterMap.Add( '7', 0x00D6 );
			CharacterMap.Add( '8', 0x00D7 );
			CharacterMap.Add( '9', 0x00D8 );
			CharacterMap.Add( '０', 0x00CF );
			CharacterMap.Add( '１', 0x00D0 );
			CharacterMap.Add( '２', 0x00D1 );
			CharacterMap.Add( '３', 0x00D2 );
			//CharacterMap.Add('4', 0x00D3);
			CharacterMap.Add( '５', 0x00D4 );
			//CharacterMap.Add('6', 0x00D5);
			//CharacterMap.Add('7', 0x00D6);
			CharacterMap.Add( '８', 0x00D7 );
			//CharacterMap.Add('9', 0x00D8);

			CharacterMap.Add( 'A', 0x00E0 );
			CharacterMap.Add( 'B', 0x00E1 );
			CharacterMap.Add( 'C', 0x00E2 );
			CharacterMap.Add( 'D', 0x00E3 );
			CharacterMap.Add( 'E', 0x00E4 );
			CharacterMap.Add( 'F', 0x00E5 );
			CharacterMap.Add( 'G', 0x00E6 );
			CharacterMap.Add( 'H', 0x00E7 );
			CharacterMap.Add( 'I', 0x00E8 );
			CharacterMap.Add( 'J', 0x00E9 );
			CharacterMap.Add( 'K', 0x00EA );
			CharacterMap.Add( 'L', 0x00EB );
			CharacterMap.Add( 'M', 0x00EC );
			CharacterMap.Add( 'N', 0x00ED );
			CharacterMap.Add( 'O', 0x00EE );
			CharacterMap.Add( 'P', 0x00EF );
			CharacterMap.Add( 'Q', 0x00F0 );
			CharacterMap.Add( 'R', 0x00F1 );
			CharacterMap.Add( 'S', 0x00F2 );
			CharacterMap.Add( 'T', 0x00F3 );
			CharacterMap.Add( 'U', 0x00F4 );
			CharacterMap.Add( 'V', 0x00F5 );
			CharacterMap.Add( 'W', 0x00F6 );
			CharacterMap.Add( 'X', 0x00F7 );
			CharacterMap.Add( 'Y', 0x00F8 );
			CharacterMap.Add( 'Z', 0x00F9 );

			CharacterMap.Add( 'a', 0x0101 );
			CharacterMap.Add( 'b', 0x0102 );
			CharacterMap.Add( 'c', 0x0103 );
			CharacterMap.Add( 'd', 0x0104 );
			CharacterMap.Add( 'e', 0x0105 );
			CharacterMap.Add( 'f', 0x0106 );
			CharacterMap.Add( 'g', 0x0107 );
			CharacterMap.Add( 'h', 0x0108 );
			CharacterMap.Add( 'i', 0x0109 );
			CharacterMap.Add( 'j', 0x010A );
			CharacterMap.Add( 'k', 0x010B );
			CharacterMap.Add( 'l', 0x010C );
			CharacterMap.Add( 'm', 0x010D );
			CharacterMap.Add( 'n', 0x010E );
			CharacterMap.Add( 'o', 0x010F );
			CharacterMap.Add( 'p', 0x0110 );
			CharacterMap.Add( 'q', 0x0111 );
			CharacterMap.Add( 'r', 0x0112 );
			CharacterMap.Add( 's', 0x0113 );
			CharacterMap.Add( 't', 0x0114 );
			CharacterMap.Add( 'u', 0x0115 );
			CharacterMap.Add( 'v', 0x0116 );
			CharacterMap.Add( 'w', 0x0117 );
			CharacterMap.Add( 'x', 0x0118 );
			CharacterMap.Add( 'y', 0x0119 );
			CharacterMap.Add( 'z', 0x011A );

			CharacterMap.Add( 'Α', 0x01DF );
			CharacterMap.Add( 'Β', 0x01E0 );
			CharacterMap.Add( 'Γ', 0x01E1 );
			CharacterMap.Add( 'Δ', 0x01E2 );
			CharacterMap.Add( 'Ε', 0x01E3 );
			CharacterMap.Add( 'Ζ', 0x01E4 );
			CharacterMap.Add( 'Η', 0x01E5 );
			CharacterMap.Add( 'Θ', 0x01E6 );
			CharacterMap.Add( 'Ι', 0x01E7 );
			CharacterMap.Add( 'Κ', 0x01E8 );
			CharacterMap.Add( 'Λ', 0x01E9 );
			CharacterMap.Add( 'Μ', 0x01EA );
			CharacterMap.Add( 'Ν', 0x01EB );
			CharacterMap.Add( 'Ξ', 0x01EC );
			CharacterMap.Add( 'Ο', 0x01ED );
			CharacterMap.Add( 'Π', 0x01EE );
			CharacterMap.Add( 'Ρ', 0x01EF );
			CharacterMap.Add( 'Σ', 0x01F0 );
			CharacterMap.Add( 'Τ', 0x01F1 );
			CharacterMap.Add( 'Υ', 0x01F2 );
			CharacterMap.Add( 'Φ', 0x01F3 );
			CharacterMap.Add( 'Χ', 0x01F4 );
			CharacterMap.Add( 'Ψ', 0x01F5 );
			CharacterMap.Add( 'Ω', 0x01F6 );

			CharacterMap.Add( 'α', 0x01FF );
			CharacterMap.Add( 'β', 0x0200 );
			CharacterMap.Add( 'γ', 0x0201 );
			CharacterMap.Add( 'δ', 0x0202 );
			CharacterMap.Add( 'ε', 0x0203 );
			CharacterMap.Add( 'ζ', 0x0204 );
			CharacterMap.Add( 'η', 0x0205 );
			CharacterMap.Add( 'θ', 0x0206 );
			CharacterMap.Add( 'ι', 0x0207 );
			CharacterMap.Add( 'κ', 0x0208 );
			CharacterMap.Add( 'λ', 0x0209 );
			CharacterMap.Add( 'μ', 0x020A );
			CharacterMap.Add( 'ν', 0x020B );
			CharacterMap.Add( 'ξ', 0x020C );
			CharacterMap.Add( 'ο', 0x020D );
			CharacterMap.Add( 'π', 0x020E );
			CharacterMap.Add( 'ρ', 0x020F );
			CharacterMap.Add( 'σ', 0x0210 );
			CharacterMap.Add( 'τ', 0x0211 );
			CharacterMap.Add( 'υ', 0x0212 );
			CharacterMap.Add( 'φ', 0x0213 );
			CharacterMap.Add( 'χ', 0x0214 );
			CharacterMap.Add( 'ψ', 0x0215 );
			CharacterMap.Add( 'ω', 0x0216 );

			CharacterMap.Add( 'ぁ', 0x011F );

			CharacterMap.Add( 'あ', 0x0120 );
			CharacterMap.Add( 'ぃ', 0x0121 );
			CharacterMap.Add( 'い', 0x0122 );
			CharacterMap.Add( 'ぅ', 0x0123 );

			CharacterMap.Add( 'う', 0x0124 );
			CharacterMap.Add( 'ぇ', 0x0125 );
			CharacterMap.Add( 'え', 0x0126 );
			CharacterMap.Add( 'ぉ', 0x0127 );

			CharacterMap.Add( 'お', 0x0128 );
			CharacterMap.Add( 'か', 0x0129 );
			CharacterMap.Add( 'が', 0x012A );
			CharacterMap.Add( 'き', 0x012B );

			CharacterMap.Add( 'ぎ', 0x012C );
			CharacterMap.Add( 'く', 0x012D );
			CharacterMap.Add( 'ぐ', 0x012E );
			CharacterMap.Add( 'け', 0x012F );

			CharacterMap.Add( 'げ', 0x0130 );
			CharacterMap.Add( 'こ', 0x0131 );
			CharacterMap.Add( 'ご', 0x0132 );
			CharacterMap.Add( 'さ', 0x0133 );

			CharacterMap.Add( 'ざ', 0x0134 );
			CharacterMap.Add( 'し', 0x0135 );
			CharacterMap.Add( 'じ', 0x0136 );
			CharacterMap.Add( 'す', 0x0137 );

			CharacterMap.Add( 'ず', 0x0138 );
			CharacterMap.Add( 'せ', 0x0139 );
			CharacterMap.Add( 'ぜ', 0x013A );
			CharacterMap.Add( 'そ', 0x013B );

			CharacterMap.Add( 'ぞ', 0x013C );
			CharacterMap.Add( 'た', 0x013D );
			CharacterMap.Add( 'だ', 0x013E );
			CharacterMap.Add( 'ち', 0x013F );


			CharacterMap.Add( 'ぢ', 0x0140 );
			CharacterMap.Add( 'っ', 0x0141 );
			CharacterMap.Add( 'つ', 0x0142 );
			CharacterMap.Add( 'づ', 0x0143 );

			CharacterMap.Add( 'て', 0x0144 );
			CharacterMap.Add( 'で', 0x0145 );
			CharacterMap.Add( 'と', 0x0146 );
			CharacterMap.Add( 'ど', 0x0147 );

			CharacterMap.Add( 'な', 0x0148 );
			CharacterMap.Add( 'に', 0x0149 );
			CharacterMap.Add( 'ぬ', 0x014A );
			CharacterMap.Add( 'ね', 0x014B );

			CharacterMap.Add( 'の', 0x014C );
			CharacterMap.Add( 'は', 0x014D );
			CharacterMap.Add( 'ば', 0x014E );
			CharacterMap.Add( 'ぱ', 0x014F );


			CharacterMap.Add( 'ひ', 0x0150 );
			CharacterMap.Add( 'び', 0x0151 );
			CharacterMap.Add( 'ぴ', 0x0152 );
			CharacterMap.Add( 'ふ', 0x0153 );

			CharacterMap.Add( 'ぶ', 0x0154 );
			CharacterMap.Add( 'ぷ', 0x0155 );
			CharacterMap.Add( 'へ', 0x0156 );
			CharacterMap.Add( 'べ', 0x0157 );

			CharacterMap.Add( 'ぺ', 0x0158 );
			CharacterMap.Add( 'ほ', 0x0159 );
			CharacterMap.Add( 'ぼ', 0x015A );
			CharacterMap.Add( 'ぽ', 0x015B );

			CharacterMap.Add( 'ま', 0x015C );
			CharacterMap.Add( 'み', 0x015D );
			CharacterMap.Add( 'む', 0x015E );
			CharacterMap.Add( 'め', 0x015F );


			CharacterMap.Add( 'も', 0x0160 );
			CharacterMap.Add( 'ゃ', 0x0161 );
			CharacterMap.Add( 'や', 0x0162 );
			CharacterMap.Add( 'ゅ', 0x0163 );

			CharacterMap.Add( 'ゆ', 0x0164 );
			CharacterMap.Add( 'ょ', 0x0165 );
			CharacterMap.Add( 'よ', 0x0166 );
			CharacterMap.Add( 'ら', 0x0167 );

			CharacterMap.Add( 'り', 0x0168 );
			CharacterMap.Add( 'る', 0x0169 );
			CharacterMap.Add( 'れ', 0x016A );
			CharacterMap.Add( 'ろ', 0x016B );

			CharacterMap.Add( 'ゎ', 0x016C );
			CharacterMap.Add( 'わ', 0x016D );
			CharacterMap.Add( 'ゐ', 0x016E );
			CharacterMap.Add( 'ゑ', 0x016F );


			CharacterMap.Add( 'を', 0x0170 );
			CharacterMap.Add( 'ん', 0x0171 );


			CharacterMap.Add( 'ァ', 0x0180 );
			CharacterMap.Add( 'ア', 0x0181 );
			CharacterMap.Add( 'ィ', 0x0182 );
			CharacterMap.Add( 'イ', 0x0183 );

			CharacterMap.Add( 'ゥ', 0x0184 );
			CharacterMap.Add( 'ウ', 0x0185 );
			CharacterMap.Add( 'ェ', 0x0186 );
			CharacterMap.Add( 'エ', 0x0187 );

			CharacterMap.Add( 'ォ', 0x0188 );
			CharacterMap.Add( 'オ', 0x0189 );
			CharacterMap.Add( 'カ', 0x018A );
			CharacterMap.Add( 'ガ', 0x018B );

			CharacterMap.Add( 'キ', 0x018C );
			CharacterMap.Add( 'ギ', 0x018D );
			CharacterMap.Add( 'ク', 0x018E );
			CharacterMap.Add( 'グ', 0x018F );


			CharacterMap.Add( 'ケ', 0x0190 );
			CharacterMap.Add( 'ゲ', 0x0191 );
			CharacterMap.Add( 'コ', 0x0192 );
			CharacterMap.Add( 'ゴ', 0x0193 );

			CharacterMap.Add( 'サ', 0x0194 );
			CharacterMap.Add( 'ザ', 0x0195 );
			CharacterMap.Add( 'シ', 0x0196 );
			CharacterMap.Add( 'ジ', 0x0197 );

			CharacterMap.Add( 'ス', 0x0198 );
			CharacterMap.Add( 'ズ', 0x0199 );
			CharacterMap.Add( 'セ', 0x019A );
			CharacterMap.Add( 'ゼ', 0x019B );

			CharacterMap.Add( 'ソ', 0x019C );
			CharacterMap.Add( 'ゾ', 0x019D );
			CharacterMap.Add( 'タ', 0x019E );
			CharacterMap.Add( 'ダ', 0x019F );


			CharacterMap.Add( 'チ', 0x01A0 );
			CharacterMap.Add( 'ヂ', 0x01A1 );
			CharacterMap.Add( 'ッ', 0x01A2 );
			CharacterMap.Add( 'ツ', 0x01A3 );

			CharacterMap.Add( 'ヅ', 0x01A4 );
			CharacterMap.Add( 'テ', 0x01A5 );
			CharacterMap.Add( 'デ', 0x01A6 );
			CharacterMap.Add( 'ト', 0x01A7 );

			CharacterMap.Add( 'ド', 0x01A8 );
			CharacterMap.Add( 'ナ', 0x01A9 );
			CharacterMap.Add( 'ニ', 0x01AA );
			CharacterMap.Add( 'ヌ', 0x01AB );

			CharacterMap.Add( 'ネ', 0x01AC );
			CharacterMap.Add( 'ノ', 0x01AD );
			CharacterMap.Add( 'ハ', 0x01AE );
			CharacterMap.Add( 'バ', 0x01AF );


			CharacterMap.Add( 'パ', 0x01B0 );
			CharacterMap.Add( 'ヒ', 0x01B1 );
			CharacterMap.Add( 'ビ', 0x01B2 );
			CharacterMap.Add( 'ピ', 0x01B3 );

			CharacterMap.Add( 'フ', 0x01B4 );
			CharacterMap.Add( 'ブ', 0x01B5 );
			CharacterMap.Add( 'プ', 0x01B6 );
			CharacterMap.Add( 'ヘ', 0x01B7 );

			CharacterMap.Add( 'ベ', 0x01B8 );
			CharacterMap.Add( 'ペ', 0x01B9 );
			CharacterMap.Add( 'ホ', 0x01BA );
			CharacterMap.Add( 'ボ', 0x01BB );

			CharacterMap.Add( 'ポ', 0x01BC );
			CharacterMap.Add( 'マ', 0x01BD );
			CharacterMap.Add( 'ミ', 0x01BE );


			CharacterMap.Add( 'ム', 0x01C0 );
			CharacterMap.Add( 'メ', 0x01C1 );
			CharacterMap.Add( 'モ', 0x01C2 );
			CharacterMap.Add( 'ャ', 0x01C3 );

			CharacterMap.Add( 'ヤ', 0x01C4 );
			CharacterMap.Add( 'ュ', 0x01C5 );
			CharacterMap.Add( 'ユ', 0x01C6 );
			CharacterMap.Add( 'ョ', 0x01C7 );

			CharacterMap.Add( 'ヨ', 0x01C8 );
			CharacterMap.Add( 'ラ', 0x01C9 );
			CharacterMap.Add( 'リ', 0x01CA );
			CharacterMap.Add( 'ル', 0x01CB );

			CharacterMap.Add( 'レ', 0x01CC );
			CharacterMap.Add( 'ロ', 0x01CD );
			CharacterMap.Add( 'ヮ', 0x01CE );
			CharacterMap.Add( 'ワ', 0x01CF );


			CharacterMap.Add( 'ヰ', 0x01D0 );
			CharacterMap.Add( 'ヱ', 0x01D1 );
			CharacterMap.Add( 'ヲ', 0x01D2 );
			CharacterMap.Add( 'ン', 0x01D3 );

			CharacterMap.Add( 'ヴ', 0x01D4 );
			CharacterMap.Add( 'ヵ', 0x01D5 );
			CharacterMap.Add( 'ヶ', 0x01D6 );




		}

	}
}
