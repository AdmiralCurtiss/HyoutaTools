using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.DanganRonpa.Font {
	public class DRFontChar {
		public Int16 Character;
		public Int16 XOffset;
		public Int16 YOffset;
		public Int16 Width;
		public Int16 Height;
		public Int16 Unk1;
		public Int16 Unk2;
		public Int16 Unk3;
	}

	public class DRFontInfo {
		Byte[] file;
		int InfoLength;

		int Magic;
		int Unknown1;
		int InfoCount;
		int InfoLocation;
		int LookupTableCount;
		int LookupTableLocation;
		int Unknown2;
		int Unknown3;

		List<DRFontChar> Chars;

		public DRFontInfo( Byte[] file ) {
			this.file = file;
			Initialize();
		}

		void Initialize() {
			this.InfoLength = 0x10;
			this.Magic = BitConverter.ToInt32( file, 0x00 );
			this.Unknown1 = BitConverter.ToInt32( file, 0x04 );
			this.InfoCount = BitConverter.ToInt32( file, 0x08 );
			this.InfoLocation = BitConverter.ToInt32( file, 0x0C );
			this.LookupTableCount = BitConverter.ToInt32( file, 0x10 );
			this.LookupTableLocation = BitConverter.ToInt32( file, 0x14 );
			this.Unknown2 = BitConverter.ToInt32( file, 0x18 );
			this.Unknown3 = BitConverter.ToInt32( file, 0x1C );

			Chars = new List<DRFontChar>( InfoCount );

			for ( int i = 0; i < InfoCount; ++i ) {
				DRFontChar c = GetCharFromRaw( i );
				Chars.Add( c );
			}
		}

		public void WriteFile( String Filename ) {
			FileStream fs = new FileStream( Filename, FileMode.Create );


			LookupTableCount = 0;
			foreach ( DRFontChar c in Chars ) {
				LookupTableCount = Math.Max( c.Character, LookupTableCount );
			}
			InfoCount = Chars.Count;
			InfoLocation = LookupTableLocation + 2 * LookupTableCount;


			fs.Write( BitConverter.GetBytes( Magic ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown1 ), 0, 4 );
			fs.Write( BitConverter.GetBytes( InfoCount ), 0, 4 );
			fs.Write( BitConverter.GetBytes( InfoLocation ), 0, 4 );
			fs.Write( BitConverter.GetBytes( LookupTableCount ), 0, 4 );
			fs.Write( BitConverter.GetBytes( LookupTableLocation ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown2 ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown3 ), 0, 4 );

			int currentCharId = 0;
			for ( int i = 0; i < LookupTableCount; ++i ) {
				if ( Chars[currentCharId].Character == i ) {
					fs.Write( BitConverter.GetBytes( (ushort)currentCharId ), 0, 2 );
					++currentCharId;
				} else {
					fs.WriteByte( 0xFF );
					fs.WriteByte( 0xFF );
				}
			}

			foreach ( DRFontChar c in Chars ) {
				fs.Write( BitConverter.GetBytes( c.Character ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.XOffset ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.YOffset ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.Width ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.Height ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.Unk1 ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.Unk2 ), 0, 4 );
				fs.Write( BitConverter.GetBytes( c.Unk3 ), 0, 4 );
			}

			fs.Close();
		}

		public string[] GetGnConfig() {
			List<string> gn = new List<string>();

			for ( int i = 0; i < this.InfoCount; ++i ) {

				var ch = GetCharFromRaw( i );

				byte[] chbytes = BitConverter.GetBytes( ch.Character );
				char character = Encoding.Unicode.GetChars( chbytes )[0];

				string c = Util.XmlEscape( character.ToString() );

				string n =
					"			<Glyph char=\"" + c + "\" " +
					"x=\"" + ch.XOffset + "\" y=\"" + ch.YOffset +
					"\" width=\"" + ch.Width + "\" height=\"" + ch.Height + "\" />";
				gn.Add( n );
			}

			return gn.ToArray();
		}

		public DRFontChar GetChar( int id ) {
			return Chars[id];
		}
		private DRFontChar GetCharFromRaw( int id ) {
			if ( id >= this.InfoCount ) id = 0;

			DRFontChar c = new DRFontChar();
			c.Character = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x00 );
			c.XOffset = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x02 );
			c.YOffset = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x04 );
			c.Width = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x06 );
			c.Height = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x08 );
			c.Unk1 = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x0A );
			c.Unk2 = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x0C );
			c.Unk3 = BitConverter.ToInt16( file, InfoLocation + ( id * InfoLength ) + 0x0E );

			return c;
		}
	}
}
