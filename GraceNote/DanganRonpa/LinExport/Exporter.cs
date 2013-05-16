using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.Lin;

namespace HyoutaTools.GraceNote.DanganRonpa.LinExport {
	public static class Exporter {
		public static int Export( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: text.lin.orig text.lin.new database [alignment (default 1024)] [LIN version (default 1)] [-refreshNames] [-refreshCode]" );
				return -1;
			}

			int Alignment;
			if ( !( args.Count >= 4 && Int32.TryParse( args[3], out Alignment ) ) ) {
				Alignment = 1024;
			}
			uint Version;
			if ( !( args.Count >= 5 && UInt32.TryParse( args[4], out Version ) ) ) {
				Version = 1;
			}
			HyoutaTools.DanganRonpa.DanganUtil.GameVersion = Version;
			return LinExport.Exporter.Export( args[0], args[1], args[2], Alignment, args.Contains( "-refreshNames" ), args.Contains( "-refreshCode" ) );
		}
		public static int Export( String InFilename, String OutFilename, String DB, int Alignment, bool RefreshNames, bool RefreshCode ) {
			LIN lin;
			try {
				lin = new LIN( InFilename );
				if ( RefreshNames ) { lin.ReinsertNamesIntoDatabase( "Data Source=" + DB ); }
				if ( RefreshCode ) { lin.ReinsertCodeIntoDatabase( "Data Source=" + DB ); }
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
