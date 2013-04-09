using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	public class OoTSaveFile {
		byte[] File;
		public OoTSaveHeader Header;
		public OoTSingleSave[] Saves;

		public bool EndianSwappedSave = false;

		public OoTSaveFile( string Filename ) {
			Initialize( System.IO.File.ReadAllBytes( Filename ) );
		}

		public OoTSaveFile( byte[] File ) {
			Initialize( File );
		}

		private void Initialize( byte[] File ) {
			this.File = File;

			Header = new OoTSaveHeader( File );
			if ( Header.IdentifyTextELDA == 0x41444c45 ) {
				SwapFileEndianness();
				EndianSwappedSave = true;
				Header = new OoTSaveHeader( File );
			}

			Saves = new OoTSingleSave[6];
			for ( int i = 0; i < Saves.Length; ++i ) {
				Saves[i] = new OoTSingleSave( File, 32 + i * 5200 );
			}
		}

		private void SwapFileEndianness() {
			// swap the whole file around
			byte b0, b1, b2, b3;
			for ( int i = 0; i < File.Length; i += 4 ) {
				b0 = File[i];
				b1 = File[i + 1];
				b2 = File[i + 2];
				b3 = File[i + 3];
				File[i] = b3;
				File[i + 1] = b2;
				File[i + 2] = b1;
				File[i + 3] = b0;
			}
		}

		internal void WriteSave( string Filename ) {
			Header.WriteToFile();
			foreach ( OoTSingleSave s in Saves ) {
				s.WriteToFile();
			}

			if ( EndianSwappedSave ) {
				SwapFileEndianness();
			}

			System.IO.File.WriteAllBytes( Filename, File );

			if ( EndianSwappedSave ) {
				SwapFileEndianness();
			}
		}
	}
}
