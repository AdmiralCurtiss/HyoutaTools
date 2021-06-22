using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Globalization;

namespace HyoutaTools.Generic.BlockCopy {
	class BlockCopy {
		public static int Execute( List<string> args ) {
			if ( args.Count != 5 ) {
				Console.WriteLine( "Usage: BlockCopy SourceFile SourceStartLocation DestinationFile DestinationStartLocation Length" );
				Console.WriteLine( "All numbers (including length!) in Hex." );
				Console.WriteLine();
				Console.WriteLine( "Copies [Length] bytes from [SourceStartLocation] in [SourceFile] to [DestinationStartLocation] in [DestinationFile]." );
				Console.WriteLine( "Length may be \"auto\" to use whole source file." );
				return -1;
			}

			string srcFilename = args[0];
			string dstFilename = args[2];
			using ( var src = new System.IO.FileStream( srcFilename, System.IO.FileMode.Open, FileAccess.Read, FileShare.Read ) )
			using ( var dst = new System.IO.FileStream( dstFilename, System.IO.FileMode.Open, FileAccess.Write ) ) {
				int SourceStart = Int32.Parse( args[1], NumberStyles.AllowHexSpecifier );
				int DestinationStart = Int32.Parse( args[3], NumberStyles.AllowHexSpecifier );

				long Size;
				if ( args[4].ToLowerInvariant() == "auto" ) {
					Size = src.Length;
				} else {
					Size = Int64.Parse( args[4], NumberStyles.AllowHexSpecifier );
				}

				src.Position = SourceStart;
				dst.Position = DestinationStart;
				for ( int i = 0; i < Size; i++ ) {
					dst.WriteByte( (byte)src.ReadByte() );
				}

				dst.Close();
				src.Close();
			}
			return 0;
		}
	}
}
