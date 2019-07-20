using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x220 bytes, only in PS3 and later
	public class SaveDataBlockSoundTheater {
		public DuplicatableStream Stream;

		public SaveDataBlockSoundTheater( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}
	}
}
