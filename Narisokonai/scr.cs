using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Narisokonai {
	public class scrElement {
		public enum scrElementType { Code, Text }
		public scrElementType Type;
		public string Text;
		public byte[] Code;

		public override string ToString() {
			switch ( Type ) {
				case scrElementType.Text: return Text;
				case scrElementType.Code: return BitConverter.ToString( Code );
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
					byte[] Code;
					switch ( NextType ) {
						case scrElementType.Code:
							Code = new byte[Current - Start];
							ArrayUtils.CopyByteArrayPart( File, (int)Start, Code, 0, Code.Length );
							newElement.Code = Code;
							newElement.Text = TextUtils.ShiftJISEncoding.GetString( File, (int)Start, (int)( Current - Start ) ); // just checking something...
							NextType = scrElementType.Text;
							break;
						case scrElementType.Text:
							newElement.Text = TextUtils.ShiftJISEncoding.GetString( File, (int)Start, (int)( Current - Start ) );
							NextType = scrElementType.Code;
							// original byte array must equal back-converted text, otherwise we just read a code section as text
							byte[] check = TextUtils.StringToBytesShiftJis( newElement.Text );
							if ( (int)( Current - Start ) != check.Length || !ArrayUtils.IsByteArrayPartEqual( check, 0, File, (int)Start, check.Length ) ) {
								// well guess this is not actually text!
								Code = new byte[Current - Start];
								ArrayUtils.CopyByteArrayPart( File, (int)Start, Code, 0, Code.Length );
								newElement.Code = Code;
								newElement.Type = scrElementType.Code;
								NextType = scrElementType.Text;
							}
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

	public class scrSectionPointerIndexComparer : IComparer<scrSection> {
		public int Compare( scrSection x, scrSection y ) { return x.PointerIndex.CompareTo( y.PointerIndex ); }
	}
	public class scrSection : IComparable<scrSection> {
		public int PointerIndex;
		public uint Location;
		public List<scrElement> Elements;

		public int CompareTo( scrSection other ) {
			return this.Location.CompareTo( other.Location );
		}
		public int CompareToByPointerIndex( scrSection other ) {
			return this.PointerIndex.CompareTo( other.PointerIndex );
		}
		public override string ToString() {
			return "Idx: " + PointerIndex + " / Ptr: " + Location.ToString( "X6" ) + " / ElementCount: " + Elements.Count;
		}
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
		public scr() { }

		public List<scrSection> Sections;

		private bool LoadFile( byte[] File ) {
			uint PointerCount = NumberUtils.ToUInt24( File, 0 );
			List<uint> Pointers = new List<uint>( (int)PointerCount );

			for ( int i = 0; i < PointerCount; ++i ) {
				Pointers.Add( NumberUtils.ToUInt24( File, 0x03 + i * 0x03 ) );
			}

			// create sections we fill in later, so we can remember the original order
			Sections = new List<scrSection>();

			// one dummy at the start for anything before the first pointer
			scrSection sec = new scrSection();
			sec.PointerIndex = -1;
			sec.Location = 0;
			Sections.Add( sec );

			for ( int i = 0; i < Pointers.Count; ++i ) {
				sec = new scrSection();
				sec.PointerIndex = i;
				sec.Location = Pointers[i];
				Sections.Add( sec );
			}

			// sort by Location so we don't read any code/text more than once
			Sections.Sort();

			// one dummy section at end to make the next loop simpler
			uint HeaderEnd = PointerCount * 0x03 + 0x03;
			sec = new scrSection();
			sec.PointerIndex = Pointers.Count;
			sec.Location = (uint)File.Length - HeaderEnd;
			Sections.Add( sec );

			// read the file into code and strings
			for ( int i = 0; i < Sections.Count - 1; ++i ) {
				uint Pointer = Sections[i].Location + HeaderEnd;
				uint EndPointer = Sections[i + 1].Location + HeaderEnd;
				List<scrElement> l = scrElement.LoadAt( File, Pointer, EndPointer );

				Sections[i].Elements = l;
			}

			return true;
		}

		public void CreateFile( string Filename ) {

			// make sure it's sorted by pointer
			Sections.Sort();

			List<byte> b = new List<byte>();

			uint OriginalLocation = 0xFFFFFFFF;
			for ( int i = 0; i < Sections.Count; ++i ) {
				scrSection s = Sections[i];
				if ( s.Elements == null ) continue;


				// okay, so we have an issue here: if multiple sections originally pointed to the same thing,
				// all pointers that come here after the section that has the elements will point to the wrong (next) thing
				// so we need to compare the original location, and match it if it matched before
				if ( OriginalLocation == s.Location ) {
					s.Location = Sections[i - 1].Location;
				} else {
					OriginalLocation = s.Location;
					s.Location = (uint)b.Count;
				}

				foreach ( scrElement e in s.Elements ) {
					switch ( e.Type ) {
						case HyoutaTools.Narisokonai.scrElement.scrElementType.Code:
							b.AddRange( e.Code );
							break;
						case HyoutaTools.Narisokonai.scrElement.scrElementType.Text:
							b.AddRange( TextUtils.StringToBytesShiftJis( e.Text ) );
							break;
					}
					b.Add( 0xFE );
					b.Add( 0xFE );
				}

				if ( s.Elements.Count > 0 ) {
					// remove the last 0xFEFE, it's not in the original files, just makes the code easier to do it this way
					b.RemoveAt( b.Count - 1 );
					b.RemoveAt( b.Count - 1 );
				}
			}

			// sort by pointer index to get the original pointer order back
			Sections.Sort( new scrSectionPointerIndexComparer() );

			// create the header
			List<byte> Header = new List<byte>();
			uint ptrCount = 0;
			foreach ( scrSection s in Sections ) {
				if ( s.PointerIndex == -1 || s.Elements == null ) continue;
				Header.AddRange( NumberUtils.GetBytesForUInt24( s.Location ) );
				++ptrCount;
			}

			// and combine all this
			List<byte> File = new List<byte>( 3 + Header.Count + b.Count );
			File.AddRange( NumberUtils.GetBytesForUInt24( ptrCount ) );
			File.AddRange( Header );
			File.AddRange( b );

			System.IO.File.WriteAllBytes( Filename, File.ToArray() );

		}
	}
}
