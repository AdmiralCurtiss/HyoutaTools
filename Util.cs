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

		#region NumberUtils
		public static byte ParseDecOrHexToByte( string s ) {
			s = s.Trim();

			if ( s.StartsWith( "0x" ) ) {
				s = s.Substring( 2 );
				return Byte.Parse( s, System.Globalization.NumberStyles.HexNumber );
			} else {
				return Byte.Parse( s );
			}
		}

		public static byte[] HexStringToByteArray( string hex ) {
			if ( hex.Length % 2 == 1 )
				throw new Exception( "The binary key cannot have an odd number of digits" );

			byte[] arr = new byte[hex.Length >> 1];

			for ( int i = 0; i < hex.Length >> 1; ++i ) {
				arr[i] = (byte)( ( GetHexVal( hex[i << 1] ) << 4 ) + ( GetHexVal( hex[( i << 1 ) + 1] ) ) );
			}

			return arr;
		}

		public static int GetHexVal( char hex ) {
			int val = (int)hex;
			//For uppercase A-F letters:
			//return val - (val < 58 ? 48 : 55);
			//For lowercase a-f letters:
			//return val - (val < 58 ? 48 : 87);
			//Or the two combined, but a bit slower:
			return val - ( val < 58 ? 48 : ( val < 97 ? 55 : 87 ) );
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
		public static String GetTextUTF8( int Pointer, byte[] File ) {
			if ( Pointer == -1 ) return null;

			try {
				int i = Pointer;
				while ( File[i] != 0x00 ) {
					i++;
				}
				String Text = Encoding.UTF8.GetString( File, Pointer, i - Pointer );
				return Text;
			} catch ( Exception ) {
				return null;
			}
		}
		public static String TrimNull( String s ) {
			int n = s.IndexOf( '\0', 0 );
			if ( n >= 0 ) {
				return s.Substring( 0, n );
			}
			return s;
		}
		public static byte[] StringToBytesShiftJis( String s ) {
			//byte[] bytes = ShiftJISEncoding.GetBytes(s);
			//return bytes.TakeWhile(subject => subject != 0x00).ToArray();
			return ShiftJISEncoding.GetBytes( s );
		}
		#endregion

		#region TimeUtils
		public static DateTime UnixTimeToDateTime( uint unixTime ) {
			return new DateTime( 1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc ).AddSeconds( unixTime ).ToLocalTime();
		}
		public static DateTime PS3TimeToDateTime( ulong PS3Time ) {
			return new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc ).AddMilliseconds( PS3Time / 1000 ).ToLocalTime();
		}
		#endregion
	}
}
