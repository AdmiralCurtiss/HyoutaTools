using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.DanganRonpa.Pak {
	class Program {

		public static void CopyStream( Stream input, Stream output, int count ) {
			byte[] buffer = new byte[4096];
			int read;

			int bytesLeft = count;
			while ( ( read = input.Read( buffer, 0, Math.Min( buffer.Length, bytesLeft ) ) ) > 0 ) {
				output.Write( buffer, 0, read );
				bytesLeft -= read;
				if ( bytesLeft <= 0 ) return;
			}
		}

		static void Extract( FileStream file, String destination ) {
			System.IO.Directory.CreateDirectory( destination );

			byte[] buffer = new byte[4];
			file.Read( buffer, 0, 4 );
			int count = BitConverter.ToInt32( buffer, 0 );

			int[] FileOffsets = new int[count + 1];
			for ( int i = 0; i < count; ++i ) {
				file.Read( buffer, 0, 4 );
				FileOffsets[i] = BitConverter.ToInt32( buffer, 0 );
			}
			FileOffsets[count] = (int)file.Length;

			for ( int i = 0; i < count; ++i ) {
				FileStream newFile = new FileStream( System.IO.Path.Combine( destination, i.ToString( "D4" ) ), FileMode.Create );
				file.Position = FileOffsets[i];
				CopyStream( file, newFile, FileOffsets[i + 1] - FileOffsets[i] );
				newFile.Close();
			}

			return;
		}

		static int ExecuteExtract( string[] args ) {
			bool fail = false;
			foreach ( string arg in args ) {
				try {
					Extract( new FileStream( arg, FileMode.Open ), arg + ".ex" );
				} catch ( Exception ) {
					Console.WriteLine( "Failed on " + arg );
					fail = true;
				}
			}
			return fail ? -1 : 0;
		}
	}
}
