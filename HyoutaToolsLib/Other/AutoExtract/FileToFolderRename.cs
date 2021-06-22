using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.AutoExtract {
	class FileToFolderRename {
		public static int Execute( List<string> args ) {
			Queue<FileStruct> queue = new Queue<FileStruct>();

			Console.WriteLine( "Adding all files and folders recursively..." );
			Program.EnqueueDirectoryRecursively( queue, System.Environment.CurrentDirectory );

			while ( queue.Count > 0 ) {
				FileStruct fstr = queue.Dequeue();
				int idx = fstr.Filename.LastIndexOf( '\\' );
				string a = fstr.Filename.Substring( 0, idx );
				string b = fstr.Filename.Substring( idx + 1 );
				string newFilename = a + "." + b;

				System.IO.File.Move( fstr.Filename, newFilename );
			}

			return 0;
		}
	}
}
