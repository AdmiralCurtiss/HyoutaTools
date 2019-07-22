using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0xA0000 bytes, only in PS3 and up
	// 8 blocks of 0x14000 bytes each
	// probably contains the ghost data for the snowboard minigame
	// somehow I find it hilarious that this is most of the save file
	public class SaveDataBlockSnowBoard {
		public DuplicatableStream Stream;

		public SaveDataBlockSnowBoard( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
