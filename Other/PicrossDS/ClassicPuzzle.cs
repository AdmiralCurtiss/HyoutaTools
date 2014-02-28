using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class ClassicPuzzle {

		// should be a total of 0xC0 bytes, starts at 0x30DA4
		public byte Unknown1;
		public byte Unknown2; // 0x11 unsolved 0x59 solved ???
		public byte Width;
		public byte Height;
		public uint ClearTime; // in frames at 60 fps
		public byte Mode; // 0x01 = Normal, 0x02 = Free
		public byte Unknown3;
		public string PuzzleName; // 0x32 bytes
		public byte PackNumber;
		public byte PackLetter;
		public string PackName; // 0x32 bytes
		public uint[] PuzzleData; // 20 lines, 4 bytes each, first 7 bits always 0, rest left-to-right 0 empty 1 filled

		public bool Deleted = false;
		public int IndexForGui = 0;

		public ClassicPuzzle() { }
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

		public override string ToString() {
			return "Classic Puzzle #" + IndexForGui + ": " + PuzzleName.Trim( '\0' ) + " (" + PackName.Trim( '\0' ) + ")";
		}
	}
}
