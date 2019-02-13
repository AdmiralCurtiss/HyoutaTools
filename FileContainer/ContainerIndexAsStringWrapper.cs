using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	public class ContainerIndexAsStringWrapper : ContainerBase {
		private IContainer Container;

		public ContainerIndexAsStringWrapper( IContainer container ) {
			Container = container;
		}

		public override void Dispose() {
			Container.Dispose();
		}

		public override INode GetChildByIndex( long index ) {
			string name = index.ToString();
			INode n = GetChildByName( name );
			if ( n != null ) {
				return n;
			}
			IEnumerable<string> names = GetChildNames();
			string probablyName = names.Where( x => x.StartsWith( name + "." ) ).FirstOrDefault();
			if ( probablyName != null ) {
				return GetChildByName( probablyName );
			}
			return null;
		}

		public override INode GetChildByName( string name ) {
			return Container.GetChildByName( name );
		}

		public override IEnumerable<string> GetChildNames() {
			return Container.GetChildNames();
		}
	}
}
