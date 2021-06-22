using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PSP.GIM {
	interface ISection {
		uint GetPartSize();
		void Recalculate( int NewFilesize );
		byte[] Serialize();
	}
}
