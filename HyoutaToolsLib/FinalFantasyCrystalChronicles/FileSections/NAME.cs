using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public class NAME : IFileSection {
		public string Filename;

		public NAME( System.IO.Stream stream ) : base( stream ) {
			stream.ReadAlign( 0x10 );
			Filename = stream.ReadAscii( (int)SectionSize ).TrimNull();
		}
	}
}
