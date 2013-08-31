using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Narisokonai {
	public class scrElement {
		public enum scrElementType { Code, Text }
		public scrElementType Type;
		public string Text;
		public byte[] Code;

		public override string ToString() {
			switch ( Type ) {
				case scrElementType.Text: return Text;
				case scrElementType.Code: return Code.Length.ToString();
			}
			return "[NONE]";
		}

		public static List<scrElement> LoadAt( byte[] File, uint Location, uint EndLocation ) {
			List<scrElement> l = new List<scrElement>();

			uint Start = Location;
			uint Current = Location;
			scrElementType NextType = scrElementType.Code;
			while ( true ) {
				if ( Current == EndLocation || ( File[Current] == 0xFE && File[Current + 1] == 0xFE ) ) {
					scrElement newElement = new scrElement();
					newElement.Type = NextType;
					switch ( NextType ) {
						case scrElementType.Code:
							byte[] Code = new byte[Current - Start];
							Util.CopyByteArrayPart( File, (int)Start, Code, 0, Code.Length );
							newElement.Code = Code;
							NextType = scrElementType.Text;
							break;
						case scrElementType.Text:
							newElement.Text = Util.ShiftJISEncoding.GetString( File, (int)Start, (int)( Current - Start ) );
							NextType = scrElementType.Code;
							break;
					}
					l.Add( newElement );

					if ( Current == EndLocation ) break;

					Current += 2;
					Start = Current;
					continue;
				}

				++Current;
			}

			return l;
		}
	}

	public class scrSection {
		public int PointerLocation;
		public uint Location;
		public List<scrElement> Elements;
	}

	public class scr {
		public scr( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "scr: Load Failed!" );
			}
		}
		public scr( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "scr: Load Failed!" );
			}
		}

		List<scrSection> Sections;

		private bool LoadFile( byte[] File ) {
			uint PointerCount = Util.ToUInt24( File, 0 );
			List<uint> Pointers = new List<uint>( (int)PointerCount );

			for ( int i = 0; i < PointerCount; ++i ) {
				Pointers.Add( Util.ToUInt24( File, 0x03 + i * 0x03 ) );
			}

			uint[] OriginalPointerOrder = new uint[ Pointers.Count ];
			Pointers.CopyTo(OriginalPointerOrder);


			Pointers.Add( 0 ); // make sure we get the start
			Pointers.Sort();
			uint HeaderEnd = PointerCount * 0x03 + 0x03;
			Pointers.Add( (uint)File.Length - HeaderEnd );	// make sure we go until the end

			// read the file into code and strings
			Sections = new List<scrSection>();
			for ( int i = 0; i < Pointers.Count - 1; ++i ) {
				uint Pointer = Pointers[i] + HeaderEnd;
				uint EndPointer = Pointers[i + 1] + HeaderEnd;
				List<scrElement> l = scrElement.LoadAt( File, Pointer, EndPointer );

				scrSection sec = new scrSection();
				sec.Location = Pointers[i];
				sec.Elements = l;
				Sections.Add( sec );
			}

			// figure out the original locations for the pointers and re-sort
			foreach ( scrSection sec in Sections ) {
				sec.PointerLocation = -1;
				for ( int i = 0; i < OriginalPointerOrder.Length; ++i ) {
					if ( OriginalPointerOrder[i] == sec.Location ) {
						if ( sec.PointerLocation != -1 ) throw new Exception( "scr: Found duplicate pointer!" );
						sec.PointerLocation = i;
					}
				}
			}

			return true;
		}
	}
}
