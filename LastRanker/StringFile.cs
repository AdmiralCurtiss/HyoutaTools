using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.LastRanker {
	public class StringFile {
		public StringFile( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "StringFile: Load Failed!" );
			}
		}
		public StringFile( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "StringFile: Load Failed!" );
			}
		}


		private uint StringBlockSize;
		public List<bscrString> Strings;

		private bool LoadFile( byte[] File ) {
			Strings = new List<bscrString>();
			StringBlockSize = BitConverter.ToUInt32( File, 0 );

			int pos = 0x04;
			while ( pos < StringBlockSize + 4 ) {
				bscrString s = new bscrString();
				int nullLoc;
				s.String = Util.GetTextUTF8( File, pos, out nullLoc );
				s.Position = (uint)pos;

				Strings.Add( s );
				pos = nullLoc + 1;
			}

			return true;
		}
	}
}
