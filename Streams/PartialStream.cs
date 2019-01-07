using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Streams {
	public class PartialStream : DuplicatableStream {
		private DuplicatableStream BaseStream;
		private long PartialStart;
		private long PartialLength;

		private long CurrentPosition;

		private long PartialEnd { get { return PartialStart + PartialLength; } }
		private long PartialBytesLeft { get { return PartialLength - CurrentPosition; } }

		public PartialStream( DuplicatableStream stream, long position, long length ) {
			if ( position < 0 ) { throw new Exception( "Invalid position, must be positive." ); }
			if ( length < 0 ) { throw new Exception( "Invalid length, must be positive." ); }

			BaseStream = stream.Duplicate();
			PartialStart = position;
			PartialLength = length;
			CurrentPosition = 0;
			BaseStream.Position = PartialStart;
		}

		public override bool CanRead => BaseStream.CanRead;
		public override bool CanSeek => BaseStream.CanSeek;
		public override bool CanWrite => BaseStream.CanWrite;
		public override long Length => PartialLength;
		public override long Position {
			get => CurrentPosition;
			set {
				if ( value < 0 ) {
					throw new Exception( "Invalid position for partial stream." );
				}
				CurrentPosition = Math.Min( value, PartialLength );
				BaseStream.Position = PartialStart + CurrentPosition;
			}
		}

		public override void Flush() {
			BaseStream.Flush();
		}

		public override int Read( byte[] buffer, int offset, int count ) {
			int c = (int)Math.Min( PartialBytesLeft, count );
			int v = BaseStream.Read( buffer, offset, c );
			CurrentPosition += v;
			return v;
		}

		public override long Seek( long offset, SeekOrigin origin ) {
			switch ( origin ) {
				case SeekOrigin.Begin:
					Position = offset;
					break;
				case SeekOrigin.Current:
					Position = Position + offset;
					break;
				case SeekOrigin.End:
					Position = Length - offset; // FIXME: is that right?
					break;
			}
			return Position;
		}

		public override void SetLength( long value ) {
			throw new Exception( "Cannot set length of partial streams." );
		}

		public override void Write( byte[] buffer, int offset, int count ) {
			int c = (int)Math.Min( PartialBytesLeft, count );
			BaseStream.Write( buffer, offset, c );
			CurrentPosition += c;
		}

		public override int ReadByte() {
			if ( PartialBytesLeft > 0 ) {
				int v = BaseStream.ReadByte();
				if ( v != -1 ) {
					++CurrentPosition;
				}
				return v;
			}
			return -1;
		}

		public override void WriteByte( byte value ) {
			if ( PartialBytesLeft > 0 ) {
				BaseStream.WriteByte( value );
				++CurrentPosition;
			}
		}

		public override DuplicatableStream Duplicate() {
			return new PartialStream( BaseStream, PartialStart, PartialLength );
		}
	}
}
