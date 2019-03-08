using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x4D8 bytes in 360, 0x4DC bytes in PS3 and later
	public class SaveDataBlockFieldSave {
		public DuplicatableStream Stream;

		public SaveDataBlockFieldSave( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
