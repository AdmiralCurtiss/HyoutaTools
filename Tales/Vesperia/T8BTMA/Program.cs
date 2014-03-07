using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	class Program {
		public static int Execute( List<string> args ) {
			TSS.TSSFile stringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( args[1] ) );

			Dictionary<uint, TSS.TSSEntry> stringIdDict = new Dictionary<uint, TSS.TSSEntry>();
			foreach ( var e in stringDic.Entries ) {
				if ( e.inGameStringId > -1 ) {
					stringIdDict.Add( (uint)e.inGameStringId, e );
				}
			}

			T8BTMA arteFile = new T8BTMA( args[0] );

			StringBuilder sb = new StringBuilder();
			foreach ( var a in arteFile.ArteList ) {
				sb.Append( a.Type.ToString() );
				sb.Append( " --- " );
				sb.Append( stringIdDict[a.StringIdName].StringJPN );
				sb.AppendLine();
				//sb.AppendLine( a.ToString() );
			}

			System.IO.File.WriteAllText( args[0] + ".txt", sb.ToString() );

			return 0;
		}
	}
}
