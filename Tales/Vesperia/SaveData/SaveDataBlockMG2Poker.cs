using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x80 bytes in all versions of the game
	public class SaveDataBlockMG2Poker {
		public DuplicatableStream Stream;

		public SaveDataBlockMG2Poker( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
