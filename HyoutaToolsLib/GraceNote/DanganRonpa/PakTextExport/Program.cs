using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.PakText;

namespace HyoutaTools.GraceNote.DanganRonpa.PakTextExport {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: GN_DRM Outfile DB" );
				return -1;
			}

			// templateDB must contain:
			/***
			 * CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, IdentifyString text, IdentifyPointerRef integer);
			 ***/

			String Filename = args[0];
			String DB = args[1];

			PakText DRMF;
			try {
				DRMF = new PakText();
				DRMF.GetSQL( "Data Source=" + DB );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading menu file!" );
				return -1;
			}
			byte[] newfile = DRMF.CreateFile();
			System.IO.File.WriteAllBytes( Filename, newfile );

			return 0;
		}
	}
}
