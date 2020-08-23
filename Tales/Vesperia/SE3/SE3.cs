using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.SE3 {
	public class SE3 {
		public SE3( string filename, EndianUtils.Endianness? endian, TextUtils.GameTextEncoding encoding ) {
			if ( !LoadFile( new System.IO.FileStream( filename, FileMode.Open ), endian, encoding ) ) {
				throw new Exception( "Loading SE3 failed!" );
			}
		}

		public SE3( Stream stream, EndianUtils.Endianness? endian, TextUtils.GameTextEncoding encoding ) {
			if ( !LoadFile( stream, endian, encoding ) ) {
				throw new Exception( "Loading SE3 failed!" );
			}
		}

		private uint DataBegin;
		private uint FileCount;
		private List<string> Filenames;
		private Stream Data;

		private bool LoadFile( Stream stream, EndianUtils.Endianness? endianParam, TextUtils.GameTextEncoding encoding ) {
			EndianUtils.Endianness endian;
			if ( endianParam == null ) {
				uint magic = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
				if ( magic == 0x53453320 ) {
					endian = EndianUtils.Endianness.BigEndian;
				} else if (magic == 0x20334553 ) {
					endian = EndianUtils.Endianness.LittleEndian;
				} else {
					Console.WriteLine( "Invalid magic: " + magic );
					return false;
				}
			} else {
				endian = endianParam.Value;
				uint magic = stream.ReadUInt32().FromEndian( endian );
				if ( magic != 0x53453320 ) {
					Console.WriteLine( "Invalid magic: " + magic );
					return false;
				}
			}

			stream.DiscardBytes( 8 ); // unknown
			DataBegin = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 4 ); // unknown
			FileCount = stream.ReadUInt32().FromEndian( endian );
			Filenames = new List<string>( (int)FileCount );
			for ( uint i = 0; i < FileCount; ++i ) {
				Filenames.Add( stream.ReadSizedString( 48, encoding ).TrimNull() );
			}

			Data = stream;

			return true;
		}

		// TODO: We could parse/extract the nub directly to keep filenames, but eh, effort...

		public void ExtractToNub(string targetName) {
			using (var fs = new FileStream(targetName, FileMode.Create)) {
				ExtractToNub(fs);
			}
		}

		public void ExtractToNub(Stream targetStream) {
			Data.Position = DataBegin;
			StreamUtils.CopyStream(Data, targetStream, Data.Length - DataBegin);
		}
	}
}
