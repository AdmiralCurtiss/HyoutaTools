using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.FileContainer {
	public class FileFromStream : IFile {
		public FileFromStream( DuplicatableStream stream ) {
			DataStream = stream.Duplicate();
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
