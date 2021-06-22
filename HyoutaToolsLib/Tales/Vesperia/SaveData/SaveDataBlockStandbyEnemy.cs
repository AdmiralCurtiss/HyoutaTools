using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0xC28 bytes, only exists in PS3 and later
	public class SaveDataBlockStandbyEnemy {
		public DuplicatableStream Stream;

		public SaveDataBlockStandbyEnemy( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
