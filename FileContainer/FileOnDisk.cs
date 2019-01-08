using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.FileContainer {
	public class FileOnDisk : IFile {
		private string Path;

		public FileOnDisk( string path ) {
			Path = path;
		}

		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public DuplicatableStream DataStream => new DuplicatableFileStream( Path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read );
	}
}
