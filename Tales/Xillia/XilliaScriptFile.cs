using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Xillia {
	struct XS {
		public int PointerIDString;
		public int PointerText;
		public String IDString;
		public String Text;
	}

	class XilliaScriptFile {

		public List<XS> TextList;
		public int Offset = 0;

		public XilliaScriptFile( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "Loading a Xillia Script File failed!" );
			}
		}

		public XilliaScriptFile( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "Loading a Xillia Script File failed!" );
			}
		}

		private bool LoadFile( byte[] Bytes ) {
			Offset = 0;
			int Ident = BitConverter.ToInt32( Bytes, Offset );
			while ( Ident != 0x08000000 ) {
				Offset += 4;
				Ident = BitConverter.ToInt32( Bytes, Offset );
			}


			uint Amount = HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Bytes, Offset + 4 ) );
			TextList = new List<XS>( (int)Amount );

			Amount *= 2;

			for ( uint i = 0; i < Amount; i += 2 ) {
				XS x = new XS();
				x.PointerIDString = ( Offset + ( (int)i * 4 + 8 ) ) + (int)HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Bytes, Offset + ( (int)i * 4 + 8 ) ) );
				x.PointerText = ( Offset + ( (int)( i + 1 ) * 4 + 8 ) ) + (int)HyoutaTools.Util.SwapEndian( BitConverter.ToUInt32( Bytes, Offset + ( (int)( i + 1 ) * 4 + 8 ) ) );
				x.IDString = HyoutaTools.Util.GetTextUTF8( x.PointerIDString, Bytes );
				x.Text = HyoutaTools.Util.GetTextUTF8( x.PointerText, Bytes );
				TextList.Add( x );
			}


			return true;
		}
	}
}
