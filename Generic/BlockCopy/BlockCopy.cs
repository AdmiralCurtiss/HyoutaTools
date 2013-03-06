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

			byte[] Source = File.ReadAllBytes( args[0] );
			byte[] Destination = File.ReadAllBytes( args[2] );

			int SourceStart = Int32.Parse( args[1], NumberStyles.AllowHexSpecifier );
			int DestinationStart = Int32.Parse( args[3], NumberStyles.AllowHexSpecifier );
			int Size;
			if ( args[4].ToLowerInvariant() == "auto" ) {
				Size = Source.Length;
			} else {
				Size = Int32.Parse( args[4], NumberStyles.AllowHexSpecifier );
			}

			for ( int i = 0; i < Size; i++ ) {
				Destination[DestinationStart + i] = Source[SourceStart + i];
			}

			File.WriteAllBytes( args[2], Destination );
			return 0;
		}
	}
}
