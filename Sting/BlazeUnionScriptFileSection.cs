using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Sting {
	public class BlazeUnionScriptFileSection {
		public Dictionary<uint, string> Strings = new Dictionary<uint, string>();

		public BlazeUnionScriptFileSection( Stream stream, uint sectionEnd ) {
			List<uint> pointers = new List<uint>();

			for ( int i = 0; i < 9; ++i ) {
				pointers.Add( stream.ReadUInt32() );
			}

			// search for byte sequence indicating string pointers
			uint startOfBytecode = pointers[0]; // probably always right?
			stream.Position = startOfBytecode;
			while ( stream.Position < sectionEnd - 2 ) {
				if ( stream.PeekUInt24() == 0x000746 ) {
					stream.ReadUInt24();
					uint pointerPosition = Convert.ToUInt32( stream.Position );
					uint stringPointer = stream.ReadUInt32();
					Strings.Add( pointerPosition, ReadStringFromPositionAndReset( stream, stringPointer ) );

				} else {
					stream.ReadByte();
				}
			}
		}

		private string ReadStringFromPositionAndReset( Stream s, uint stringPosition ) {
			long pos = s.Position;
			s.Position = stringPosition;

			StringBuilder sb = new StringBuilder();
			byte[] buffer = new byte[2];

			int b = s.ReadByte();
			while ( b != 0 && b != -1 ) {
				if ( b == 0x01 ) {
					sb.Append( '\n' );
				} else if ( b == 0x02 ) {
					if ( s.ReadByte() != 0x00 ) { throw new Exception( "control code 0x02 not followed by a null byte!" ); }
					sb.AppendFormat( "<Voice: {0:x4}>", s.ReadUInt16() );
				} else if ( b == 0x03 ) {
					throw new Exception( "unknown control code 0x03" );
				} else if ( b == 0x04 ) {
					throw new Exception( "unknown control code 0x04" );
				} else if ( ( b > 0x04 && b <= 0x80 ) || ( b >= 0xA0 && b <= 0xDF ) ) {
					// is a single byte
					buffer[0] = (byte)b;
					sb.Append( Util.ShiftJISEncoding.GetString( buffer, 0, 1 ) );
				} else {
					// is two bytes
					buffer[0] = (byte)b;
					buffer[1] = (byte)s.ReadByte();
					sb.Append( Util.ShiftJISEncoding.GetString( buffer ) );
				}
				b = s.ReadByte();
			}

			s.Position = pos;
			return sb.ToString();
		}
	}
}
