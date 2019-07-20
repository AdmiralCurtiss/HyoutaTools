using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public static class FileSectionFactory {
		public static IFileSection ParseNextSection( System.IO.Stream stream ) {
			switch ( stream.PeekUInt32().SwapEndian() ) {
				case 0x43464C44: return new FileSections.CFLD( stream );
				case 0x44415441: return new FileSections.DATA( stream );
				case 0x4D455320: return new FileSections.MES( stream );
				case 0x4E414D45: return new FileSections.NAME( stream );
				case 0x5441424C: return new FileSections.TABL( stream );
				default: throw new Exception( "Unknown file section '" + Encoding.ASCII.GetString( BitConverter.GetBytes( stream.PeekUInt32() ) ) + "'." );
			}
		}
	}
}
