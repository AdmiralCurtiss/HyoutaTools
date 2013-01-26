using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TownMap {
	static class Util {
		public static String Path = @"../../0.bin";

		public static String GetText( byte[] File, int Pointer ) {
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

		public static UInt32 SwapEndian( UInt32 x ) {
			return x = ( x >> 24 ) |
					  ( ( x << 8 ) & 0x00FF0000 ) |
					  ( ( x >> 8 ) & 0x0000FF00 ) |
					   ( x << 24 );
		}

		public static Int32 SwapEndian( Int32 y ) {
			UInt32 x = (uint)y;
			x = ( x >> 24 ) |
			  ( ( x << 8 ) & 0x00FF0000 ) |
			  ( ( x >> 8 ) & 0x0000FF00 ) |
			   ( x << 24 );
			return (int)x;
		}

		public static void DisplayException( Exception e ) {
			Console.WriteLine( "Exception occurred:\n" + e.Message );
		}
	}
}
