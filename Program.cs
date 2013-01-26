using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools {
	class Program {
		static int Main( string[] args ) {
			if ( args.Length > 0 ) {
				string ProgramName = args[0];
				List<string> ProgramArguments = new List<string>( args.Length - 1 );
				for ( int i = 1; i < args.Length; ++i ) {
					ProgramArguments.Add( args[i] );
				}

				switch ( ProgramName ) {
					case "DanganRonpa.umdimagedat": return DanganRonpa.umdimagedat.umdimagedat.Execute( ProgramArguments.ToArray() );
					case "Generic.BlockCopy": return Generic.BlockCopy.BlockCopy.Execute( ProgramArguments.ToArray() );
					case "Generic.ByteHotfix": return Generic.ByteHotfix.ByteHotfix.Execute( ProgramArguments.ToArray() );
					case "Tales.tlzc": return Tales.tlzc.tlzcmain.Execute( ProgramArguments.ToArray() );
					// case "GraceNote.LuxPainEvtExport":
					// case "GraceNote.LuxPainEvtImport":
					// case "GraceNote.XilliaScriptFileDump":
					// case "Other.GoldenSunDarkDawnMsgExtract":
					// case "GraceNote.Vesperia.VVoicesGenerate":
					// case "Tales.Xillia.TldatExtract":
					// case "Tales.Vesperia.MapList":
					// case "Other.NisPakEx":
					// case "Tales.Vesperia.SpkdUnpack":
					// case "GraceNote.Vesperia.ScfombinImport":
					// case "Other.NitroidDataBinEx":
					// case "GraceNote.Vesperia.StringDicExport":
					// case "DanganRonpa.Pak.Extract":
					// case "DanganRonpa.Pak.Pack":
					// case "Other.AutoExtract":
					// case "GraceNote.FindEarliestGracesJapaneseEntry":
					// case "DanganRonpa.Nonstop.Viewer":
					// case "GraceNote.DumpDatabase":
					// case "GraceNote.Trophy.TropSfmImport":
					// case "GraceNote.Trophy.TropSfmExport":
					// case "Other.InvokeGimConv";
					// case "Tales.Vesperia.ItemDat.Viewer";
					// case "GraceNote.To8chtxExport";
					// case "GraceNote.To8chtxImport";
					// case "Tales.Vesperia.Font.Viewer";
					// case "DanganRonpa.Font.Viewer;
					// case "GraceNote.DanganRonpa.LinImport";
					// case "GraceNote.DanganRonpa.PakTextExport";
					// case "GraceNote.DanganRonpa.PakTextImport";
					// case "Tales.Vesperia.TownMap.Viewer";
				}

			}
			return -1;
		}
	}
}
