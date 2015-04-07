using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Sting {
	public class BlazeUnionScriptFile {
		public BlazeUnionScriptFile( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading BlazeUnionScriptFile failed!" );
				}
			}
		}

		public BlazeUnionScriptFile( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading BlazeUnionScriptFile failed!" );
			}
		}

		public List<BlazeUnionScriptFileSection> Sections;

		private bool LoadFile( Stream stream ) {
			// okay this is a super weird file format
			// first uint is a pointer to a structure of five other pointers at the end of the file
			// which each point to a file section (ptrs may be null to say "section doesn't exist")
			// each of these sections starts with nine pointers that I don't quite understand yet but seem to point
			// to some sort of bytecode, and then after those nine pointers is a list of nullterminated shiftjis strings
			// with some extra special control codes in the 0x01-0x04 range
			// known control codes:
			//   0x01 -> newline
			//   0x0200yyyy -> loads yyyy as voice clip
			// these strings are referenced in the bytecode below, so that must be updated if the strings change
			// string pointers can be found by the bytesequence 46 07 00, which is followed by a 4 byte pointer

			uint sectionStructurePointer = stream.ReadUInt32();
			List<uint> sectionPointers = new List<uint>();
			stream.Position = sectionStructurePointer;
			while ( stream.Position < stream.Length ) { // no idea if it's always exactly five so let's not assume
				sectionPointers.Add( stream.ReadUInt32() );
			}

			Sections = new List<BlazeUnionScriptFileSection>();
			var sortedAndValidSectionPtrs = sectionPointers.Where( x => x > 0 ).ToList();
			sortedAndValidSectionPtrs.Sort();
			sortedAndValidSectionPtrs.Add( sectionStructurePointer );
			for ( int i = 0; i < sortedAndValidSectionPtrs.Count - 1; ++i ) {
				if ( sortedAndValidSectionPtrs[i] == sectionStructurePointer ) {
					throw new Exception( "Assumption about file format incorrect, SectionStruct is not after all sections!" );
				}

				stream.Position = sortedAndValidSectionPtrs[i];
				Sections.Add( new BlazeUnionScriptFileSection( stream, sortedAndValidSectionPtrs[i + 1] ) );
			}

			return true;
		}
	}
}
