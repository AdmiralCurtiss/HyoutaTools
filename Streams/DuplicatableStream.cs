using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Streams {
	public abstract class DuplicatableStream : System.IO.Stream {
		public abstract DuplicatableStream Duplicate();
	}
}
