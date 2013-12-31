using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools {

	class ProgramName : IEquatable<string>, IComparable<ProgramName> {
		public string Name;
		public string Shortcut;
		public ProgramName( string Name, string Shortcut ) { this.Name = Name; this.Shortcut = Shortcut; }
		public bool Equals( string other ) { return Name.ToLowerInvariant() == other.ToLowerInvariant() || Shortcut.ToLowerInvariant() == other.ToLowerInvariant(); }
		public int CompareTo( ProgramName other ) { return Name.CompareTo( other.Name ); }
	}

	class Program {

		public delegate int ExecuteProgramDelegate( List<string> args );


		public static System.Collections.Generic.List<KeyValuePair<ProgramName, ExecuteProgramDelegate>> ProgramList = new List<KeyValuePair<ProgramName, ExecuteProgramDelegate>>() {
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Font.Viewer",                 "DrFont"      ),  DanganRonpa.Font.Viewer.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Nonstop.Viewer",              "DrNonstop"   ),  DanganRonpa.Nonstop.RunNonstopForm.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Pak.Extract",                 "DrPakE"      ),  DanganRonpa.Pak.Program.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.Pak.Pack",                    "DrPakP"      ),  DanganRonpa.Pak.Program.ExecutePack) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "DanganRonpa.umdimagedat",                 "DrUmdImg"    ),  DanganRonpa.umdimagedat.umdimagedat.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Generic.BlockCopy",                       "BlockCopy"   ),  Generic.BlockCopy.BlockCopy.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Generic.ByteHotfix",                      "ByteFix"     ),  Generic.ByteHotfix.ByteHotfix.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Generic.ByteSwap",                        "ByteSwap"    ),  Generic.ByteSwap.ByteSwap.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.LinImport",         "GNDRlinIm"   ),  GraceNote.DanganRonpa.LinImport.Importer.Import) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.LinExport",         "GNDRlinEx"   ),  GraceNote.DanganRonpa.LinExport.Exporter.Export) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.NonstopMetaIntoDb", "-"           ),  GraceNote.DanganRonpa.NonstopExistingDatabaseImport.Importer.AutoImport) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.LinLegacyTool",     "-"           ),  GraceNote.DanganRonpa.LinImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.PakTextExport",     "GNDRpakEx"   ),  GraceNote.DanganRonpa.PakTextExport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.PakTextImport",     "GNDRpakIm"   ),  GraceNote.DanganRonpa.PakTextImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DumpDatabase",                  "GNdump"      ),  GraceNote.DumpDatabase.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.FindEarliestGracesJapaneseId",  "-"           ),  GraceNote.FindEarliestGracesJapaneseEntry.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LuxPainEvtExport",              "-"           ),  GraceNote.LuxPainEvtExport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LuxPainEvtImport",              "-"           ),  GraceNote.LuxPainEvtImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Trophy.TropSfmExport",          "GNtrophyex"  ),  GraceNote.Trophy.TropSfmExport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Trophy.TropSfmImport",          "GNtrophyim"  ),  GraceNote.Trophy.TropSfmImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.ScfombinImport",       "GNToVscfom"  ),  GraceNote.Vesperia.ScfombinImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.StringDicExport",      "GNToVdicex"  ),  GraceNote.Vesperia.StringDicExport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.StringDicImport",      "GNToVdicim"  ),  GraceNote.Vesperia.StringDicImport.Importer.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.To8chtxExport",        "GNToVchatex" ),  GraceNote.Vesperia.To8chtxExport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.To8chtxImport",        "GNToVchatim" ),  GraceNote.Vesperia.To8chtxImport.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Vesperia.VVoicesGenerate",      "-"           ),  GraceNote.Vesperia.VVoicesGenerate.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.XilliaScriptFileDump",          "-"           ),  GraceNote.XilliaScriptFileDump.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.AutoExtract",                       "autoex"      ),  Other.AutoExtract.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.FileToFolderRename",                "-"           ),  Other.AutoExtract.FileToFolderRename.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Generic.ArbitraryTextDump",               "-"           ),  Generic.ArbitraryTextDump.Dump.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.GoldenSunDarkDawnMsgExtract",       "-"           ),  Other.GoldenSunDarkDawnMsgExtract.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.InvokeGimConv",                     "-"           ),  Other.InvokeGimConv.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.NisPakEx",                          "-"           ),  Other.NisPakEx.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.NitroidDataBinEx",                  "-"           ),  Other.NitroidDataBinEx.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.Font.Viewer",              "ToVfont"     ),  Tales.Vesperia.Font.Viewer.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.Credits.Viewer",           "ToVcredits"  ),  Tales.Vesperia.Credits.RunCreditsViewer.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.ItemDat.Viewer",           "ToVitemdat"  ),  Tales.Vesperia.ItemDat.RunItemViewer.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.MapList",                  "ToVmaplist"  ),  Tales.Vesperia.MapList.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.SpkdUnpack",               "ToVspkd"     ),  Tales.Vesperia.SpkdUnpack.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.TownMap.Viewer",           "ToVtownmap"  ),  Tales.Vesperia.TownMap.Viewer.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Vesperia.FPS4.Extract",             "ToVfps4e"    ),  Tales.Vesperia.FPS4.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Abyss.FPS2.Extract",                "TotAfps2e"   ),  Tales.Abyss.FPS2.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Abyss.FPS3.Extract",                "TotAfps3e"   ),  Tales.Abyss.FPS3.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Abyss.SB7.DumpText",                "TotAsp7dump" ),  Tales.Abyss.SB7.DumpText.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.TrailsInTheSkyScenarioDump",        "TrailsDump"  ),  Other.TrailsInTheSkyScenarioDump.DumpText.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Xillia.TldatExtract",               "ToXtldat"    ),  Tales.Xillia.TldatExtract.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.tlzc",                              "tlzc"        ),  Tales.tlzc.tlzcmain.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.Abyss.PKF.Split",                   "TotApkfspl"  ),  Tales.Abyss.PKF.Split.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.Xbox360.Rebundler",                 "-"           ),  Other.Xbox360.Rebundler.Rebundler.Rebundle) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Generic.DbTextReplace",                   "-"           ),  Generic.DbTextReplace.Replacement.Replace) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.PSP.GIM.LayerSplitter",             "gimSplit"    ),  Other.PSP.GIM.LayerSplitter.Splitter.Split) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.PSP.GIM.HomogenizePalette",         "gimSamePal"  ),  Other.PSP.GIM.HomogenizePalette.Program.Homogenize) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.DestinyRemake.TblBin.Extract",      "-"           ),  Tales.DestinyRemake.TblBin.Execute.Extract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Tales.DestinyRemake.MglkExtract.Extract", "-"           ),  Tales.DestinyRemake.MglkExtract.Execute.Extract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.N64.OoTSaveEditor",                 "OoTsaveedit" ),  Other.N64.OoTSaveEditor.Program.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.GoogleTranslate",               "GNtranslate" ),  GraceNote.GoogleTranslate.TranslateDatabase.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.DanganRonpa.AutoFormat",        "-"           ),  GraceNote.DanganRonpa.AutoFormatting.AutoFormat.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.bscrImport",         "-"           ),  GraceNote.LastRanker.bscrImport.Import) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.bscrExport",         "-"           ),  GraceNote.LastRanker.bscrExport.Export) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.SCMPextract",                  "-"           ),  LastRanker.SCMP.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.SCMPpack",                     "-"           ),  LastRanker.SCMP.ExecutePack) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.CZAAextract",                  "-"           ),  LastRanker.CZAA.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.CZAAcompress",                 "-"           ),  LastRanker.CZAA.ExecutePack) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.NPKextract",                   "-"           ),  LastRanker.NPK.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.NPKpack",                      "-"           ),  LastRanker.NPK.ExecutePack) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.RTDPextract",                  "-"           ),  LastRanker.RTDP.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.RTDPpack",                     "-"           ),  LastRanker.RTDP.ExecutePack) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.StringImport",       "-"           ),  GraceNote.LastRanker.StringImport.Import) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.StringExport",       "-"           ),  GraceNote.LastRanker.StringExport.Export) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.GZip.Extract",                      "-"           ),  Other.GZip.GZipHandler.ExecuteExtract) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "Other.GZip.Compress",                     "-"           ),  Other.GZip.GZipHandler.ExecuteCompress) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "LastRanker.PtmdToPng",                    "-"           ),  LastRanker.PTMD.PtmdToPng) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.SscrImport",         "-"           ),  GraceNote.LastRanker.SscrImport.Import) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.LastRanker.SscrExport",         "-"           ),  GraceNote.LastRanker.SscrExport.Export) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Narisokonai.scrImport",         "-"           ),  GraceNote.Narisokonai.scrImport.Import) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.Narisokonai.scrExport",         "-"           ),  GraceNote.Narisokonai.scrExport.Export) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "GraceNote.FindIdenticalEntries",          "GNident"     ),  GraceNote.FindIdenticalEntries.Finder.Execute) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "AceAttorney.PrintScriptForLP",            "-"           ),  AceAttorney.Script.DumpTextForLP) },
			{ new KeyValuePair<ProgramName, ExecuteProgramDelegate>( new ProgramName( "MyWorldMyWay.Tbl.Dump",                   "-"           ),  MyWorldMyWay.Tbl.DumpText) },
		};

		[STAThread]
		static int Main( string[] args ) {



			//var das = System.IO.File.ReadAllBytes(@"c:\Users\Georg\Downloads\PCSX2_0.9.2_Brk_on_RW\sstates\TEST");
			//var res = Util.GetTextPseudoShiftJis( das, 0 );
			//StringBuilder b = new StringBuilder(res.Length);












			if ( args.Length > 0 ) {
				string ProgramName = args[0];
				if ( ProgramName == "-" ) { PrintUsage(); return -1; }
				List<string> ProgramArguments = new List<string>( args.Length - 1 );
				for ( int i = 1; i < args.Length; ++i ) {
					ProgramArguments.Add( args[i] );
				}

				var kvp = ProgramList.Find( x => x.Key.Equals( ProgramName ) );
				if ( kvp.Value != null ) {
					return kvp.Value( ProgramArguments );
				} else {
					PrintUsage();
				}

			} else {
				PrintUsage();
			}
			return -1;
		}

		private static void PrintUsage() {
			ProgramList.Sort( ( x, y ) => x.Key.CompareTo( y.Key ) );
			foreach ( var p in ProgramList ) {
				Console.WriteLine( String.Format( " {1,-12} {0}", p.Key.Name, p.Key.Shortcut ) );
			}
		}
	}
}
