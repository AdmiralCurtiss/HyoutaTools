using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x100 bytes in all versions
	public class SaveDataBlockFieldCar {
		public DuplicatableStream Stream;

		public SaveDataBlockFieldCar( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
