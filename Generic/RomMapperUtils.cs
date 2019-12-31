using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Generic {
	public static class RomMapperUtils {
		public static uint MapRamToRom( this IRomMapper mapper, uint ramAddress ) {
			uint v;
			if ( mapper.TryMapRamToRom( ramAddress, out v ) ) {
				return v;
			}
			throw new Exception( "Address not mappable." );
		}

		public static uint MapRomToRam( this IRomMapper mapper, uint romAddress ) {
			uint v;
			if ( mapper.TryMapRomToRam( romAddress, out v ) ) {
				return v;
			}
			throw new Exception( "Address not mappable." );

		}
	}
}
