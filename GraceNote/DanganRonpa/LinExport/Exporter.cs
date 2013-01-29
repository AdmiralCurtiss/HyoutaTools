using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.Lin;

namespace HyoutaTools.GraceNote.DanganRonpa.LinExport {
	public static class Exporter {
		public static int Export( string[] args ) {
			if ( args.Length < 3 ) {
				Console.WriteLine( "Usage: text.lin.orig text.lin.new database [alignment (default 1024)]" );
				return -1;
			}

			int Alignment;
			if ( !( args.Length >= 4 && Int32.TryParse( args[3], out Alignment ) ) ) {
				Alignment = 1024;
			}
			return LinExport.Exporter.Export( args[0], args[1], args[2], Alignment );
		}
		public static int Export( String InFilename, String OutFilename, String DB, int Alignment ) {
			LIN lin;
			try {
				lin = new LIN( InFilename );
				lin.GetSQL( "Data Source=" + DB );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.Message );
				Console.WriteLine( "Failed loading text file!" );
				return -1;
			}
			byte[] newfile = lin.CreateFile( Alignment );
			System.IO.File.WriteAllBytes( OutFilename, newfile );
			return 0;
		}

	}
}
