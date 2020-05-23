using HyoutaPluginBase;
using HyoutaTools.Generic;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.GameCube {
	// https://wiibrew.org/wiki/DOL
	public class Dol : IRomMapper {
		private DuplicatableStream Stream;
		public List<uint> FileOffsets; // 18 sections; 7 code, 11 data
		public List<uint> LoadingAddress;
		public List<uint> SectionSizes;
		public uint BssAddress;
		public uint BssSize;
		public uint EntryPoint;

		public Dol( DuplicatableStream stream ) {
			Stream = stream.Duplicate();
			Stream.Position = 0;
			FileOffsets = new List<uint>( 18 );
			LoadingAddress = new List<uint>( 18 );
			SectionSizes = new List<uint>( 18 );
			for ( int i = 0; i < 18; ++i ) {
				FileOffsets.Add( Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian ) );
			}
			for ( int i = 0; i < 18; ++i ) {
				LoadingAddress.Add( Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian ) );
			}
			for ( int i = 0; i < 18; ++i ) {
				SectionSizes.Add( Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian ) );
			}
			BssAddress = Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
			BssSize = Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
			EntryPoint = Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
		}

		public DuplicatableStream DuplicateStream() {
			return Stream.Duplicate();
		}

		public bool TryMapRamToRom( ulong ramAddress, out ulong value ) {
			for ( int i = 0; i < 18; ++i ) {
				uint begin = LoadingAddress[i];
				uint end = begin + SectionSizes[i];
				if ( ramAddress >= begin && ramAddress < end ) {
					value = ( ramAddress - begin ) + FileOffsets[i];
					return true;
				}
			}
			value = 0;
			return false;
		}

		public bool TryMapRomToRam( ulong romAddress, out ulong value ) {
			for ( int i = 0; i < 18; ++i ) {
				uint begin = FileOffsets[i];
				uint end = begin + SectionSizes[i];
				if ( romAddress >= begin && romAddress < end ) {
					value = ( romAddress - begin ) + LoadingAddress[i];
					return true;
				}
			}
			value = 0;
			return false;
		}
	}
}
