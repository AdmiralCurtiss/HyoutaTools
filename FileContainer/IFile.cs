using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	public interface IFile : INode {
		Streams.DuplicatableStream DataStream { get; }
	}
}
