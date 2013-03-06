using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.DanganRonpa.Nonstop;
using HyoutaTools.DanganRonpa.Lin;
using System.Data.SQLite;
using HyoutaTools.DanganRonpa;

namespace HyoutaTools.GraceNote.DanganRonpa.NonstopExistingDatabaseImport {
	class Importer {

		public static Dictionary<string, string> nonstopDict = new Dictionary<string, string> {
		{ "nonstop_01_001.dat", "e01_102_001.lin" }, { "nonstop_01_002.dat", "e01_104_001.lin" }, { "nonstop_01_003.dat", "e01_106_001.lin" }, { "nonstop_01_004.dat", "e01_110_001.lin" }, { "nonstop_01_005.dat", "e01_112_001.lin" }, { "nonstop_01_006.dat", "e01_114_001.lin" }, { "nonstop_01_007.dat", "e01_116_001.lin" }, { "nonstop_01_008.dat", "e01_118_001.lin" }, { "nonstop_02_001.dat", "e02_102_001.lin" }, { "nonstop_02_002.dat", "e02_104_001.lin" }, { "nonstop_02_003.dat", "e02_108_001.lin" }, { "nonstop_02_004.dat", "e02_110_001.lin" }, { "nonstop_02_005.dat", "e02_112_001.lin" }, { "nonstop_02_006.dat", "e02_114_001.lin" }, { "nonstop_02_007.dat", "e02_118_001.lin" }, { "nonstop_02_008.dat", "e02_120_001.lin" }, { "nonstop_02_009.dat", "e02_122_001.lin" }, { "nonstop_03_001.dat", "e03_102_001.lin" }, { "nonstop_03_002.dat", "e03_106_001.lin" }, { "nonstop_03_003.dat", "e03_108_001.lin" }, { "nonstop_03_004.dat", "e03_112_001.lin" }, { "nonstop_03_005.dat", "e03_114_001.lin" }, { "nonstop_03_006.dat", "e03_116_001.lin" }, { "nonstop_03_007.dat", "e03_118_001.lin" }, { "nonstop_03_008.dat", "e03_120_001.lin" }, { "nonstop_03_009.dat", "e03_122_001.lin" }, { "nonstop_03_010.dat", "e03_124_001.lin" }, { "nonstop_03_011.dat", "e03_126_001.lin" }, { "nonstop_04_001.dat", "e04_102_001.lin" }, { "nonstop_04_002.dat", "e04_104_001.lin" }, { "nonstop_04_003.dat", "e04_106_001.lin" }, { "nonstop_04_004.dat", "e04_110_001.lin" }, { "nonstop_04_005.dat", "e04_116_001.lin" }, { "nonstop_04_006.dat", "e04_118_001.lin" }, { "nonstop_04_007.dat", "e04_122_001.lin" }, { "nonstop_04_008.dat", "e04_124_001.lin" }, { "nonstop_05_001.dat", "e05_102_001.lin" }, { "nonstop_05_002.dat", "e05_104_001.lin" }, { "nonstop_05_003.dat", "e05_110_001.lin" }, { "nonstop_05_004.dat", "e05_114_001.lin" }, { "nonstop_05_005.dat", "e05_116_001.lin" }, { "nonstop_05_006.dat", "e05_118_001.lin" }, { "nonstop_05_007.dat", "e05_120_001.lin" }, { "nonstop_05_008.dat", "e05_122_001.lin" }, { "nonstop_05_009.dat", "e05_151_001.lin" }, { "nonstop_06_001.dat", "e06_102_001.lin" }, { "nonstop_06_002.dat", "e06_106_001.lin" }, { "nonstop_06_003.dat", "e06_108_001.lin" }, { "nonstop_06_004.dat", "e06_110_001.lin" }, { "nonstop_06_005.dat", "e06_112_001.lin" }, { "nonstop_06_006.dat", "e06_118_001.lin" }, { "nonstop_06_007.dat", "e06_120_001.lin" }, { "nonstop_06_008.dat", "e06_134_001.lin" }, { "nonstop_06_009.dat", "e06_137_001.lin" },
		{ "nonstop_06_010.dat", "e06_143_001.lin" } };
		//{ "nonstop_06_025.dat", "e06_143_001.lin" } };
		public static int Import( string[] args ) {
			//if ( args.Length < 1 ) {
			//    Console.WriteLine( "Usage: text.lin.orig text.lin.new database [alignment (default 1024)]" );
			//    return -1;
			//}

			//int Alignment;
			//if ( !( args.Length >= 4 && Int32.TryParse( args[3], out Alignment ) ) ) {
			//    Alignment = 1024;
			//}
			//return LinExport.Exporter.Export( args[0], args[1], args[2], Alignment );
			return -1;
		}

		public static string GetFromSubstring( string[] strs, string sub ) {
			foreach ( string s in strs ) { if ( s.Contains( sub ) ) return s; }
			return null;
		}
		public static int AutoImport( List<string> args ) {
			string dir = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfRebuild\umdimage.dat.ex\";
			string voicedir = @"d:\_svn\GraceNote\GraceNote\Voices\";
			string[] files = System.IO.Directory.GetFiles( dir );

			List<String> dbsToUp = new List<string>();

			foreach ( var x in nonstopDict ) {
				string nonstopFile = GetFromSubstring( files, x.Key );
				string scriptFile = GetFromSubstring( files, x.Value );
				string scriptFileFilename = new System.IO.FileInfo( scriptFile ).Name;
				string databaseId = scriptFileFilename.Substring( 0, 4 );
				string databaseFile = @"d:\_svn\GraceNote\GraceNote\DanganRonpaBestOfDB\DRBO" + databaseId;
				dbsToUp.Add( "DRBO" + databaseId );
				//continue;

				LIN lin = new LIN( scriptFile );
				Nonstop nonstop = new Nonstop( nonstopFile );

				int lastScriptEntry = 0;
				foreach ( var item in nonstop.items ) {
					int stringId = item.data[(int)NonstopSingleStructure.StringID] + 1;
					int correspondingTextEntry = stringId * 2;
					int correspondingScriptEntry = correspondingTextEntry - 1;
					if ( item.data[(int)NonstopSingleStructure.Type] == 0 ) {
						lastScriptEntry = correspondingTextEntry;
					}


					// --- insert comment info ---
					string comment = (string)Util.GenericSqliteSelect(
						"Data Source=" + databaseFile,
						"SELECT comment FROM Text WHERE id = ?",
						new object[] { correspondingTextEntry } );

					bool weakpt = item.data[(int)NonstopSingleStructure.HasWeakPoint] > 0;
					comment = ( comment == "" ? "" : comment + "\n\n" )
						+ "Autogenerated Info:\n"
						+ ( lastScriptEntry == 0 ? "Corresponds to file: " + scriptFileFilename : "" )
						+ ( item.data[(int)NonstopSingleStructure.Type] == 0 ? "Normal Line\n" : "Background Noise\n" )
						+ ( weakpt ? "Has a Weak Point\n" : "No Weakpoint\n" )
						+ ( weakpt && ( item.data[(int)NonstopSingleStructure.ShootWithEvidence] & 0xFF ) != 255 ? "Shot with Evidence Bullet: " + item.data[(int)NonstopSingleStructure.ShootWithEvidence] + "\n" : "" )
						+ ( weakpt && ( item.data[(int)NonstopSingleStructure.ShootWithWeakpoint] & 0xFF ) != 255 ? "Shot with Weak Point: " + item.data[(int)NonstopSingleStructure.ShootWithWeakpoint] + "\n" : "" )
						+ ( weakpt && ( item.data[(int)NonstopSingleStructure.ShootWithWeakpoint] & 0xFF ) == 255 && ( item.data[(int)NonstopSingleStructure.ShootWithEvidence] & 0xFF ) == 255 ? "Can't be shot\n" : "" )
						+ ( item.data[(int)NonstopSingleStructure.Type] == 0 ? "" : "Appears around Entry #" + lastScriptEntry + "\n" )
						+ ( item.data[(int)NonstopSingleStructure.Type] == 0 ? "Sprite: " + DanganUtil.CharacterIdToName( (byte)item.data[(int)NonstopSingleStructure.Character] ) + " " + item.data[(int)NonstopSingleStructure.Sprite] + "\n" : "" )
						;
					Util.GenericSqliteUpdate(
						"Data Source=" + databaseFile,
						"UPDATE Text SET comment = ?, updated = 1 WHERE id = ?",
						new object[] { comment, correspondingTextEntry } );


					// --- insert voice info ---
					string script = (string)Util.GenericSqliteSelect(
						"Data Source=" + databaseFile,
						"SELECT english FROM Text WHERE id = ?",
						new object[] { correspondingScriptEntry } );
					string voicename;
					string voicefilecheck;

					byte charid = (byte)item.data[(int)NonstopSingleStructure.Character];
					if ( item.data[(int)NonstopSingleStructure.Type] == 0 ) {
						while ( true ) {
							string charac = DanganUtil.CharacterIdToName( charid );
							if ( charac == "Naegi" ) { charac = "Neagi"; }
							voicename = "[" + charac + "] " + item.data[(int)NonstopSingleStructure.Chapter] + " "
								+ ( item.data[(int)NonstopSingleStructure.AudioSampleId] >> 8 ) + " " + ( item.data[(int)NonstopSingleStructure.AudioSampleId] & 0xFF ) + " 100";
							voicefilecheck = voicedir + voicename + ".mp3";
							if ( System.IO.File.Exists( voicefilecheck ) ) {
								break;
							}
							charid = 0x12;
						}

						script += "<__END__>\n"
							+ "<Voice: " + voicename + ">"
							;
						Util.GenericSqliteUpdate(
							"Data Source=" + databaseFile,
							"UPDATE Text SET english = ?, updated = 1 WHERE id = ?",
							new object[] { script, correspondingScriptEntry } );


						// update the header name thingy
						string header = DanganUtil.CharacterIdToName( charid );
						Util.GenericSqliteUpdate(
							"Data Source=" + databaseFile,
							"UPDATE Text SET IdentifyString = ?, updated = 1 WHERE id = ?",
							new object[] { header, correspondingTextEntry } );
					} else {
						string header = "Background Noise";
						Util.GenericSqliteUpdate(
							"Data Source=" + databaseFile,
							"UPDATE Text SET IdentifyString = ?, updated = 1 WHERE id = ?",
							new object[] { header, correspondingTextEntry } );
					}

				}
			}

			System.IO.File.WriteAllLines(
				@"d:\_svn\GraceNote\GraceNote\temp.txt", dbsToUp.ToArray());
			return 0;
		}
	}
}
