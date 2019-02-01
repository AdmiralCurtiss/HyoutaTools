using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.FileContainer {
	public class FileOnDisk : IFile {
		public FileOnDisk( string path ) {
			DataStream = new DuplicatableFileStream( path, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read );
		}

		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public DuplicatableStream DataStream { get; }

		public void Dispose() {
			DataStream.Dispose();
		}
	}
}
