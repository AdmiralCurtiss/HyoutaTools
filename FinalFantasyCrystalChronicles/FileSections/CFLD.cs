using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public class CFLD : IFileSection {
		public List<IFileSection> Subsections;

		public CFLD( System.IO.Stream stream ) : base( stream ) {
			if ( SectionIdentifierHumanReadable != "CFLD" ) {
				throw new System.Exception( "Attempted to parse a 'CFLD' section with data from a '" + SectionIdentifierHumanReadable + "' section." );
			}

			Subsections = new List<IFileSection>();

			long PositionAtBeginning = stream.Position;
			stream.ReadAlign( 0x10 );
			while ( stream.Position < PositionAtBeginning + _SectionSize ) {
				if ( stream.PeekUInt32() == 0x00000000 ) {
					stream.DiscardBytes( 0x10 );
					continue;
				}
				IFileSection s = FileSectionFactory.ParseNextSection( stream );
				Subsections.Add( s );
				stream.ReadAlign( 0x10 );
			}
			stream.ReadAlign( 0x10 );
		}
	}
}
