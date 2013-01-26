using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools {
	class Util {

		#region SwapEndian
		public static Int16 SwapEndian( Int16 x ) {
			return (Int16)SwapEndian( (UInt16)x );
		}
		public static UInt16 SwapEndian( UInt16 x ) {
			return x = (UInt16)
					   ( ( x << 8 ) |
						( x >> 8 ) );
		}

		public static Int32 SwapEndian( Int32 x ) {
			return (Int32)SwapEndian( (UInt32)x );
		}
		public static UInt32 SwapEndian( UInt32 x ) {
			return x = ( x << 24 ) |
					  ( ( x << 8 ) & 0x00FF0000 ) |
					  ( ( x >> 8 ) & 0x0000FF00 ) |
					   ( x >> 24 );
		}

		public static Int64 SwapEndian( Int64 x ) {
			return (Int64)SwapEndian( (UInt64)x );
		}
		public static UInt64 SwapEndian( UInt64 x ) {
			return x = ( x << 56 ) |
						( ( x << 40 ) & 0x00FF000000000000 ) |
						( ( x << 24 ) & 0x0000FF0000000000 ) |
						( ( x << 8 ) & 0x000000FF00000000 ) |
						( ( x >> 8 ) & 0x00000000FF000000 ) |
						( ( x >> 24 ) & 0x0000000000FF0000 ) |
						( ( x >> 40 ) & 0x000000000000FF00 ) |
						 ( x >> 56 );
		}
		#endregion

		public static void DisplayException( Exception e ) {
			Console.WriteLine( "Exception occurred:" );
			Console.WriteLine( e.Message );
		}

		#region TextUtils
		public static Encoding ShiftJISEncoding = Encoding.GetEncoding( "shift-jis" );
		public static String GetTextShiftJis( byte[] File, int Pointer ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;
				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = ShiftJISEncoding.GetString( File, Pointer, i - Pointer );
				return Text;
			} catch ( Exception ) {
				return null;
			}
		}
		public static String GetTextAscii( byte[] File, int Pointer ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;
				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = Encoding.ASCII.GetString( File, Pointer, i - Pointer );
				return Text;
			} catch ( Exception ) {
				return null;
			}
		}
		#endregion
	}
}
