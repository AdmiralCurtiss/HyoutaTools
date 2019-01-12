using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.ScenarioFile {
	public class ScenarioFile {
		public ScenarioFile() { }

		public ScenarioFile( String filename, Util.GameTextEncoding encoding, Util.Endianness endian, Util.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, FileAccess.Read, FileShare.Read ) ) {
				if ( !LoadFile( stream, encoding, endian, bits ) ) {
					throw new Exception( "Loading ScenarioFile failed!" );
				}
			}
		}

		public ScenarioFile( Stream stream, Util.GameTextEncoding encoding, Util.Endianness endian, Util.Bitness bits ) {
			if ( !LoadFile( stream, encoding, endian, bits ) ) {
				throw new Exception( "Loading ScenarioFile failed!" );
			}
		}

		public List<ScenarioFileEntry> EntryList;
		public Website.ScenarioData Metadata;

		private bool LoadFile( Stream stream, Util.GameTextEncoding encoding, Util.Endianness endian, Util.Bitness bits ) {
			uint magic = stream.ReadUInt32().SwapEndian();
			uint headerSize = stream.ReadUInt32().FromEndian( endian );
			stream.ReadUInt32().FromEndian( endian );
			uint textStart = stream.ReadUInt32().FromEndian( endian );

			stream.ReadUInt32().FromEndian( endian );
			stream.ReadUInt32().FromEndian( endian );
			uint textLength = stream.ReadUInt32().FromEndian( endian );
			stream.ReadUInt32().FromEndian( endian );

			EntryList = FindText( stream, textStart, encoding, endian, bits );

			return true;
		}

		private static List<ScenarioFileEntry> FindText( Stream stream, uint textStart, Util.GameTextEncoding encoding, Util.Endianness endian, Util.Bitness bits ) {
			var list = new List<ScenarioFileEntry>();

			uint toMatch = ( 0x040C0000 + 2 * 4 + 4 * bits.NumberOfBytes() );
			while ( stream.Position < textStart ) {
				uint identifyingBytes = stream.ReadUInt32().FromEndian( endian );
				if ( identifyingBytes == toMatch ) {
					uint pos = (uint)stream.Position;

					uint pointerToText = stream.ReadUInt32().FromEndian( endian ) + textStart;
					
					stream.Position = pointerToText;
					stream.ReadUInt32().FromEndian( endian );
					stream.ReadUInt32().FromEndian( endian );
					ulong jpNamePtr = stream.ReadUInt( bits, endian );
					ulong jpTextPtr = stream.ReadUInt( bits, endian );
					ulong enNamePtr = stream.ReadUInt( bits, endian );
					ulong enTextPtr = stream.ReadUInt( bits, endian );
					stream.Position = (long)( jpNamePtr + textStart ); string jpName = stream.ReadNulltermString( encoding );
					stream.Position = (long)( jpTextPtr + textStart ); string jpText = stream.ReadNulltermString( encoding );
					stream.Position = (long)( enNamePtr + textStart ); string enName = stream.ReadNulltermString( encoding );
					stream.Position = (long)( enTextPtr + textStart ); string enText = stream.ReadNulltermString( encoding );

					var entry = new ScenarioFileEntry() { Pointer = pos, JpName = jpName, EnName = enName, JpText = jpText, EnText = enText };
					list.Add( entry );

					stream.Position = pos + 4;
				}
			}

			return list;
		}

		public ScenarioFile CloneShallow() {
			ScenarioFile clone = new ScenarioFile();
			
			clone.EntryList = new List<ScenarioFileEntry>( this.EntryList.Count );
			clone.EntryList.AddRange( this.EntryList );
			clone.Metadata = Metadata;

			return clone;
		}

		public override string ToString() {
			return Metadata.EpisodeId + ": " + EntryList.Count + " entries";
		}
	}
}
