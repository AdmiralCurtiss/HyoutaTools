using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using HyoutaUtils;

namespace HyoutaTools {
	public static class Util {
		public static DateTime PS3TimeToDateTime( ulong PS3Time ) {
			return new DateTime( 1, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc ).AddMilliseconds( PS3Time / 1000 ).ToLocalTime();
		}

		public static string GuessFileExtension( Stream s ) {
			uint magic32 = s.PeekUInt32();
			uint magic24 = s.PeekUInt24();
			uint magic16 = s.PeekUInt16();

			switch ( magic32 ) {
				case 0x46464952:
					return ".wav";
				case 0x474E5089:
					return ".png";
				case 0x5367674F:
					return ".ogg";
			}
			switch ( magic16 ) {
				case 0x4D42:
					return ".bmp";
			}

			return "";
		}

		public static void Assert( bool cond ) {
			if ( !cond ) {
				throw new Exception( "Assert Failed!" );
			}
		}
	}
}
