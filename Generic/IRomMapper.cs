using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Generic {
	public interface IRomMapper {
		bool TryMapRamToRom( uint ramAddress, out uint value );
		bool TryMapRomToRam( uint romAddress, out uint value );
	}
}
