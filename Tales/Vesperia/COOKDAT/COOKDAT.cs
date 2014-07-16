using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.COOKDAT {
	public class COOKDAT {
		public COOKDAT( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading COOKDAT failed!" );
				}
			}
		}

		public COOKDAT( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading COOKDAT failed!" );
			}
		}

		public List<Recipe> RecipeList;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown = stream.ReadUInt32().SwapEndian();
			uint recipeCount = stream.ReadUInt32().SwapEndian();

			RecipeList = new List<Recipe>( (int)recipeCount );
			for ( uint i = 0; i < recipeCount; ++i ) {
				Recipe r = new Recipe( stream );
				RecipeList.Add( r );
			}

			return true;
		}
	}
}
