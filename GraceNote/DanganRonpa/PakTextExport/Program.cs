using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.PakText;

namespace HyoutaTools.GraceNote.DanganRonpa.PakTextExport {
	class Program {
		public static int Execute( string[] args ) {
			if ( args.Length != 2 ) {
				Console.WriteLine( "Usage: GN_DRM Outfile DB" );
				return -1;
			}

			// templateDB must contain:
			/***
			 * CREATE TABLE Text(ID int primary key, StringID int, english text, comment text, updated tinyint, status tinyint, PointerRef integer, IdentifyString text, IdentifyPointerRef integer);
			 ***/

			//*
			String Filename = args[0];
			String DB = args[1];
			//String GracesDB = args[2];
			//*/

			/*
			String Filename = @"c:\Users\Georg\Downloads\Xillia Script files\69753.SDBJPN";
			String NewDB = @"c:\Users\Georg\Downloads\Xillia Script files\X69753";
			String TemplateDB = @"c:\Users\Georg\Downloads\Xillia Script files\XTemplate";
			String GracesDB = @"c:\Users\Georg\Downloads\Xillia Script files\GracesJapanese";
			//*/

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

			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophyConf.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(true)));
			//System.IO.File.WriteAllBytes(@"C:\TROPHY\newTrophy.trp", Encoding.UTF8.GetBytes(TROPSFM.ExportTropSFM(false)));

			return 0;
		}
	}
}
