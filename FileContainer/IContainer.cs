using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	public interface IContainer : INode {
		INode GetChildByIndex( long index );
		INode GetChildByName( string name );
		List<string> GetChildNames();
	}
}
