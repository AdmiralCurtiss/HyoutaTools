using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Other.AceAttorneyInvestigationsScriptDump {
	class Dumper {
		public static int Execute( List<string> args ) {
			string filename = args[0];

			string[] table = new string[] {
				"",   "0",  "1",  "2",  "3",  "4",  "5",  "6",  "7",  "8",  "9",  "A",  "B",  "C",  "D",  "E",  // 0
				"F",  "G",  "H",  "I",  "J",  "K",  "L",  "M",  "N",  "O",  "P",  "Q",  "R",  "S",  "T",  "U",  // 1
				"V",  "W",  "X",  "Y",  "Z",  "a",  "b",  "c",  "d",  "e",  "f",  "g",  "h",  "i",  "j",  "k",  // 2
				"l",  "m",  "n",  "o",  "p",  "q",  "r",  "s",  "t",  "u",  "v",  "w",  "x",  "y",  "z",  "!",  // 3
				"?",  ".",  "(",  ")",  ":",  ",",  "",   "",   "'",  "",   "",   "",   "",   "",   "",   "",   // 4
				"",   "",   "",   "",   "",   "",   "",   "",   "",   " ",  "",   "",   "",   "",   "",   "",   // 5
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // 6
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // 7
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // 8
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // 9
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // A
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // B
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // C
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // D
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   // E
				"",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "",   "\n", "",   // F
			};

			List<string> lines = new List<string>();
			StringBuilder sb = new StringBuilder();
			using ( Stream instream = new FileStream( filename, System.IO.FileMode.Open ) ) {
				instream.DiscardBytes( 0x20 );
				while ( instream.Position < instream.Length ) {
					byte b = instream.ReadUInt8();
					if ( b == 0xFF ) {
						byte script = instream.ReadUInt8();
						switch ( script ) {
							case 0x02: {
									// text color?
									instream.ReadUInt8();
									break;
								}
							case 0x03:
							case 0x04: {
									// end of line, store and align to *next* 4 byte boundary
									// 0x03 waits for player input, 0x04 proceeds automatically
									lines.Add( sb.ToString() );
									sb.Clear();
									instream.ReadUInt8();
									instream.ReadAlign( 4 );
									break;
								}
							case 0x05: {
									// text delay
									instream.ReadUInt8();
									break;
								}
							case 0x07: {
									// centered text?
									instream.ReadUInt8();
									break;
								}
							case 0x08: {
									instream.DiscardBytes( 5 );
									break;
								}
							case 0x09: {
									instream.DiscardBytes( 2 );
									break;
								}
							case 0x0B: {
									instream.DiscardBytes( 5 );
									break;
								}
							case 0x16: {
									instream.ReadUInt8();
									break;
								}
							default: {
									sb.Append( "<Unknown script command 0x" + script.ToString( "X2" ) + ">" );
									break;
								}
						}
						continue;
					}

					string c = table[b];
					if ( c == "" ) {
						sb.Append( "<0x" + b.ToString( "X2" ) + ">" );
					} else {
						sb.Append( c );
					}
				}
			}

			if ( sb.Length > 0 ) {
				lines.Add( sb.ToString() );
			}

			for ( int i = 0; i < lines.Count; ++i ) {
				//if ( i != 174 )
				//	continue;
				Console.WriteLine( " == Line " + i + " == " );
				Console.WriteLine( lines[i] );
				Console.WriteLine();
				Console.WriteLine();
			}

			return 0;
		}
	}
}
