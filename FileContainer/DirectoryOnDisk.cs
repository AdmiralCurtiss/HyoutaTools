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

		private static INode CreateNode( System.IO.FileSystemInfo f ) {
			if ( f is System.IO.DirectoryInfo ) {
				return new DirectoryOnDisk( f.FullName );
			} else {
				return new FileOnDisk( f.FullName );
			}
		}

		public INode GetChildByIndex( long index ) {
			if ( index > 0 && index < Children.Length ) {
				return CreateNode( Children[index] );
			}
			return null;
		}

		public INode GetChildByName( string name ) {
			foreach ( var ch in Children ) {
				if ( ch.Name == name ) {
					return CreateNode( ch );
				}
			}
			return null;
		}

		public void Dispose() {
		}
	}
}
