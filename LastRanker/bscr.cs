using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.LastRanker {
	public class bscrFunc {
		public ushort Size;
		public ushort Unknown;
		public string Name;
		public uint[] Params;
		public override string ToString() { return Name; }
	}
	public class bscrString {
		public string String;
		public uint Position;
		public override string ToString() { return String; }
	}

	public class bscr {
		public bscr( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}
		public bscr( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}


		private ushort UnknownMaybeType;
		private string Filename;

		private uint Filesize;
		private uint FunctionCount;
		private uint StartOfFunctionSection;

		private uint TextCount;
		private uint StartOfTextSection;
		private uint Unknown5;
		private uint StartOfTextureSection;

		private List<bscrFunc> FunctionCalls;
		public List<bscrString> Strings;
		private byte[] TexturesProbably;

		private bool LoadFile( byte[] File ) {
			FunctionCalls = new List<bscrFunc>();
			Strings = new List<bscrString>();

			UnknownMaybeType = BitConverter.ToUInt16( File, 0x04 );
			Filename = Util.GetTextUTF8( File, 0x06 );
			Filesize = BitConverter.ToUInt32( File, 0x24 );
			FunctionCount = BitConverter.ToUInt32( File, 0x28 );
			StartOfFunctionSection = BitConverter.ToUInt32( File, 0x2C );
			TextCount = BitConverter.ToUInt32( File, 0x30 );
			StartOfTextSection = BitConverter.ToUInt32( File, 0x34 );
			Unknown5 = BitConverter.ToUInt32( File, 0x38 );
			StartOfTextureSection = BitConverter.ToUInt32( File, 0x3C );

			int pos;
			for ( pos = (int)StartOfFunctionSection; pos < StartOfTextSection; ) {
				bscrFunc f = new bscrFunc();
				f.Size = BitConverter.ToUInt16( File, pos );
				f.Unknown = BitConverter.ToUInt16( File, pos + 2 );
				f.Name = Util.GetTextUTF8( File, pos + 4 );
				f.Params = new uint[ (f.Size - 0x24) / 4 ];
				for ( int i = 0; i < f.Params.Length; ++i ) {
					f.Params[i] = BitConverter.ToUInt32( File, pos + 0x24 + i*4 );
				}

				FunctionCalls.Add( f );
				pos += f.Size;
			}

			pos = (int)StartOfTextSection;
			for ( uint i = 0; i < TextCount; ++i ) {
				bscrString s = new bscrString();
				int nullLoc;
				s.String = Util.GetTextUTF8( File, pos, out nullLoc );
				s.Position = (uint)pos;

				Strings.Add( s );
				pos = nullLoc + 1;
			}

			TexturesProbably = new byte[Filesize - StartOfTextureSection];
			Util.CopyByteArrayPart(File, (int)StartOfTextureSection, TexturesProbably, 0, TexturesProbably.Length);

			return true;
		}
	}
}
