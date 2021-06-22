using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	class Program {
		public static int Execute( List<string> args ) {
			TSS.TSSFile stringDic = new TSS.TSSFile( args[1], TextUtils.GameTextEncoding.ShiftJIS, EndianUtils.Endianness.BigEndian );
			var stringIdDict = stringDic.GenerateInGameIdDictionary();

			T8BTMA arteFile = new T8BTMA( args[0], EndianUtils.Endianness.BigEndian, BitUtils.Bitness.B32 );

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
