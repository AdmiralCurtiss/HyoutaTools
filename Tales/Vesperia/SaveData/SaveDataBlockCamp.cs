using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x100 bytes in all versions
	public class SaveDataBlockCamp {
		public DuplicatableStream Stream;

		public SaveDataBlockCamp( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
