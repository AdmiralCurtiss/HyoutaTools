using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HyoutaTools.DanganRonpa.Font.Viewer {
	enum ProgramMode {
		None,
		GUI,
		png
	}

	static class Program {
		static void PrintUsage() {
			Console.WriteLine( "FontDisplay" );
			Console.WriteLine( " -fontinfofile ffinfo.bin/tov.elf" );
			Console.WriteLine( " -fontinfofiletype fontinfo/elf" );
			Console.WriteLine( " -textfile text.txt" );
			Console.WriteLine( " -mode gui/png" );
			Console.WriteLine( " -font FONTTEX10.TXV" );
			Console.WriteLine( " -fontblock 0" );
			Console.WriteLine( " -outfile text.png" );
			Console.WriteLine( " -boxbybox" );
			Console.WriteLine( " -dialoguebubble" );
		}

		static void Execute( string[] args ) {
			Util.Path = "../../umdimage2-0169/0001";
			String ImagePath = "../../umdimage2-0169/0000.bmp";


			try {
				byte[] File = System.IO.File.ReadAllBytes( Util.Path );

				DRFontInfo f = new DRFontInfo( File );

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				Form1 form = new Form1( f, new System.Drawing.Bitmap( ImagePath ) );
				Application.Run( form );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				PrintUsage();
				return;
			}
		}
	}
}
