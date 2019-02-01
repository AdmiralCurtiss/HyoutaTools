using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	public interface INode : IDisposable {
		bool IsFile { get; }
		bool IsContainer { get; }
		IFile AsFile { get; }
		IContainer AsContainer { get; }
	}
}
