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

		public static int Execute( List<string> args ) {
			string Filepath = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfRebuild\assets\fontdata1";
			string ImagePath = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfRebuild\assets\fontdata0";


			try {
				byte[] File = System.IO.File.ReadAllBytes( Filepath );

				DRFontInfo f = new DRFontInfo( File );

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				FontViewer form = new FontViewer( f, new System.Drawing.Bitmap( ImagePath ) );
				form.Filepath = Filepath;
				Application.Run( form );
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				PrintUsage();
				return -1;
			}

			return 0;
		}
	}
}
