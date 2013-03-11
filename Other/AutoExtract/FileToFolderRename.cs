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
				fstr.Filename.LastIndexOf( '\\' );
			}

			return 0;
		}
	}
}
