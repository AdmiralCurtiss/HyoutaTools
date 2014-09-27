using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.ShopData {
	public class ShopData {
		public ShopData( String filename, uint shopStart, uint shopCount, uint itemStart, uint itemCount ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, shopStart, shopCount, itemStart, itemCount ) ) {
					throw new Exception( "Loading ShopData failed!" );
				}
			}
		}

		public ShopData( Stream stream, uint shopStart, uint shopCount, uint itemStart, uint itemCount ) {
			if ( !LoadFile( stream, shopStart, shopCount, itemStart, itemCount ) ) {
				throw new Exception( "Loading ShopData failed!" );
			}
		}

		private bool LoadFile( Stream stream, uint shopStart, uint shopCount, uint itemStart, uint itemCount ) {
			return true;
		}
	}
}
