using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.Xbox360.Rebundler {
	public static class Rebundler {
		public static int Rebundle( List<string> args ) {
			string filename = args[0];

			// first, call bundler on the RDF, so we get a new XPR with the right header and stuff
			Util.RunProgram( "Bundler", filename + ".rdf", true, true );

			// then, copy over the swizzled data into the generated XPR
			int XprTextureLocation = 0x80C;
			byte[] xpr = System.IO.File.ReadAllBytes( filename + ".xpr" ); // generated XPR
			byte[] dat = System.IO.File.ReadAllBytes( filename + ".dat" ); // actual but swizzled data
			Util.CopyByteArrayPart( dat, 0, xpr, XprTextureLocation, xpr.Length - XprTextureLocation);
			System.IO.File.WriteAllBytes( filename + ".xpr", xpr );

			// and finally, unbundle the new XPR
			Util.RunProgram( "UnBundler", filename + ".xpr", true, true );
			
	
			return 0;
		}
	}
}
