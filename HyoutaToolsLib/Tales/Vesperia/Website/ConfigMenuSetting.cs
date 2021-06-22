using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.Website {
	public class ConfigMenuSetting {
		public uint NameStringDicId { get; private set; }
		public uint DescStringDicId { get; private set; }
		public uint[] OptionsStringDicIds { get; private set; }

		public ConfigMenuSetting( uint idName, uint idDesc, uint option1 = 0, uint option2 = 0, uint option3 = 0, uint option4 = 0 ) {
			NameStringDicId = idName;
			DescStringDicId = idDesc;
			OptionsStringDicIds = new uint[4];
			OptionsStringDicIds[0] = option1;
			OptionsStringDicIds[1] = option2;
			OptionsStringDicIds[2] = option3;
			OptionsStringDicIds[3] = option4;
		}
	}
}
