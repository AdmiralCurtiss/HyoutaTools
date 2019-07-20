using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HyoutaUtils;

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
		public uint NewPosition;
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
		private uint ScriptSize;
		private uint StartOfScriptSection;

		private List<bscrFunc> FunctionCalls;
		public List<bscrString> Strings;
		private List<uint> Script;

		private bool LoadFile( byte[] File ) {
			FunctionCalls = new List<bscrFunc>();
			Strings = new List<bscrString>();

			UnknownMaybeType = BitConverter.ToUInt16( File, 0x04 );
			Filename = TextUtils.GetTextUTF8( File, 0x06 );
			Filesize = BitConverter.ToUInt32( File, 0x24 );
			FunctionCount = BitConverter.ToUInt32( File, 0x28 );
			StartOfFunctionSection = BitConverter.ToUInt32( File, 0x2C );
			TextCount = BitConverter.ToUInt32( File, 0x30 );
			StartOfTextSection = BitConverter.ToUInt32( File, 0x34 );
			ScriptSize = BitConverter.ToUInt32( File, 0x38 );
			StartOfScriptSection = BitConverter.ToUInt32( File, 0x3C );

			int pos;
			for ( pos = (int)StartOfFunctionSection; pos < StartOfTextSection; ) {
				bscrFunc f = new bscrFunc();
				f.Size = BitConverter.ToUInt16( File, pos );
				f.Unknown = BitConverter.ToUInt16( File, pos + 2 );
				f.Name = TextUtils.GetTextUTF8( File, pos + 4 );
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
				s.String = TextUtils.GetTextUTF8( File, pos, out nullLoc );
				s.Position = (uint)pos;

				Strings.Add( s );
				pos = nullLoc + 1;
			}

			Script = new List<uint>( (int)ScriptSize );
			for ( int i = 0; i < ScriptSize; i += 4 ) {
				uint val = BitConverter.ToUInt32( File, (int)( StartOfScriptSection + i ) );
				Script.Add( val );
			}


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
			uint OldStartOfTextSectio = StartOfTextSection;
			StartOfTextSection = (uint)File.Count;
			foreach ( bscrString s in Strings ) {
				byte[] b = Encoding.UTF8.GetBytes( s.String );
				s.NewPosition = (uint)File.Count;
				File.AddRange( b );
				File.Add( 0x00 );
			}
			while ( File.Count % 0x4 != 0 ) { File.Add( 0x00 ); }

			// script
			StartOfScriptSection = (uint)File.Count;
			for ( int i = 0; i < Script.Count; ++i ) {
				uint u = Script[i];
				File.AddRange( BitConverter.GetBytes( u ) );

				// 1A F0 F1 5A seems to indicate the next uint is a pointer to text, this is hacky but might work
				if ( u == 0x5AF1F01A ) {
					++i;
					uint OldPointer = Script[i];
					uint OldPointerAdjusted = OldPointer + OldStartOfTextSectio;
					bool Found = false;
					foreach ( bscrString s in Strings ) {
						if ( s.Position == OldPointerAdjusted ) {
							uint NewPtr = s.NewPosition - StartOfTextSection;
							File.AddRange( BitConverter.GetBytes( NewPtr ) );
							Found = true;
							break;
						}
					}
					if ( !Found ) {
						throw new Exception( "Didn't find pointer for text!\nbscr at 0x" + File.Count.ToString( "X8" ) );
					}
				}
			}

			Filesize = (uint)File.Count;

			// pad
			while ( File.Count % 0x10 != 0 ) { File.Add( 0x00 ); }

			FunctionCount = (uint)FunctionCalls.Count;
			TextCount = (uint)Strings.Count;
			ScriptSize = (uint)( Script.Count * 4 );

			// header
			File[0] = (byte)'b'; File[1] = (byte)'s'; File[2] = (byte)'c'; File[3] = (byte)'r';
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( UnknownMaybeType ), 0, File, 0x04, 2 );
			byte[] FilenameBytes = Encoding.UTF8.GetBytes( Filename );
			ArrayUtils.CopyByteArrayPart( FilenameBytes, 0, File, 0x06, FilenameBytes.Length );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( Filesize ), 0, File, 0x24, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( FunctionCount ), 0, File, 0x28, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( StartOfFunctionSection ), 0, File, 0x2C, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( TextCount ), 0, File, 0x30, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( StartOfTextSection ), 0, File, 0x34, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( ScriptSize ), 0, File, 0x38, 4 );
			ArrayUtils.CopyByteArrayPart( BitConverter.GetBytes( StartOfScriptSection ), 0, File, 0x3C, 4 );

			System.IO.File.WriteAllBytes( Path, File.ToArray() );
		}
	}
}
