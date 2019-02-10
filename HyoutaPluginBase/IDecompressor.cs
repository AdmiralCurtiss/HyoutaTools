﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaPluginBase {
	public interface IDecompressor {
		// ask the decompressor whether it can decompress a given file
		// answer should be fast and depend on magic bytes in the header or similar
		// stream should appear unchanged from the outside, so reset the position to where you found it
		// if fast answer cannot be determined return 'Maybe'
		CanDecompressAnswer CanDecompress( System.IO.Stream stream );

		// decompress the given input and return a stream to the decompressed data
		// fancy decompressors may even decompress on the fly on access of the returned stream
		// (though it's not really recommended due to many algorithms assuming cheap random access)
		System.IO.Stream Decompress( System.IO.Stream input );
	}

	public enum CanDecompressAnswer { Yes, No, Maybe }
}
