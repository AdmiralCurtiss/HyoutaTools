using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Gust.ebm {
	public class SqliteToEbm {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: infile.sqlite outfile.ebm" );
				return -1;
			}

			string infile = args[0];
			string outfile = args[1];
			var ebm = new ebm();

			var rows = SqliteUtil.SelectArray( "Data Source=" + infile, "SELECT Ident, Unknown2, Unknown3, CharacterId, Unknown5, Unknown6, Unknown7, Unknown8, Entry FROM ebm ORDER BY ID" );
			foreach ( var row in rows ) {
				ebmEntry e = new ebmEntry() {
					Ident = (uint)(long)row[0],
					Unknown2 = (uint)(long)row[1],
					Unknown3 = (uint)(long)row[2],
					CharacterId = (int)row[3],
					Unknown5 = (uint)(long)row[4],
					Unknown6 = (uint)(long)row[5],
					Unknown7 = (uint)(long)row[6],
					Unknown8 = (uint)(long)row[7],
					Text = (string)row[8],
				};
				ebm.EntryList.Add( e );
			}

			using ( Stream s = new FileStream( outfile, FileMode.Create ) ) {
				ebm.WriteFile( s, TextUtils.GameTextEncoding.UTF8 );
				s.Close();
			}

			return 0;
		}
	}
}
