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
				f.Params = new uint[( f.Size - 0x24 ) / 4];
				for ( int i = 0; i < f.Params.Length; ++i ) {
					f.Params[i] = BitConverter.ToUInt32( File, pos + 0x24 + i * 4 );
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
			Util.CopyByteArrayPart( File, (int)StartOfTextureSection, TexturesProbably, 0, TexturesProbably.Length );

			return true;
		}

		public void CreateFile( string Path ) {
			List<byte> File = new List<byte>();

			// header, populate later
			for ( int i = 0; i < 0x40; ++i ) { File.Add( 0x00 ); }

			// function calls
			StartOfFunctionSection = (uint)File.Count;
			foreach ( bscrFunc f in FunctionCalls ) {
				File.AddRange( BitConverter.GetBytes( f.Size ) );
				File.AddRange( BitConverter.GetBytes( f.Unknown ) );

				byte[] NameBytes = Encoding.UTF8.GetBytes( f.Name );
				File.AddRange( NameBytes );
				for ( int i = NameBytes.Length; i < 0x20; ++i ) { File.Add( 0x00 ); }

				foreach ( uint param in f.Params ) {
					File.AddRange( BitConverter.GetBytes( param ) );
				}
			}

			// text
			StartOfTextSection = (uint)File.Count;
			foreach ( bscrString s in Strings ) {
				byte[] b = Encoding.UTF8.GetBytes( s.String );
				File.AddRange( b );
				File.Add( 0x00 );
			}
			while ( File.Count % 0x4 != 0 ) { File.Add( 0x00 ); }

			// textures
			StartOfTextureSection = (uint)File.Count;
			File.AddRange( TexturesProbably );

			Filesize = (uint)File.Count;

			// pad
			while ( File.Count % 0x10 != 0 ) { File.Add( 0x00 ); }

			FunctionCount = (uint)FunctionCalls.Count;
			TextCount = (uint)Strings.Count;
			//Unknown5;

			// header
			File[0] = (byte)'b'; File[1] = (byte)'s'; File[2] = (byte)'c'; File[3] = (byte)'r';
			Util.CopyByteArrayPart( BitConverter.GetBytes( UnknownMaybeType ), 0, File, 0x04, 2 );
			byte[] FilenameBytes = Encoding.UTF8.GetBytes( Filename );
			Util.CopyByteArrayPart( FilenameBytes, 0, File, 0x06, FilenameBytes.Length );
			Util.CopyByteArrayPart( BitConverter.GetBytes( Filesize ), 0, File, 0x24, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( FunctionCount ), 0, File, 0x28, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( StartOfFunctionSection ), 0, File, 0x2C, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( TextCount ), 0, File, 0x30, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( StartOfTextSection ), 0, File, 0x34, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( Unknown5 ), 0, File, 0x38, 4 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( StartOfTextureSection ), 0, File, 0x3C, 4 );

			System.IO.File.WriteAllBytes( Path, File.ToArray() );
		}
	}
}
