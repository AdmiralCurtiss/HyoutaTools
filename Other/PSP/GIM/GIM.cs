using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM {
	class GIM {

		byte[] File;
		List<ISection> Sections;

		public GIM( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		public void Initialize( byte[] File ) {
			this.File = File;
			uint location = 0x10;

			Sections = new List<ISection>();
			Sections.Add( new HeaderSection( File, 0 ) );
			while ( location < File.Length ) {
				ushort CurrentType = BitConverter.ToUInt16( File, (int)location );
				ISection section;
				switch ( CurrentType ) {
					case 0x02:
						section = new EndOfFileSection( File, (int)location );
						break;
					case 0x03:
						section = new EndOfImageSection( File, (int)location );
						break;
					case 0x04:
						section = new ImageSection( File, (int)location );
						break;
					case 0x05:
						section = new PaletteSection( File, (int)location );
						break;
					case 0xFF:
						section = new FileInfoSection( File, (int)location );
						break;
					default:
						throw new Exception( "Invalid Section Type" );
				}

				Sections.Add( section );
				location += section.GetPartSize();
			}
		}

		public uint GetTotalFilesize() {
			uint totalFilesize = 0;
			foreach ( var section in Sections ) {
				totalFilesize += section.GetPartSize();
			}
			return totalFilesize;
		}

		public void ReduceToOneImage( int imageNumber ) {
			foreach ( var section in Sections ) {
				if ( section.GetType() == typeof( ImageSection ) ) {
					ImageSection isec = (ImageSection)section;
					byte[] img = isec.Images[imageNumber];
					isec.Images = new byte[1][];
					isec.Images[0] = img;
					isec.Width = (ushort)( isec.Width >> imageNumber );
					isec.Height = (ushort)( isec.Height >> imageNumber );
				}
				if ( section.GetType() == typeof( PaletteSection ) ) {
					PaletteSection psec = (PaletteSection)section;
					byte[] pal = psec.Palettes[imageNumber];
					psec.Palettes = new byte[1][];
					psec.Palettes[0] = pal;
				}
			}

			uint fileinfosection = 0;
			foreach ( var section in Sections ) {
				section.Recalculate( 0 );
				if ( section.GetType() == typeof( EndOfFileSection ) ) {
					fileinfosection = section.GetPartSize();
				}
			}
			uint Filesize = GetTotalFilesize();
			foreach ( var section in Sections ) {
				if ( section.GetType() == typeof( EndOfFileSection ) ) {
					section.Recalculate( (int)Filesize - 0x10 );
				}
				if ( section.GetType() == typeof( EndOfImageSection ) ) {
					section.Recalculate( (int)Filesize - 0x64 - (int)fileinfosection );
				}
			}
		}

		public byte[] Serialize() {
			List<byte> newfile = new List<byte>( File.Length );
			foreach ( var section in Sections ) {
				newfile.AddRange( section.Serialize() );
			}
			return newfile.ToArray();
		}
	}
}
