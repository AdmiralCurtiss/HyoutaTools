using System;
using System.Collections.Generic;
using HyoutaTools.FileContainer;
using HyoutaPluginBase;
using HyoutaPluginBase.FileContainer;
using HyoutaUtils;
using HyoutaUtils.Streams;

namespace HyoutaTools.Trophy {
	public class TrophyTrpFile {
		public string Filename { get; private set; }

		public uint Unknown1 { get; private set; }
		public uint Start { get; private set; }
		public uint Unknown3 { get; private set; }
		public uint Length { get; private set; }

		public uint Unknown5 { get; private set; }
		public uint Unknown6 { get; private set; }
		public uint Unknown7 { get; private set; }
		public uint Unknown8 { get; private set; }

		public TrophyTrpFile( DuplicatableStream stream, EndianUtils.Endianness endian ) {
			Filename = stream.ReadAscii( 0x20 ).TrimNull();
			Unknown1 = stream.ReadUInt32().FromEndian( endian );
			Start = stream.ReadUInt32().FromEndian( endian );
			Unknown3 = stream.ReadUInt32().FromEndian( endian );
			Length = stream.ReadUInt32().FromEndian( endian );
			Unknown5 = stream.ReadUInt32().FromEndian( endian );
			Unknown6 = stream.ReadUInt32().FromEndian( endian );
			Unknown7 = stream.ReadUInt32().FromEndian( endian );
			Unknown8 = stream.ReadUInt32().FromEndian( endian );
		}
	}

	public class TrophyTrp : ContainerBase {
		private DuplicatableStream Stream;
		private EndianUtils.Endianness Endian;

		private uint Magic;
		private uint Unknown2;
		private uint Unknown3;
		private uint Filesize;

		private uint Filecount;
		private uint Unknown6; // could be first file start or single file header size?
		private uint Unknown7;
		private uint Unknown8;

		private uint Unknown9;
		private uint Unknown10;
		private uint Unknown11;
		private uint Unknown12;

		private uint Unknown13;
		private uint Unknown14;
		private uint Unknown15;
		private uint Unknown16;

		public TrophyTrp( HyoutaPluginBase.DuplicatableStream stream, EndianUtils.Endianness endian = EndianUtils.Endianness.BigEndian ) {
			Stream = stream.Duplicate();
			Endian = endian;
			try {
				Stream.ReStart();
				Magic = Stream.ReadUInt32().FromEndian( EndianUtils.Endianness.LittleEndian );
				if ( Magic != 0x004DA2DC ) {
					throw new Exception( "invalid magic" );
				}
				Unknown2 = Stream.ReadUInt32().FromEndian( endian );
				Unknown3 = Stream.ReadUInt32().FromEndian( endian );
				Filesize = Stream.ReadUInt32().FromEndian( endian );

				Filecount = Stream.ReadUInt32().FromEndian( endian );
				Unknown6 = Stream.ReadUInt32().FromEndian( endian );
				Unknown7 = Stream.ReadUInt32().FromEndian( endian );
				Unknown8 = Stream.ReadUInt32().FromEndian( endian );

				Unknown9 = Stream.ReadUInt32().FromEndian( endian );
				Unknown10 = Stream.ReadUInt32().FromEndian( endian );
				Unknown11 = Stream.ReadUInt32().FromEndian( endian );
				Unknown12 = Stream.ReadUInt32().FromEndian( endian );

				Unknown13 = Stream.ReadUInt32().FromEndian( endian );
				Unknown14 = Stream.ReadUInt32().FromEndian( endian );
				Unknown15 = Stream.ReadUInt32().FromEndian( endian );
				Unknown16 = Stream.ReadUInt32().FromEndian( endian );
			} finally {
				Stream.End();
			}
		}

		public override void Dispose() {
			Stream.Dispose();
		}

		private TrophyTrpFile GetFileByIndex( long index ) {
			if ( index >= 0 && index < Filecount ) {
				Stream.Position = 0x40 + index * 0x40;
				return new TrophyTrpFile( Stream, Endian );
			}
			return null;
		}

		public override INode GetChildByIndex( long index ) {
			TrophyTrpFile f = GetFileByIndex( index );
			if ( f != null ) {
				return new FileFromStream( new PartialStream( Stream, f.Start, f.Length ) );
			}
			return null;
		}

		public override INode GetChildByName( string name ) {
			for ( uint i = 0; i < Filecount; ++i ) {
				TrophyTrpFile f = GetFileByIndex( i );
				if ( f.Filename == name ) {
					return new FileFromStream( new PartialStream( Stream, f.Start, f.Length ) );
				}
			}
			return null;
		}

		public override IEnumerable<string> GetChildNames() {
			for ( uint i = 0; i < Filecount; ++i ) {
				yield return GetFileByIndex( i ).Filename;
			}
		}
	}
}
