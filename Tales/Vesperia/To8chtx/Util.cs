using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.To8chtx {
	static class Util {
		public static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );

		public static byte[] StringToBytes( String s ) {
			//byte[] bytes = ShiftJISEncoding.GetBytes(s);
			//return bytes.TakeWhile(subject => subject != 0x00).ToArray();
			return ShiftJISEncoding.GetBytes( s );
		}
	}
}
