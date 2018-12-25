using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.TO8CHTX;

namespace HyoutaTools.GraceNote.Vesperia.TO8CHTXExport {
	class Program {
		public static int Execute( List<string> args ) {
			String Filename;
			String Database;
			String NewFilename;

			if ( args.Count != 3 ) {
				Console.WriteLine( "Usage: GraceNote_TO8CHTX ChatFilename DBFilename NewChatFilename" );
				return -1;
			} else {
				Filename = args[0];
				Database = args[1];
				NewFilename = args[2];
			}

			ChatFile c = new ChatFile( Filename, Util.Endianness.BigEndian, Util.GameTextEncoding.ShiftJIS );

			c.GetSQL( "Data Source=" + Database );

			c.RecalculatePointers();
			System.IO.File.WriteAllBytes( NewFilename, c.Serialize() );

			return 0;
		}
	}
}
