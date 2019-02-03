using System;
using HyoutaTools.Streams;

namespace HyoutaTools.FileContainer {
	public abstract class FileBase : IFile {
		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public abstract DuplicatableStream DataStream { get; }
		public abstract void Dispose();
	}
}
