using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Streams {
	public abstract class DuplicatableStream : System.IO.Stream {
		public abstract DuplicatableStream Duplicate();

		// use these two to keep open file handles in check
		// no need to keep thousands of file handles open if we're only ever accessing a handful at once...
		public abstract void ReStart(); // open or reset underlying stream; call before accessing data
		public abstract void End(); // signify that we're done with accessing the data for now
	}
}
