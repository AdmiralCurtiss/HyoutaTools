using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.tlzc {
	public class TlzcDecompressor : IDecompressor {
		public CanDecompressAnswer CanDecompress( Stream stream ) {
			long pos = stream.Position;
			CanDecompressAnswer answer = stream.ReadAscii( 4 ) == "TLZC" ? CanDecompressAnswer.Yes : CanDecompressAnswer.No;
			stream.Position = pos;
			return answer;
		}

		public Stream Decompress( Stream input ) {
			byte[] data = new byte[input.Length];
			input.Read( data, 0, data.Length );
			return new Streams.DuplicatableByteArrayStream( TLZC.Decompress( data ) );
		}

		public string GetId() {
			return "TLZC";
		}
	}
}
