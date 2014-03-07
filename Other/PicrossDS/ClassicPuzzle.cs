using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class ClassicPuzzle {

		// should be a total of 0xC0 bytes, starts at 0x30DA4
		public byte Type; // 0x02 = Classic, 0x03 = Original. SET THIS CORRECTLY OR THE GAME GETS CONFUSED
		public byte Unknown2; // seems to be a bitmask, 0x01 = uncleared
		public byte Width;
		public byte Height;
		public uint ClearTime; // in frames at 60 fps
		public byte Mode; // 0x01 = Normal, 0x02 = Free
		public byte Unknown3;
		public string PuzzleName; // 0x32 bytes
		public byte PackNumber;	// seems also to be used for difficulty in original puzzles
		public byte PackLetter;
		public string PackName; // 0x32 bytes
		public uint[] PuzzleData; // 20 lines, 4 bytes each, first 7 bits always 0, rest left-to-right 0 empty 1 filled

		public bool Deleted = false;
		public int IndexForGui = 0;

		public ClassicPuzzle() { }
		public ClassicPuzzle( byte[] File, uint Offset ) {
			Type = File[Offset];
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

		public virtual void Write( byte[] File, uint Offset ) {
			File[Offset] = Type;
			File[Offset + 0x01] = Unknown2;
			File[Offset + 0x02] = Width;
			File[Offset + 0x03] = Height;
			BitConverter.GetBytes( ClearTime ).CopyTo( File, Offset + 0x04 );
			File[Offset + 0x08] = Mode;
			File[Offset + 0x09] = Unknown3;

			// this technically might overflow, but will get overwritten by the packnumber/letter so whatev
			byte[] puzzleNameBytes = Encoding.Unicode.GetBytes( PuzzleName + '\0' );
			puzzleNameBytes.CopyTo( File, Offset + 0x0A );

			File[Offset + 0x3C] = PackNumber;
			File[Offset + 0x3D] = PackLetter;

			// same, except it'll get overwritten by the puzzle data
			byte[] packNameBytes = Encoding.Unicode.GetBytes( PackName + '\0' );
			packNameBytes.CopyTo( File, Offset + 0x3E );

			for ( int i = 0; i < 20; ++i ) {
				BitConverter.GetBytes( PuzzleData[i] ).CopyTo( File, Offset + 0x70 + i * 4 );
			}
		}

		public override string ToString() {
			string pzlName = PuzzleName.Contains( '\0' ) ? PuzzleName.Substring( 0, PuzzleName.IndexOf( '\0' ) ) : PuzzleName;
			string pckName = PackName.Contains( '\0' ) ? PackName.Substring( 0, PackName.IndexOf( '\0' ) ) : PackName;
			return "Classic Puzzle #" + IndexForGui.ToString( "D3" ) + " - " + pckName + " - " + pzlName;
		}
	}
}
