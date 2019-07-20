using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.LastRanker {
	public class StringFile {
		public StringFile() {
			StringBlockSize = 0;
			Strings = new List<bscrString>();
		}

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
				s.String = TextUtils.GetTextUTF8( File, pos, out nullLoc );
				s.Position = (uint)pos;

				Strings.Add( s );
				pos = nullLoc + 1;
			}

			return true;
		}

		public void CreateFile( string Path ) {
			List<byte> NewFile = new List<byte>();
			NewFile.Add( 0x00 ); NewFile.Add( 0x00 ); NewFile.Add( 0x00 ); NewFile.Add( 0x00 );

			foreach ( bscrString s in Strings ) {
				byte[] b = Encoding.UTF8.GetBytes( s.String );
				NewFile.AddRange( b );
				NewFile.Add( 0x00 );
			}
			StringBlockSize = (uint)( NewFile.Count - 4 );

			byte[] sbb = BitConverter.GetBytes( StringBlockSize );
			ArrayUtils.CopyByteArrayPart( sbb, 0, NewFile, 0, 4 );

			System.IO.File.WriteAllBytes( Path, NewFile.ToArray() );
		}
	}
}
