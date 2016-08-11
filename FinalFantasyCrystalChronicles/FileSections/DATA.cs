using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public class DATA : IFileSection {
		public DATA( System.IO.Stream stream ) : base( stream ) {
			// probably a generic binary blob, though I don't quite understand the format
			// just skip it if we encountered it
			stream.ReadAlign( 0x10 );
			stream.DiscardBytes( SectionSize );
		}
	}
}
