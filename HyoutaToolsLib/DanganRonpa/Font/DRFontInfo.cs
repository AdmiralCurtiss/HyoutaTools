using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.DanganRonpa.Font {
	public class DRFontChar {
		public UInt16 Character;
		public UInt16 XOffset;
		public UInt16 YOffset;
		public UInt16 Width;
		public UInt16 Height;
		public UInt16 Unk1;
		public UInt16 Unk2;
		public UInt16 Unk3;
		static public int Compare( DRFontChar one, DRFontChar other ) {
			return one.Character - other.Character;
		}
	}

	public class DRFontInfo {
		Byte[] file;
		int InfoLength;

		int Magic;
		int Unknown1;
		int InfoCount;
		int InfoLocation;
		int LookupTableLength;
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
			this.LookupTableLength = BitConverter.ToInt32( file, 0x10 );
			this.LookupTableLocation = BitConverter.ToInt32( file, 0x14 );
			this.Unknown2 = BitConverter.ToInt32( file, 0x18 );
			this.Unknown3 = BitConverter.ToInt32( file, 0x1C );

			Chars = new List<DRFontChar>( InfoCount );

			for ( int i = 0; i < InfoCount; ++i ) {
				DRFontChar c = GetCharFromRaw( i );
				Chars.Add( c );
			}

			return;
		}

		public void WriteFile( String Filename ) {
			FileStream fs = new FileStream( Filename, FileMode.Create );


			LookupTableLength = 0;
			foreach ( DRFontChar c in Chars ) {
				LookupTableLength = Math.Max( c.Character, LookupTableLength );
			}
			LookupTableLength = LookupTableLength * 2 + 2;
			InfoCount = Chars.Count;
			InfoLocation = LookupTableLocation + LookupTableLength;


			fs.Write( BitConverter.GetBytes( Magic ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown1 ), 0, 4 );
			fs.Write( BitConverter.GetBytes( InfoCount ), 0, 4 );
			fs.Write( BitConverter.GetBytes( InfoLocation ), 0, 4 );
			fs.Write( BitConverter.GetBytes( LookupTableLength / 2 ), 0, 4 );
			fs.Write( BitConverter.GetBytes( LookupTableLocation ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown2 ), 0, 4 );
			fs.Write( BitConverter.GetBytes( Unknown3 ), 0, 4 );

			int currentCharId = 0;
			for ( int i = 0; i < LookupTableLength / 2; ++i ) {
				if ( Chars[currentCharId].Character == i ) {
					fs.Write( BitConverter.GetBytes( (ushort)currentCharId ), 0, 2 );
					++currentCharId;
				} else {
					fs.WriteByte( 0xFF );
					fs.WriteByte( 0xFF );
				}
			}

			foreach ( DRFontChar c in Chars ) {
				//c.YOffset += 2; <- convert DR1 font to SDR2 font
				//c.Height -= 5;
				fs.Write( BitConverter.GetBytes( c.Character ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.XOffset ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.YOffset ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.Width ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.Height ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.Unk1 ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.Unk2 ), 0, 2 );
				fs.Write( BitConverter.GetBytes( c.Unk3 ), 0, 2 );
			}

			while ( fs.Length % 16 != 0 ) {
				fs.WriteByte( 0x00 );
			}

			fs.Close();
		}

		public string[] GetGnConfig() {
			List<string> gn = new List<string>();

			for ( int i = 0; i < this.InfoCount; ++i ) {

				var ch = GetCharFromRaw( i );

				byte[] chbytes = BitConverter.GetBytes( ch.Character );
				char character = Encoding.Unicode.GetChars( chbytes )[0];

				string c = TextUtils.XmlEscape( character.ToString() );

				string n =
					"			<Glyph char=\"" + c + "\" " +
					"x=\"" + ch.XOffset + "\" y=\"" + ch.YOffset +
					"\" width=\"" + ch.Width + "\" height=\"" + ch.Height + "\" />";
				gn.Add( n );
			}

			return gn.ToArray();
		}

		public DRFontChar GetCharViaId( int id ) {
			return Chars[id];
		}
		public DRFontChar GetCharViaCharacter( ushort Character ) {
			foreach ( DRFontChar c in Chars ) {
				if ( c.Character == Character ) {
					return c;
				}
			}
			return null;
		}
		private DRFontChar GetCharFromRaw( int id ) {
			if ( id >= this.InfoCount ) id = 0;

			DRFontChar c = new DRFontChar();
			c.Character = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x00 );
			c.XOffset = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x02 );
			c.YOffset = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x04 );
			c.Width = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x06 );
			c.Height = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x08 );
			c.Unk1 = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x0A );
			c.Unk2 = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x0C );
			c.Unk3 = BitConverter.ToUInt16( file, InfoLocation + ( id * InfoLength ) + 0x0E );

			return c;
		}

		public void CopyInfoFrom( DRFontInfo f2 ) {
			foreach ( DRFontChar fc in f2.Chars ) {
				DRFontChar mine = GetCharViaCharacter( fc.Character );
				if ( mine == null ) {
					this.Chars.Add( fc );
				} else {
					mine.Character = fc.Character;
					mine.XOffset = fc.XOffset;
					mine.YOffset = fc.YOffset;
					mine.Width = fc.Width;
					mine.Height = fc.Height;
					mine.Unk1 = fc.Unk1;
					mine.Unk2 = fc.Unk2;
					mine.Unk3 = fc.Unk3;
				}
			}

			this.Chars.Sort( DRFontChar.Compare );
		}

		public void ImportExternalCharacter( DRFontChar fc ) {
			DRFontChar mine = GetCharViaCharacter( fc.Character );
			if ( mine == null ) {
				this.Chars.Add( fc );
				this.Chars.Sort( DRFontChar.Compare );
			} else {
				mine.Character = fc.Character;
				mine.XOffset = fc.XOffset;
				mine.YOffset = fc.YOffset;
				mine.Width = fc.Width;
				mine.Height = fc.Height;
				mine.Unk1 = fc.Unk1;
				mine.Unk2 = fc.Unk2;
				mine.Unk3 = fc.Unk3;
			}
		}

		public static void CopyCharInfo( DRFontChar CopyFrom, DRFontChar CopyTo ) {
			CopyTo.Width = CopyFrom.Width;
			CopyTo.Height = CopyFrom.Height;
			CopyTo.XOffset = CopyFrom.XOffset;
			CopyTo.YOffset = CopyFrom.YOffset;
			CopyTo.Unk1 = CopyFrom.Unk1;
			CopyTo.Unk2 = CopyFrom.Unk2;
			CopyTo.Unk3 = CopyFrom.Unk3;
		}

	}
}
