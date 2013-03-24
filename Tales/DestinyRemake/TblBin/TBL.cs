using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.DestinyRemake.TblBin {
	// THIS DOESN'T APPEAR TO BE CORRECT
	class TblBinFileReference {
		public uint Start;
		public uint Unknown;

		public uint End;
	}

	class TBL {

		public List<TblBinFileReference> Refs;

		public void Load( byte[] Tbl ) {
			Refs = new List<TblBinFileReference>();

			for ( int i = 0; i < Tbl.Length - 8; i += 8 ) {
				TblBinFileReference r = new TblBinFileReference();
				r.Start = BitConverter.ToUInt32( Tbl, i );
				r.Unknown = BitConverter.ToUInt32( Tbl, i + 4 );
				r.End = BitConverter.ToUInt32( Tbl, i + 8 );
				Refs.Add( r );
			}
		}

		public void Extract( Stream Bin, string Outfolder ) {
			Directory.CreateDirectory( Outfolder );
			int count = 0;
			foreach ( TblBinFileReference r in Refs ) {
				FileStream fs = new FileStream( System.IO.Path.Combine( Outfolder, count.ToString( "D6" ) ), FileMode.Create );
				Bin.Position = r.Start;
				Util.CopyStream( Bin, fs, (int)( r.End - r.Start ) );
				fs.Close();
				++count;
			}
		}
	}
}
