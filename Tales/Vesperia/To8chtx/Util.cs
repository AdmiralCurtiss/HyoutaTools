using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.To8chtx {
	static class Util {
		public static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );

		public static UInt32 SwapEndian( UInt32 x ) {
			return x = ( x >> 24 ) |
					  ( ( x << 8 ) & 0x00FF0000 ) |
					  ( ( x >> 8 ) & 0x0000FF00 ) |
					   ( x << 24 );
		}

		public static UInt64 SwapEndian( UInt64 x ) {
			return x = ( x >> 56 ) |
						( ( x << 40 ) & 0x00FF000000000000 ) |
						( ( x << 24 ) & 0x0000FF0000000000 ) |
						( ( x << 8 ) & 0x000000FF00000000 ) |
						( ( x >> 8 ) & 0x00000000FF000000 ) |
						( ( x >> 24 ) & 0x0000000000FF0000 ) |
						( ( x >> 40 ) & 0x000000000000FF00 ) |
						 ( x << 56 );
		}



		public static byte[] StringToBytes( String s ) {
			//byte[] bytes = ShiftJISEncoding.GetBytes(s);
			//return bytes.TakeWhile(subject => subject != 0x00).ToArray();
			return ShiftJISEncoding.GetBytes( s );
		}

		public static void DisplayException( Exception e ) {
			Console.WriteLine( "Exception occurred:" );
			Console.WriteLine( e.Message );
		}
	}
}
