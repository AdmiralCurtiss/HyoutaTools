using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Abyss.PKF {
	public class Split {
		public static int Execute( List<string> args ) {
			string Filename = args[0];
			byte[] File = System.IO.File.ReadAllBytes( Filename );
			return SplitPkf( File, Filename + ".ex" ) ? 0 : -1;
		}

		public static bool SplitPkf( Byte[] File, string Directory ) {
			try {
				System.IO.Directory.CreateDirectory( Directory );
				uint location = 0;
				while ( true ) {
					byte Ident = File[location];
					uint CompressedSize = BitConverter.ToUInt32( File, (int)location + 1 );
					uint DecompressedSize = BitConverter.ToUInt32( File, (int)location + 5 );

					byte[] SplitFile = new byte[CompressedSize + 9];
					Util.CopyByteArrayPart( File, (int)location, SplitFile, 0, (int)CompressedSize + 9 );

					System.IO.File.WriteAllBytes( Directory + "/" + location.ToString( "X8" ) + ".c", SplitFile );

					location = Util.Align( location + CompressedSize + 9, 0x800u );
					if ( location >= File.Length ) break;
				}
			} catch ( Exception ) {
				return false;
			}
			return true;
		}
	}
}
