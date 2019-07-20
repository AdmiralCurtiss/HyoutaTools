using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;

namespace HyoutaTools.FileContainer {
	public class FileOnDisk : IFile {
		public FileOnDisk( string path ) {
			DataStream = new DuplicatableFileStream( path );
		}

		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public DuplicatableStream DataStream { get; }

		public void Dispose() {
			DataStream.Dispose();
		}

		public override string ToString() {
			return DataStream.ToString();
		}
	}
}
