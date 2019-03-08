using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x800 bytes in all versions
	public class SaveDataBlockFieldArea {
		public DuplicatableStream Stream;

		public SaveDataBlockFieldArea( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
