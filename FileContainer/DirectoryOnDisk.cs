using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	public class DirectoryOnDisk : IContainer {
		private System.IO.FileSystemInfo[] Children;

		public DirectoryOnDisk( string path ) {
			Children = new System.IO.DirectoryInfo( path ).GetFileSystemInfos();
		}

		public bool IsFile => false;
		public bool IsContainer => true;
		public IFile AsFile => null;
		public IContainer AsContainer => this;

		public INode GetChildByIndex( long index ) {
			if ( index > 0 && index < Children.Length ) {
				return new FileOnDisk( Children[index].FullName );
			}
			return null;
		}

		public INode GetChildByName( string name ) {
			foreach ( var ch in Children ) {
				if ( ch.Name == name ) {
					return new FileOnDisk( ch.FullName );
				}
			}
			return null;
		}
	}
}
