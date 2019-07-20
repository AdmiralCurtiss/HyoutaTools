using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x1300 bytes in all versions
	// probably contains story and sidequest progression flags?
	public class SaveDataBlockScenario {
		public DuplicatableStream Stream;

		public SaveDataBlockScenario( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
