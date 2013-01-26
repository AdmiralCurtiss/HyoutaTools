using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaTools.Tales.Vesperia.To8chtx;

namespace HyoutaTools.GraceNote.Vesperia.To8chtxExport {
	class Program {
		static void Execute( string[] args ) {
			String Filename;
			String Database;
			String NewFilename;

			if ( args.Length != 3 ) {
				Console.WriteLine( "Usage: GraceNote_TO8CHTX ChatFilename DBFilename NewChatFilename" );
				return;
			} else {
				Filename = args[0];
				Database = args[1];
				NewFilename = args[2];
			}
			/*
			Filename = @"c:\#gracenote_chat_repack\oldchat\VC002J";
			Database = @"c:\#gracenote_chat_repack\db\VC002J";
			NewFilename = @"c:\#gracenote_chat_repack\newchat\VC002J";
			//*/
			ChatFile c = new ChatFile( System.IO.File.ReadAllBytes( Filename ) );

			c.GetSQL( "Data Source=" + Database );

			c.RecalculatePointers();
			System.IO.File.WriteAllBytes( NewFilename, c.Serialize() );

			return;
		}
	}
}
