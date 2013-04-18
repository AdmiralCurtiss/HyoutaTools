using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Collections;

namespace HyoutaTools.GraceNote.GoogleTranslate {
	public class TranslateDatabase {
		public static void Translate( string filename ) {
			Stream stream = new FileStream( filename, FileMode.Open, FileAccess.Read );
			Scs scs = Scs.Create( stream );
			stream.Close();
			WebClient webClient = new WebClient();
			webClient.BaseAddress = "http://translate.google.com";
			int num = 0;
			int entryCount = scs.Entries.Count;


			for ( int j = num; j < entryCount; j++ ) {
				string text = scs.Entries[j];
				try {
					webClient.Encoding = Encoding.UTF8;
					webClient.Headers["User-Agent"] = "Mozilla/5.0 (X11; U; Linux x86_64; en-US; rv:1.9.2.3) Gecko/20100402 Namoroka/3.6.3 (.NET CLR 3.5.30729)";
					webClient.QueryString.Clear();
					webClient.QueryString["client"] = "t";
					webClient.QueryString["sl"] = "ja";
					webClient.QueryString["tl"] = "en";
					webClient.QueryString["text"] = text;
					
					string text2 = webClient.DownloadString( "/translate_a/t" );
					
					scs.Entries[j] = text2;

				} catch ( WebException ) {
				}
			}
			Stream stream2 = new FileStream( filename + ".new", FileMode.Create, FileAccess.Write );
			scs.Save( stream2 );
			stream2.Close();
		}
	}
}
