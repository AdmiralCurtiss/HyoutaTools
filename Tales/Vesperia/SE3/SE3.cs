using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.SE3 {
	public class SE3 {
		public SE3( string filename, Util.Endianness endian, Util.GameTextEncoding encoding ) {
			if ( !LoadFile( new System.IO.FileStream( filename, FileMode.Open ), endian, encoding ) ) {
				throw new Exception( "Loading SE3 failed!" );
			}
		}

		public SE3( Stream stream, Util.Endianness endian, Util.GameTextEncoding encoding ) {
			if ( !LoadFile( stream, endian, encoding ) ) {
				throw new Exception( "Loading SE3 failed!" );
			}
		}

		private uint DataBegin;
		private uint FileCount;
		private List<string> Filenames;
		private Stream Data;

		private bool LoadFile( Stream stream, Util.Endianness endian, Util.GameTextEncoding encoding ) {
			string magic = stream.ReadAscii( 4 );
			if ( magic != "SE3 " ) {
				return false;
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

		public void ExtractToNub( string targetName ) {
			Data.Position = DataBegin;
			using ( Stream fs = new FileStream( targetName, FileMode.Create ) ) {
				Util.CopyStream( Data, fs, Data.Length - DataBegin );
			}
		}
	}
}
