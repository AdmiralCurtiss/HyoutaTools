using System;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;

namespace HyoutaTools.FileContainer {
	public abstract class FileBase : IFile {
		public bool IsFile => true;
		public bool IsContainer => false;
		public IFile AsFile => this;
		public IContainer AsContainer => null;

		public abstract DuplicatableStream DataStream { get; }
		public abstract void Dispose();

		public override string ToString() {
			return DataStream.ToString();
		}
	}
}
