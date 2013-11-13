using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HyoutaTools.Tales.Vesperia.Font.Viewer {
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
			/*
			FontDisplay
			-fontinfofile tov.elf
			-fontinfofiletype elf/fontinfo
			-fontpngdir FontTex
			-textfile text.txt
			-mode gui/png
			-font FONTTEX10
			-fontblock 0
			-outfile text.png
			*/

			string Filepath = "ffinfo.bin";
			int FontInfoOffset = 0;
			String Textfile = null;
			ProgramMode Mode = ProgramMode.GUI;
			String Font = "FONTTEX10.TXV";
			int Fontblock = 0;
			String Outfile = "out.png";
			bool BoxByBox = false;
			bool DialogueBoxColor = false;
			String FontPngDir = @"FontTex";

			try {
				for ( int i = 0; i < args.Count; i++ ) {
					switch ( args[i].ToLowerInvariant() ) {
						case "-fontinfofile":
							Filepath = args[++i];
							break;
						case "-fontinfofiletype":
							switch ( args[++i].ToLowerInvariant() ) {
								case "elf":
									FontInfoOffset = 0x00720860;
									break;
								case "fontinfo":
									FontInfoOffset = 0;
									break;
							}
							break;
						case "-textfile":
							Textfile = args[++i];
							break;
						case "-fontpngdir":
							FontPngDir = args[++i];
							break;
						case "-mode":
							switch ( args[++i].ToLowerInvariant() ) {
								case "gui":
									Mode = ProgramMode.GUI;
									break;
								case "png":
									Mode = ProgramMode.png;
									break;
							}
							break;
						case "-font":
							Font = args[++i];
							break;
						case "-fontblock":
							Fontblock = Int32.Parse( args[++i] );
							break;
						case "-outfile":
							Outfile = args[++i];
							break;
						case "-boxbybox":
							BoxByBox = true;
							break;
						case "-dialoguebubble":
							DialogueBoxColor = true;
							break;
					}
				}
			} catch ( IndexOutOfRangeException ) {
				PrintUsage();
				return -1;
			}



			try {
				byte[] File = System.IO.File.ReadAllBytes( Filepath );

				FontInfo[] f = new FontInfo[6];
				f[0] = new FontInfo( File, FontInfoOffset );
				f[1] = new FontInfo( File, FontInfoOffset + 0x880 );
				f[2] = new FontInfo( File, FontInfoOffset + 0x880 * 2 );
				f[3] = new FontInfo( File, FontInfoOffset + 0x880 * 3 );
				f[4] = new FontInfo( File, FontInfoOffset + 0x880 * 4 );
				f[5] = new FontInfo( File, FontInfoOffset + 0x880 * 5 );

				String[] TextLines = null;
				if ( Textfile != null ) {
					TextLines = System.IO.File.ReadAllLines( Textfile );
				}

				Application.EnableVisualStyles();
				Application.SetCompatibleTextRenderingDefault( false );
				FontViewer form = new FontViewer( f, Font, FontPngDir, Fontblock, TextLines, BoxByBox, DialogueBoxColor );
				form.Filepath = Filepath;
				form.FontInfoOffset = FontInfoOffset;

				if ( Mode == ProgramMode.GUI ) {
					Application.Run( form );
				} else if ( Mode == ProgramMode.png ) {
					form.SaveAsPng( Outfile );
				}
			} catch ( Exception ex ) {
				Console.WriteLine( ex.ToString() );
				PrintUsage();
				return -1;
			}

			return 0;
		}
	}
}
