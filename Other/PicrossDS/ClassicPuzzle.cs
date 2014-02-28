using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class ClassicPuzzle {

		// should be a total of 0xC0 bytes, starts at 0x30DA4
		byte Unknown1;
		byte Unknown2; // 0x11 unsolved 0x59 solved ???
		byte Width;
		byte Height;
		uint ClearTime; // in frames at 60 fps
		byte Mode; // 0x01 = Normal, 0x02 = Free
		byte Unknown3;
		string PuzzleName; // 0x32 bytes
		byte PackNumber;
		byte PackLetter;
		string PackName; // 0x32 bytes
		uint[] PuzzleData; // 20 lines, 4 bytes each, first 7 bits always 0, rest left-to-right 0 empty 1 filled

		public ClassicPuzzle( byte[] File, uint Offset ) {
			Unknown1 = File[Offset];
			Unknown2 = File[Offset + 0x01];
			Width = File[Offset + 0x02];
			Height = File[Offset + 0x03];
			ClearTime = BitConverter.ToUInt32( File, (int)Offset + 0x04 );
			Mode = File[Offset + 0x08];
			Unknown3 = File[Offset + 0x09];
			PuzzleName = Encoding.Unicode.GetString( File, (int)Offset + 0x0A, 0x32 );
			PackNumber = File[Offset + 0x3C];
			PackLetter = File[Offset + 0x3D];
			PackName = Encoding.Unicode.GetString( File, (int)Offset + 0x3E, 0x32 );

			PuzzleData = new uint[20];
			for ( int i = 0; i < 20; ++i ) {
				PuzzleData[i] = BitConverter.ToUInt32( File, (int)Offset + 0x70 + i * 4 );
			}
		}
	}
}
