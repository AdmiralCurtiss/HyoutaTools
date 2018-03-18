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
			Console.WriteLine( "Usage: FontViewer fontimage fontinfo" );
		}

		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				PrintUsage();
				return -1;
			}

			string Filepath = args[1];
			string ImagePath = args[0];
			//string Filepath2 = args[2];


			FontViewer form;
			try {
				byte[] File = System.IO.File.ReadAllBytes( Filepath );

				DRFontInfo f = new DRFontInfo( File );
				//DRFontInfo f2 = new DRFontInfo( System.IO.File.ReadAllBytes( Filepath2 ) );
				//f.CopyInfoFrom( f2 );

				DRFontChar apos = f.GetCharViaCharacter( (ushort)'\'' );
				DRFontChar quot = f.GetCharViaCharacter( (ushort)'"' );

				DRFontChar fontch = new DRFontChar();
				fontch.Character = (ushort)'’';
				DRFontInfo.CopyCharInfo( apos, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'‘';
				DRFontInfo.CopyCharInfo( apos, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'‛';
				DRFontInfo.CopyCharInfo( apos, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'“';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'”';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'‟';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'〝';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'〞';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'＂';
				DRFontInfo.CopyCharInfo( quot, fontch );
				f.ImportExternalCharacter( fontch );

				fontch = new DRFontChar();
				fontch.Character = (ushort)'＇';
				DRFontInfo.CopyCharInfo( apos, fontch );
				f.ImportExternalCharacter( fontch );

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				form = new FontViewer( f, new System.Drawing.Bitmap( ImagePath ) );
				form.Filepath = Filepath;
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				PrintUsage();
				return -1;
			}

			Application.Run( form );

			return 0;
		}
	}
}
