using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.ItemDat {
	public class ItemDat {
		public List<ItemDatSingle> items;

		public ItemDat( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Initialize( byte[] file ) {
			items = new List<ItemDatSingle>( file.Length / 0x2E4 );

			for ( int i = 0; i < file.Length; i += 0x2E4 ) {
				items.Add( new ItemDatSingle( i, file ) );
			}

		}
	}
}
