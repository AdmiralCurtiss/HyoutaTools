using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.DanganRonpa.Font {
	public struct DRFontChar {
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
		int InfoLocation;
		int InfoLength;
		int InfoCount;


		public DRFontInfo( Byte[] file ) {
			this.file = file;
			Initialize();
		}

		void Initialize() {
			this.InfoLength = 0x10;
			this.InfoCount = BitConverter.ToInt32( file, 0x08 );
			this.InfoLocation = BitConverter.ToInt32( file, 0x0C );
		}

		public void WriteFile( String Filename ) {
			System.IO.File.WriteAllBytes( Filename, file );
		}

		public string[] GetGnConfig() {
			List<string> gn = new List<string>();

			for ( int i = 0; i < this.InfoCount; ++i ) {

				var ch = GetChar( i );

				byte[] chbytes = BitConverter.GetBytes(ch.Character);
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

		public void SetChar( int id, DRFontChar c ) {
			if ( id >= this.InfoCount ) id = 0;

			BitConverter.GetBytes( c.Character ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x00 );
			BitConverter.GetBytes( c.XOffset ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x02 );
			BitConverter.GetBytes( c.YOffset ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x04 );
			BitConverter.GetBytes( c.Width ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x06 );
			BitConverter.GetBytes( c.Height ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x08 );
			BitConverter.GetBytes( c.Unk1 ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x0A );
			BitConverter.GetBytes( c.Unk2 ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x0C );
			BitConverter.GetBytes( c.Unk3 ).CopyTo( file, InfoLocation + ( id * InfoLength ) + 0x0E );
		}
	}
}
