using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Generic.AddDDSHeader {
	public class Program {
		public static int Execute( List<string> args ) {
			string filename = args[0];
			uint width = UInt32.Parse( args[1] );
			uint height = UInt32.Parse( args[2] );
			byte[] data = System.IO.File.ReadAllBytes( filename );

			string path = filename + ".dds";
			using ( var fs = new System.IO.FileStream( path, System.IO.FileMode.Create ) ) {
				fs.Write( Textures.DDSHeader.Generate( width, height, 1, format: Textures.TextureFormat.DXT5 ).ToBytes() );
				fs.Write( data );
				fs.Close();
			}

			return 0;
		}
	}
}
