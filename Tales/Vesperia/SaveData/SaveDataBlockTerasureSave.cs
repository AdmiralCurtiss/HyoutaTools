using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x145C bytes, 360 only
	public class SaveDataBlockTerasureSave {
		public DuplicatableStream Stream;

		public SaveDataBlockTerasureSave( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
