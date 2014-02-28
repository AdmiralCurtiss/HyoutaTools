using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.PicrossDS {
	class SaveFile {
		public SaveFile( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "PicrossDS.SaveFile: Load Failed!" );
			}
		}

		public SaveFile( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "PicrossDS.SaveFile: Load Failed!" );
			}
		}

		byte[] File;
		public ClassicPuzzle[] ClassicPuzzles;
		public OriginalPuzzle[] OriginalPuzzles;

		private bool LoadFile( byte[] File ) {
			this.File = File;
			return true;
		}

		static uint ClassicPuzzleMappingTableOffset = 0x30DA4 - 0x64;
		static uint ClassicPuzzleOffset = 0x30DA4;
		public void LoadClassicPuzzles() {
			HashSet<uint> mappedPuzzles = new HashSet<uint>();
			List<ClassicPuzzle> puzzleList = new List<ClassicPuzzle>();

			// read all existing puzzles in ingame order
			for ( uint i = 0; i < 100; ++i ) {
				uint index = File[ClassicPuzzleMappingTableOffset + i];
				if ( index > 0x63 ) {
					continue;	
				}

				var puzzle = new ClassicPuzzle( File, ClassicPuzzleOffset + index * 0xC0 );
				puzzleList.Add( puzzle );
				mappedPuzzles.Add( index );
			}

			// find deleted puzzles, add those too
			for ( uint i = 0; i < 100; ++i ) {
				if ( !mappedPuzzles.Contains( i ) ) {
					var puzzle = new ClassicPuzzle( File, ClassicPuzzleOffset + i * 0xC0 );
					puzzle.Deleted = true;
					puzzleList.Add( puzzle );
				}
			}

			// and fill the rest with garbage if needed
			while ( puzzleList.Count < 100 ) {
				puzzleList.Add( new ClassicPuzzle() );
			}

			// and cut off if somehow more than 100 were added
			if ( puzzleList.Count > 100 ) {
				puzzleList.RemoveRange( 100, puzzleList.Count - 100 );
			}

			for ( int i = 0; i < puzzleList.Count; ++i ) {
				puzzleList[i].IndexForGui = i + 1;
			}

			ClassicPuzzles = puzzleList.ToArray();
		}

		public void WriteClassicPuzzles() {
			for ( uint i = 0; i < 100; ++i ) {
				ClassicPuzzles[i].Write( File, ClassicPuzzleOffset + i * 0xC0 );
			}

			// and write the new puzzle map
			List<byte> puzzleMap = new List<byte>();
			for ( uint i = 0; i < 100; ++i ) {
				// might need a better detection or something
				if ( ClassicPuzzles[i].Unknown1 != 0xFF ) {
					puzzleMap.Add( (byte)i );
				}
			}

			int puzzleCount = puzzleMap.Count;
			while ( puzzleMap.Count < 100 ) {
				puzzleMap.Add( 0xFF );
			}

			puzzleMap.CopyTo( File, (int)ClassicPuzzleMappingTableOffset );
			File[0x358A4] = (byte)puzzleCount;
		}

		public void RecalculateChecksum() {
			byte[] checksumTable = new byte[] { 1, 2, 4, 8,
											    2, 4, 8, 1,
											    4, 8, 1, 2,
											    8, 1, 2, 4 };

			uint[] checksum = new uint[] { 0, 0, 0, 0 };
			for ( uint i = 0; i < 0x39D00; ++i ) {
				checksum[0] += (uint)( File[i] * checksumTable[( i + 0 ) % 16] );
				checksum[1] += (uint)( File[i] * checksumTable[( i + 1 ) % 16] );
				checksum[2] += (uint)( File[i] * checksumTable[( i + 2 ) % 16] );
				checksum[3] += (uint)( File[i] * checksumTable[( i + 3 ) % 16] );
			}

			for ( int i = 0; i < 4; ++i ) {
				Util.CopyByteArrayPart( BitConverter.GetBytes( checksum[i] ), 0, File, 0x39E00 + i * 4, 4 );
			}
		}

		public void WriteFile( string Filename ) {
			WriteClassicPuzzles();
			RecalculateChecksum();
			System.IO.File.WriteAllBytes( Filename, File );
		}
	}
}
