using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles {
	public static class TextConverter {
		public static string ReadToNulltermAndDecode( System.IO.Stream s ) {
			StringBuilder sb = new StringBuilder();
			byte[] buffer = new byte[2];

			int b = s.ReadByte();
			while ( b != 0 && b != -1 ) {
				if ( ( b >= 0 && b <= 0x80 ) || ( b >= 0xA0 && b <= 0xDF ) ) {
					// is a single byte
						buffer[0] = (byte)b;
						sb.Append( TextUtils.ShiftJISEncoding.GetString( buffer, 0, 1 ) );
				} else {
					if ( b != 0xFF ) {
						// is two bytes
						buffer[0] = (byte)b;
						buffer[1] = (byte)s.ReadByte();
						sb.Append( TextUtils.ShiftJISEncoding.GetString( buffer ) );
					} else {
						// is a FFCC command
						b = s.ReadByte();
						switch ( b ) {
							case 0xA0: // newline
								sb.AppendLine();
								break;
							case 0xA1: // end of textbox
								sb.AppendLine();
								sb.AppendLine();
								break;
							case 0xA2:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xA3: // wait before printing more
								sb.Append( "<Wait: " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xA4:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xA5:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xA6:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xA7: // multiple choice
								sb.Append( "<Choice: " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xA8:
								sb.Append( "<Player Name: " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xA9:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xAA:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt48().ToString( "X12" ) + ">" );
								break;
							case 0xAB:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xAC:
								sb.Append( "<Color: Black>" );
								break;
							case 0xAD:
								sb.Append( "<Color: Blue>" );
								break;
							case 0xAE:
								sb.Append( "<Color: Red>" );
								break;
							case 0xAF:
								sb.Append( "<Color: Pink>" );
								break;
							case 0xB0:
								sb.Append( "<Color: Green>" );
								break;
							case 0xB1:
								sb.Append( "<" + b.ToString( "X2" ) + ">" ); // color?
								break;
							case 0xB2:
								sb.Append( "<Color: Yellow>" );
								break;
							case 0xB3:
								sb.Append( "<Color: Default>" );
								break;
							case 0xB4:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xB5:
								sb.Append( "<Color: Orange>" );
								break;
							case 0xB6:
								sb.Append( "<" + b.ToString( "X2" ) + ">" ); // color?
								break;
							case 0xBA:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xBD:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xC2:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt64().ToString( "X16" ) + ">" );
								break;
							case 0xC4:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xC5:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xC6:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xC7:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xC8:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xC9:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xCA:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xCB:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xCC:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xCD:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt48().ToString( "X12" ) + ">" );
								break;
							case 0xCE:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xCF:
								sb.Append( "<Hometown>" );
								break;
							case 0xD0: // print numeric variable
								sb.Append( "<Numeric Variable " + s.ReadUInt24().ToString( "X6" ) + ">" );
								break;
							case 0xD1:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt64().ToString( "X16" ) + ">" );
								break;
							case 0xD2:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xD3:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xD4:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xD5:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xD6:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt24().ToString( "X6" ) + ">" );
								break;
							case 0xD7:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xD8:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xD9:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xDA:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt40().ToString( "X10" ) + ">" );
								break;
							case 0xDB:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt32().ToString( "X8" ) + ">" );
								break;
							case 0xDC:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt40().ToString( "X10" ) + ">" );
								break;
							case 0xDD:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt48().ToString( "X12" ) + ">" );
								break;
							case 0xDE:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt56().ToString( "X14" ) + ">" );
								break;
							case 0xDF:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt64().ToString( "X16" ) + ">" );
								break;
							case 0xE0:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt56().ToString( "X14" ) + ">" );
								break;
							case 0xE1:
								sb.Append( "<" + b.ToString( "X2" ) + ": " + s.ReadUInt16().ToString( "X4" ) + ">" );
								break;
							case 0xE2: // start of a singular/plural either/or checking a variable; singular follows
								sb.Append( "<If Variable " + s.ReadUInt16().ToString( "X4" ) + " == 1 Then: " );
								break;
							case 0xE3: // start of a singular/plural either/or checking the single/multiplayer status; singular follows
								sb.Append( "<If Player Count == 1 Then: " );
								break;
							case 0xE4: // start of a male/female either/or checking the current player character; male follows
								sb.Append( "<If Player is Male Then: " );
								break;
							case 0xE5: // start of a male/female either/or checking a different player character; male follows
								sb.Append( "<If Character is Male Then: " );
								break;
							case 0xE6: // middle of an either/or; else case follows
								sb.Append( " / Else: " );
								break;
							case 0xE8:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xE9:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xEB:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xED:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xEE:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xEF:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xF0:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xF2:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xF3:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xF4:
								sb.Append( "<" + b.ToString( "X2" ) + ">" );
								break;
							case 0xE7: // end of an either/or
								sb.Append( ">" );
								break;
							default:
								string unknownCommandString = "Unknown FFCC text command 0x" + b.ToString( "X2" );
								sb.Append( "<" + unknownCommandString + ">" );
								break;
						}
					}
				}
				b = s.ReadByte();
			}
			return sb.ToString();
		}
	}
}
