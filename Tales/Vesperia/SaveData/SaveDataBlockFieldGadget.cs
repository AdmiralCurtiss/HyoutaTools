using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x200 bytes in all versions
	public class SaveDataBlockFieldGadget {
		public DuplicatableStream Stream;

		public SaveDataBlockFieldGadget( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
