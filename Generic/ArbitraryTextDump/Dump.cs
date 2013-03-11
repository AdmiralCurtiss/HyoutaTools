using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Generic.ArbitraryTextDump {
	public class Dump {
		public static int Execute( List<string> args ) {
			string Filename = args[0];

			byte[] File = System.IO.File.ReadAllBytes( Filename );
			List<string> strs = new List<string>();

			for ( int i = 0; i < File.Length; ++i ) {
				while ( i < File.Length && File[i] == 0x00 ) { ++i; }
				if ( i >= File.Length ) break;

				string s;
				s = Util.GetTextShiftJis( File, i );
				s = s.Replace( '\x01', ' ' );
				s = s.Replace( '\x02', ' ' );
				s = s.Replace( '\x03', ' ' );
				strs.Add( s );

				while ( i < File.Length && File[i] != 0x00 ) { ++i; }
			}

			System.IO.File.WriteAllLines( Filename + ".dmp", strs.ToArray() );

			return 0;
		}
	}
}
