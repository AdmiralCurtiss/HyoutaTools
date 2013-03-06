using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.GoldenSunDarkDawnMsgExtract {
	class Program {
		public static int Execute( List<string> args ) {
			String Filename = args[0];


			Byte[] File = System.IO.File.ReadAllBytes( Filename );

			List<String> Lines = new List<string>();
			List<int> Pointers = new List<int>();

			for ( int i = 0; i < File.Length; i += 4 ) {
				int ptr = BitConverter.ToInt32( File, i );
				if ( ptr == -1 ) break;
				Pointers.Add( ptr );
			}

			foreach ( int ptr in Pointers ) {
				int i;
				for ( i = 0; ( i + ptr ) < File.Length; i += 2 ) {
					int tmp = BitConverter.ToInt16( File, ptr + i );
					if ( tmp == 0 ) break;
				}
				Lines.Add( Encoding.Unicode.GetString( File, ptr, i ) );
			}

			String[] variables = { "\xF00E", "\xF01C", "\xF023" };

			for ( int i = 0; i < Lines.Count; i++ ) {
				Lines[i] = Lines[i].Replace( '\n', ' ' );
				foreach ( String v in variables ) {
					while ( Lines[i].Contains( v ) ) {
						String str = Lines[i];
						int loc = str.IndexOf( v );
						String a = str.Substring( 0, loc );
						String var = str.Substring( loc, 2 );
						String b = str.Substring( loc + 2 );

						byte[] by = Encoding.Unicode.GetBytes( var );

						Lines[i] = a + "Variable" + BitConverter.ToString( by ).Replace( "-", "" ) + b;
					}
				}
			}

			System.IO.File.WriteAllLines( Filename + ".txt", Lines.ToArray() );

			return 0;
		}
	}
}
