using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x118 bytes in 360, 0x120 bytes in PS3, 0x124 bytes in PC
	public class SaveDataBlockCustomData {
		public DuplicatableStream Stream;

		public SaveDataBlockCustomData( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
