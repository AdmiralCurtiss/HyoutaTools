using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FinalFantasyCrystalChronicles {
	public static class TextDumper {
		public static int Execute( List<string> args ) {
			if ( args.Count != 2 ) {
				Console.WriteLine( "Usage: input.cfd output.txt" );
				return -1;
			}

			FileSections.CFLD cfld;
			using ( FileStream fs = File.Open( args[0], FileMode.Open, FileAccess.Read ) ) {
				cfld = new FileSections.CFLD( fs );
				fs.Close();
			}

			StringBuilder sb = new StringBuilder();
			foreach ( var s in cfld.Subsections ) {
				if ( s as FileSections.MES != null ) {
					sb.AppendLine( "============ MES  BLOCK ============" );
					sb.AppendLine();
					int i = 0;
					foreach ( var m in ( s as FileSections.MES ).Messages ) {
						sb.Append( "[Entry " ).Append( i++ ).Append( "]" ).AppendLine();
						sb.AppendLine( m );
						sb.AppendLine();
					}
				}
				if ( s as FileSections.TABL != null ) {
					sb.AppendLine( "============ TABL BLOCK ============" );
					sb.AppendLine();
					int i = 0;
					foreach ( var m in ( s as FileSections.TABL ).Messages ) {
						sb.Append( "[Entry " ).Append( i++ ).Append( "]" ).AppendLine();
						sb.AppendLine( m );
						sb.AppendLine();
					}
				}
				sb.AppendLine();
			}

			System.IO.File.WriteAllText( args[1], sb.ToString() );

			return 0;
		}
	}
}
