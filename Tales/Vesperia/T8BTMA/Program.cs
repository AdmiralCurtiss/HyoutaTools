using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	class Program {
		public static int Execute( List<string> args ) {
			TSS.TSSFile stringDic = new TSS.TSSFile( System.IO.File.ReadAllBytes( args[1] ) );
			var stringIdDict = stringDic.GenerateInGameIdDictionary();

			T8BTMA arteFile = new T8BTMA( args[0] );

			StringBuilder sb = new StringBuilder();
			foreach ( var a in arteFile.ArteList ) {
				sb.Append( a.Type.ToString() );
				sb.Append( " --- " );
				sb.Append( stringIdDict[a.NameStringDicId].StringJpn );
				sb.AppendLine();
				//sb.AppendLine( a.ToString() );
			}

			System.IO.File.WriteAllText( args[0] + ".txt", sb.ToString() );

			arteFile.UpdateDatabaseWithArteProps( "Data Source=" + args[2] );

			return 0;
		}
	}
}
