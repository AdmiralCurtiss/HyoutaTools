using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Abyss.SB7 {
	public class SB7 {
		public uint PointerToText;
		public uint PointerToSomething;
		public uint EndOfTextStartList;
		public uint SomethingCounter;
		public List<string> Somethings;
		public List<string> Texts;

		public SB7( string Filename ) {
			Load( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Load( byte[] File ) {
			PointerToText = BitConverter.ToUInt32( File, 0x50 );
			EndOfTextStartList = BitConverter.ToUInt32( File, 0x30 );
			SomethingCounter = BitConverter.ToUInt32( File, 0x5C );

			/*
			PointerToSomething = BitConverter.ToUInt32( File, 0x50 );
			Somethings = new List<string>();
			for ( uint i = 0; i < SomethingCounter; i += 4 ) {
				Somethings.Add( Util.GetTextShiftJis( File, (int)( PointerToSomething + BitConverter.ToUInt32( File, (int)i + 0x5C ) ) ) );
			}
			 * */

			Texts = new List<string>();
			for ( uint i = 0x5C + SomethingCounter; i < EndOfTextStartList; i += 4 ) {
				Texts.Add( Util.GetTextShiftJis( File, (int)( PointerToText + BitConverter.ToUInt32( File, (int)i ) ) ) );
			}

			return;
		}



	}
}
