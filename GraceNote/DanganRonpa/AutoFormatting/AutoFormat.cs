using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;
using System.Data.SQLite;

namespace HyoutaTools.GraceNote.DanganRonpa.AutoFormatting {
	public class AutoFormat {

		public static int Execute( List<string> args ) {
			FormatDatabase( args[0], args[1] );
			return 0;
		}

		public static void FormatDatabase( string Filename, string FilenameGracesJapanese ) {
			GraceNoteDatabaseEntry[] entries = GraceNoteDatabaseEntry.GetAllEntriesFromDatabase( "Data Source=" + Filename, "Data Source=" + FilenameGracesJapanese );

			foreach ( GraceNoteDatabaseEntry e in entries ) {
				if ( e.Status == -1 ) { continue; }

				e.TextEN = FormatString( e.TextEN );

				Util.GenericSqliteUpdate(
					"Data Source=" + Filename,
					"UPDATE Text SET english = ? WHERE ID = ?",
					new object[] { e.TextEN, e.ID }
				);
			}
			return;
		}

		public static string FormatString( string str ) {
			bool NewLineAtEnd = str.EndsWith( "\n" );

			StringBuilder sb = new StringBuilder( str );

			int bracketCount = 0;
			int charCount = 0;
			int lastSpaceAt = -1;
			int end = NewLineAtEnd ? sb.Length - 1 : sb.Length;
			int numbersOfNewLinesInserted = 0;
			for ( int i = 0; i < end; ++i ) {
				char c = sb[i];
				switch ( c ) {
					case '<': bracketCount++; break;
					case '>': bracketCount--; break;
				}
				if ( bracketCount > 0 ) continue;

				switch ( c ) {
					case ' ': lastSpaceAt = i; charCount++; break;
					case '\n': sb[i] = ' '; lastSpaceAt = i; charCount++; break;
					default: charCount++; break;
				}

				if ( charCount >= 28 ) {
					numbersOfNewLinesInserted++;
					charCount = i - lastSpaceAt;
					sb[lastSpaceAt] = numbersOfNewLinesInserted % 2 == 0 ? '\f' : '\n'; // inserts a newline on 1st, 3rd, 5th, ... and a feed on 2nd, 4th, 6th, ...
				}
			}

			return sb.ToString();
		}
	}
}
