using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.COOKDAT {
	public class COOKDAT {
		public COOKDAT( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading COOKDAT failed!" );
				}
			}
		}

		public COOKDAT( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading COOKDAT failed!" );
			}
		}

		public List<Recipe> RecipeList;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown = stream.ReadUInt32().FromEndian( endian );
			uint recipeCount = stream.ReadUInt32().FromEndian( endian );

			RecipeList = new List<Recipe>( (int)recipeCount );
			for ( uint i = 0; i < recipeCount; ++i ) {
				Recipe r = new Recipe( stream, endian );
				RecipeList.Add( r );
			}

			return true;
		}
	}
}
