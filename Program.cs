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
					case "DanganRonpa.Font.Viewer": return DanganRonpa.Font.Viewer.Program.Execute( ProgramArguments.ToArray() );
					case "DanganRonpa.Nonstop.Viewer": return DanganRonpa.Nonstop.RunNonstopForm.Execute();
					case "DanganRonpa.Pak.Extract": return DanganRonpa.Pak.Program.ExecuteExtract( ProgramArguments.ToArray() );
					case "DanganRonpa.Pak.Pack": return DanganRonpa.Pak.Program.ExecutePack( ProgramArguments.ToArray() );
					case "DanganRonpa.umdimagedat": return DanganRonpa.umdimagedat.umdimagedat.Execute( ProgramArguments.ToArray() );
					case "Generic.BlockCopy": return Generic.BlockCopy.BlockCopy.Execute( ProgramArguments.ToArray() );
					case "Generic.ByteHotfix": return Generic.ByteHotfix.ByteHotfix.Execute( ProgramArguments.ToArray() );
					case "GraceNote.DanganRonpa.LinImport": return GraceNote.DanganRonpa.LinImport.Importer.Import( ProgramArguments.ToArray() );
					case "GraceNote.DanganRonpa.LinExport": return GraceNote.DanganRonpa.LinExport.Exporter.Export( ProgramArguments.ToArray() );
					case "GraceNote.DanganRonpa.NonstopExistingDatabaseImport.Auto": return GraceNote.DanganRonpa.NonstopExistingDatabaseImport.Importer.AutoImport();
					case "GraceNote.DanganRonpa.LinLegacyTool": return GraceNote.DanganRonpa.LinImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.DanganRonpa.PakTextExport": return GraceNote.DanganRonpa.PakTextExport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.DanganRonpa.PakTextImport": return GraceNote.DanganRonpa.PakTextImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.DumpDatabase": return GraceNote.DumpDatabase.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.FindEarliestGracesJapaneseEntry": return GraceNote.FindEarliestGracesJapaneseEntry.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.LuxPainEvtExport": return GraceNote.LuxPainEvtExport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.LuxPainEvtImport": return GraceNote.LuxPainEvtImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Trophy.TropSfmExport": return GraceNote.Trophy.TropSfmExport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Trophy.TropSfmImport": return GraceNote.Trophy.TropSfmImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Vesperia.ScfombinImport": return GraceNote.Vesperia.ScfombinImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Vesperia.StringDicExport": return GraceNote.Vesperia.StringDicExport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Vesperia.To8chtxExport": return GraceNote.Vesperia.To8chtxExport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Vesperia.To8chtxImport": return GraceNote.Vesperia.To8chtxImport.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.Vesperia.VVoicesGenerate": return GraceNote.Vesperia.VVoicesGenerate.Program.Execute( ProgramArguments.ToArray() );
					case "GraceNote.XilliaScriptFileDump": return GraceNote.XilliaScriptFileDump.Program.Execute( ProgramArguments.ToArray() );
					case "Other.AutoExtract": return Other.AutoExtract.Program.Execute();
					case "Other.GoldenSunDarkDawnMsgExtract": return Other.GoldenSunDarkDawnMsgExtract.Program.Execute( ProgramArguments.ToArray() );
					case "Other.InvokeGimConv": return Other.InvokeGimConv.Program.Execute( ProgramArguments.ToArray() );
					case "Other.NisPakEx": return Other.NisPakEx.Program.Execute( ProgramArguments.ToArray() );
					case "Other.NitroidDataBinEx": return Other.NitroidDataBinEx.Program.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.Font.Viewer": return Tales.Vesperia.Font.Viewer.Program.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.Credits.Viewer": return Tales.Vesperia.Credits.RunCreditsViewer.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.ItemDat.Viewer": return Tales.Vesperia.ItemDat.RunItemViewer.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.MapList": return Tales.Vesperia.MapList.Program.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.SpkdUnpack": return Tales.Vesperia.SpkdUnpack.Program.Execute( ProgramArguments.ToArray() );
					case "Tales.Vesperia.TownMap.Viewer": return Tales.Vesperia.TownMap.Viewer.Program.Execute();
					case "Tales.Xillia.TldatExtract": return Tales.Xillia.TldatExtract.Program.Execute( ProgramArguments.ToArray() );
					case "Tales.tlzc": return Tales.tlzc.tlzcmain.Execute( ProgramArguments.ToArray() );
					case "Other.Xbox360.Rebundler": return Other.Xbox360.Rebundler.Rebundler.Rebundle( ProgramArguments.ToArray() );
					case "Generic.DbTextReplace": return Generic.DbTextReplace.Replacement.Replace( ProgramArguments.ToArray() );
					case "Other.PSP.GIM.LayerSplitter": return Other.PSP.GIM.LayerSplitter.Splitter.Split( ProgramArguments.ToArray() );
					default: PrintUsage(); break;
				}

			} else {
				PrintUsage();
			}
			return -1;
		}

		private static void PrintUsage() {
			Console.WriteLine( "DanganRonpa.Font.Viewer" );
			Console.WriteLine( "DanganRonpa.Nonstop.Viewer" );
			Console.WriteLine( "DanganRonpa.Pak.Extract" );
			Console.WriteLine( "DanganRonpa.Pak.Pack" );
			Console.WriteLine( "DanganRonpa.umdimagedat" );
			Console.WriteLine( "Generic.BlockCopy" );
			Console.WriteLine( "Generic.ByteHotfix" );
			Console.WriteLine( "GraceNote.DanganRonpa.LinExport" );
			Console.WriteLine( "GraceNote.DanganRonpa.LinImport" );
			Console.WriteLine( "GraceNote.DanganRonpa.PakTextExport" );
			Console.WriteLine( "GraceNote.DanganRonpa.PakTextImport" );
			Console.WriteLine( "GraceNote.DumpDatabase" );
			Console.WriteLine( "GraceNote.FindEarliestGracesJapaneseEntry" );
			Console.WriteLine( "GraceNote.LuxPainEvtExport" );
			Console.WriteLine( "GraceNote.LuxPainEvtImport" );
			Console.WriteLine( "GraceNote.Trophy.TropSfmExport" );
			Console.WriteLine( "GraceNote.Trophy.TropSfmImport" );
			Console.WriteLine( "GraceNote.Vesperia.ScfombinImport" );
			Console.WriteLine( "GraceNote.Vesperia.StringDicExport" );
			Console.WriteLine( "GraceNote.Vesperia.To8chtxExport" );
			Console.WriteLine( "GraceNote.Vesperia.To8chtxImport" );
			Console.WriteLine( "GraceNote.Vesperia.VVoicesGenerate" );
			Console.WriteLine( "GraceNote.XilliaScriptFileDump" );
			Console.WriteLine( "Other.AutoExtract" );
			Console.WriteLine( "Other.GoldenSunDarkDawnMsgExtract" );
			Console.WriteLine( "Other.InvokeGimConv" );
			Console.WriteLine( "Other.NisPakEx" );
			Console.WriteLine( "Other.NitroidDataBinEx" );
			Console.WriteLine( "Tales.Vesperia.Font.Viewer" );
			Console.WriteLine( "Tales.Vesperia.ItemDat.Viewer" );
			Console.WriteLine( "Tales.Vesperia.MapList" );
			Console.WriteLine( "Tales.Vesperia.SpkdUnpack" );
			Console.WriteLine( "Tales.Vesperia.TownMap.Viewer" );
			Console.WriteLine( "Tales.Xillia.TldatExtract" );
			Console.WriteLine( "Tales.tlzc" );
		}
	}
}
