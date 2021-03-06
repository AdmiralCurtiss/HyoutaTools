﻿using HyoutaTools.Tales.Vesperia.Font;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace HyoutaLibGUI.Tales.Vesperia.Font.Viewer {
	enum ProgramMode {
		None,
		GUI,
		png
	}

	static class Program {
		static void PrintUsage() {
			MessageBox.Show(
				  "Available arguments:"
				+ "\n -fontinfofile ffinfo.bin/tov.elf"
				+ "\n -fontinfofiletype fontinfo/elf"
				+ "\n -textfile text.txt"
				+ "\n -mode gui/png"
				+ "\n -font FONTTEX10.TXV"
				+ "\n -fontblock 0"
				+ "\n -outfile text.png"
				+ "\n -boxbybox"
				+ "\n -dialoguebubble"
			);
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

				FontViewer form = new FontViewer( f, Font, FontPngDir, Fontblock, TextLines, BoxByBox, DialogueBoxColor );
				form.Filepath = Filepath;
				form.FontInfoOffset = FontInfoOffset;

				if ( Mode == ProgramMode.GUI ) {
					form.Show();
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
