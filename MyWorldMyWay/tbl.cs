using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools;
using HyoutaUtils;

namespace HyoutaTools.MyWorldMyWay {
	class Tbl {
		public static int DumpText( List<string> args ) {
			string Filename = args[0];
			string Outfilename = args[1];
			var File = new FileStream( Filename, FileMode.Open );

			Tbl tbl = new Tbl();
			tbl.LoadFile( File );
			File.Close();

			System.IO.File.WriteAllLines( Outfilename, tbl.Strings.ToArray() );
			return 0;
		}

		List<uint> Pointers;
		public List<string> Strings;
		public void LoadFile( Stream File ) {
			File.Position = 0;

			Pointers = new List<uint>();
			while ( true ) {
				uint ptr = File.ReadUInt32();
				if ( ptr == 0 ) break;
				Pointers.Add( ptr );
			}

			uint Magic = File.ReadUInt32();
			uint LengthWithoutPointers = File.ReadUInt32();
			uint StringCount = File.ReadUInt32();

			Strings = new List<string>( (int)StringCount );
			foreach ( uint ptr in Pointers ) {
				File.Position = ptr;
				string s = File.ReadUTF16Nullterm();
				Strings.Add( s );
			}
		}
	}
}
