using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// short header, used for save menu on non-PS3 versions to display basic info about save
	public class SaveMenuBlock {
		public Streams.DuplicatableStream Stream;

		public SaveMenuBlock( Streams.DuplicatableStream stream ) {
			Stream = stream.Duplicate();
		}
	}
}
