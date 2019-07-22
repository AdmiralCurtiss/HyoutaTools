using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x24C bytes, only in PS3 and later, probably replaces TERASURE_SAVE?
	public class SaveDataBlockTreasureSaveData {
		public DuplicatableStream Stream;

		public SaveDataBlockTreasureSaveData( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
