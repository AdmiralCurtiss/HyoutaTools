using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools {
	class Program {
		static int Main( string[] args ) {
			if ( args.Length > 0 ) {
				string ProgramName = args[0];
				List<string> ProgramArguments = new List<string>( args.Length - 1 );
				for ( int i = 1; i < args.Length; ++i ) {
					ProgramArguments.Add( args[i] );
				}

				switch ( ProgramName ) {
					case "DanganRonpa.umdimagedat": return DanganRonpa.umdimagedat.umdimagedat.Execute( ProgramArguments.ToArray() );
					case "Generic.BlockCopy": return Generic.BlockCopy.BlockCopy.Execute( ProgramArguments.ToArray() );
					case "Generic.ByteHotfix": return Generic.ByteHotfix.ByteHotfix.Execute( ProgramArguments.ToArray() );
					case "Tales.tlzc": return Tales.tlzc.tlzcmain.Execute( ProgramArguments.ToArray() );
					// case "GraceNote.LuxPainDump":
					// case "GraceNote.XilliaScriptFileDump":
					// case "Other.GoldenSunDarkDawnMsgExtract":
					// case "GraceNote.Vesperia.VVoicesGenerate":
					// case "Tales.Xillia.TldatExtract":
					// case "Tales.Vesperia.MapList":
					// case "Other.NisPakEx":
					// case "Tales.Vesperia.SpkdUnpack":
					// case "GraceNote.Vesperia.SCFOMBIN-to-GraceNote":
					//
					//
					//
					//
					//
				}

			}
			return -1;
		}
	}
}
