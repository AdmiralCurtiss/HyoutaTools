using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.DanganRonpa.Nonstop {
	public class Nonstop {
		public List<NonstopSingle> items;

		public Nonstop( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Initialize( byte[] file ) {
			int count = BitConverter.ToUInt16( file, 0x02 );
			items = new List<NonstopSingle>( count );

			for ( int i = 0; i < count; ++i ) {
				items.Add( new NonstopSingle( 0x04 + i * 0x3C, file ) );
			}

		}
	}
}
