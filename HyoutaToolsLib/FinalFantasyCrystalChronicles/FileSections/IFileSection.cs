using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.FinalFantasyCrystalChronicles.FileSections {
	public abstract class IFileSection {
		protected UInt32 _SectionIdentifier;
		public UInt32 SectionIdentifier {
			get { return _SectionIdentifier; }
		}

		protected UInt32 _SectionSize;
		public UInt32 SectionSize {
			get { return _SectionSize; }
		}

		public virtual string SectionIdentifierHumanReadable {
			get { return Encoding.ASCII.GetString( BitConverter.GetBytes( SectionIdentifier ) ); }
		}

		protected IFileSection( System.IO.Stream stream ) {
			_SectionIdentifier = stream.ReadUInt32();
			_SectionSize = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
		}
	}
}
