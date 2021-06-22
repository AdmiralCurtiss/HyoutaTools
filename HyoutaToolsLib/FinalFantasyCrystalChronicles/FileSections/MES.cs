using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public class MES : IFileSection {
		public List<string> Messages;

		public MES( System.IO.Stream stream ) : base( stream ) {
			uint count = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
			stream.ReadAlign( 0x10 );

			Messages = new List<string>( (int)count );
			for ( uint i = 0; i < count; ++i ) {
				Messages.Add( TextConverter.ReadToNulltermAndDecode( stream ) );
			}
		}
	}
}
