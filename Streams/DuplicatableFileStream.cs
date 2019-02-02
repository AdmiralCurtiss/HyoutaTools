using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HyoutaTools.Streams {
	public class DuplicatableFileStream : DuplicatableStream {
		private FileStream BaseStreamInternal;
		private string Path;
		private FileMode Mode;
		private FileAccess Access;
		private FileShare Share;
		private bool Disposed;

		private FileStream Base {
			get {
				if ( Disposed ) {
					throw new Exception( "Accessing disposed DuplicatableFileStream." );
				}
				if ( BaseStreamInternal == null ) {
					BaseStreamInternal = new FileStream( Path, Mode, Access, Share );
				}
				return BaseStreamInternal;
			}
		}

		public DuplicatableFileStream( string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read, FileShare share = FileShare.Read ) {
			BaseStreamInternal = null;
			Path = path;
			Mode = mode;
			Access = access;
			Share = share;
			Disposed = false;
		}

		public override DuplicatableStream Duplicate() {
			if ( Disposed ) {
				throw new Exception( "Duplicating disposed DuplicatableFileStream." );
			}
			return new DuplicatableFileStream( Path, Mode, Access, Share );
		}

		public override bool CanRead => Base.CanRead;
		public override bool CanSeek => Base.CanSeek;
		public override bool CanWrite => Base.CanWrite;
		public override long Length => Base.Length;
		public override long Position { get => Base.Position; set => Base.Position = value; }
		public override void Flush() { Base.Flush(); }
		public override int Read( byte[] buffer, int offset, int count ) { return Base.Read( buffer, offset, count ); }
		public override long Seek( long offset, SeekOrigin origin ) { return Base.Seek( offset, origin ); }
		public override void SetLength( long value ) { Base.SetLength( value ); }
		public override void Write( byte[] buffer, int offset, int count ) { Base.Write( buffer, offset, count ); }

		public override int ReadByte() { return Base.ReadByte(); }
		public override void WriteByte( byte value ) { Base.WriteByte( value ); }

		protected override void Dispose( bool disposing ) {
			End();
			Disposed = true;
		}

		public override string ToString() {
			return "File at " + Path;
		}

		public override void ReStart() {
			// Base accessor creates the stream if it's currently not
			Base.Position = 0;
		}

		public override void End() {
			if ( BaseStreamInternal != null ) {
				BaseStreamInternal.Dispose();
				BaseStreamInternal = null;
			}
		}
	}
}
