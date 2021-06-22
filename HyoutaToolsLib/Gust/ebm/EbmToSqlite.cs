using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Gust.ebm {
	public class EbmToSqlite {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile.ebm outfile.sqlite" );
				return -1;
			}

			string infile = args[0];
			string outfile = args[1];
			var ebm = new ebm( infile, TextUtils.GameTextEncoding.UTF8 );

			if ( System.IO.File.Exists( outfile ) ) {
				System.IO.File.Delete( outfile );
			}
			SQLiteConnection connection = new SQLiteConnection( "Data Source=" + outfile );
			connection.Open();
			using ( SQLiteTransaction transaction = connection.BeginTransaction() ) {
				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					command.CommandText = "CREATE TABLE IF NOT EXISTS ebm(" +
						"ID INTEGER PRIMARY KEY AUTOINCREMENT," +
						"Ident UNSIGNED INT," +
						"Unknown2 UNSIGNED INT," +
						"Unknown3 UNSIGNED INT," +
						"CharacterId INT," +
						"Unknown5 UNSIGNED INT," +
						"Unknown6 UNSIGNED INT," +
						"Unknown7 UNSIGNED INT," +
						"Unknown8 UNSIGNED INT," +
						"Entry TEXT" +
						")";
					command.ExecuteNonQuery();
				}

				using ( SQLiteCommand command = new SQLiteCommand( connection ) ) {
					SQLiteParameter Ident = new SQLiteParameter();
					SQLiteParameter Unknown2 = new SQLiteParameter();
					SQLiteParameter Unknown3 = new SQLiteParameter();
					SQLiteParameter CharacterId = new SQLiteParameter();
					SQLiteParameter Unknown5 = new SQLiteParameter();
					SQLiteParameter Unknown6 = new SQLiteParameter();
					SQLiteParameter Unknown7 = new SQLiteParameter();
					SQLiteParameter Unknown8 = new SQLiteParameter();
					SQLiteParameter Text = new SQLiteParameter();
					command.CommandText = "INSERT INTO ebm ( Ident, Unknown2, Unknown3, CharacterId, Unknown5, Unknown6, Unknown7, Unknown8, Entry ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)";
					command.Parameters.Add( Ident );
					command.Parameters.Add( Unknown2 );
					command.Parameters.Add( Unknown3 );
					command.Parameters.Add( CharacterId );
					command.Parameters.Add( Unknown5 );
					command.Parameters.Add( Unknown6 );
					command.Parameters.Add( Unknown7 );
					command.Parameters.Add( Unknown8 );
					command.Parameters.Add( Text );

					foreach ( var e in ebm.EntryList ) {
						Ident.Value = e.Ident;
						Unknown2.Value = e.Unknown2;
						Unknown3.Value = e.Unknown3;
						CharacterId.Value = e.CharacterId;
						Unknown5.Value = e.Unknown5;
						Unknown6.Value = e.Unknown6;
						Unknown7.Value = e.Unknown7;
						Unknown8.Value = e.Unknown8;
						Text.Value = e.Text;
						command.ExecuteNonQuery();
					}
				}
				transaction.Commit();
			}
			connection.Close();

			return 0;
		}
	}
}
