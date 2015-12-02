using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Tales.Vesperia.xma {
	public class FixHeader {
		public static int Execute( List<string> args ) {
			if ( args.Count == 0 ) {
				Console.WriteLine( "This is intended to help extracting skit audio from the Xbox 360 game files." );
				Console.WriteLine( "Do the following in order:" );
				Console.WriteLine( "-- unpack chat.svo (FPS4 archive, with HyoutaTools -> ToVfps4e)" );
				Console.WriteLine( "-- decompress individual skit with xbdecompress" );
				Console.WriteLine( "-- unpack skit (FPS4 archive, with HyoutaTools -> ToVfps4e)" );
				Console.WriteLine( "-- cut SE3 header from audio file to get a nub archive" );
				Console.WriteLine( "   (file 0004, seems to be 0x800 bytes for skits but can be bigger, first four bytes of new file should be 0x00020100)" );
				Console.WriteLine( "-- extract nub archive with NUBExt r12beta" );
				Console.WriteLine( "-- this gives you an \"xma\" file that isn't actually an xma, run this tool on it" );
				Console.WriteLine( "-- resulting file is a valid enough xma file that can be converted to WAV with \"toWav\"" );
				return -1;
			}

			string filename = args[0];
			using ( var source = new FileStream( filename, FileMode.Open ) ) {
				using ( var dest = new FileStream( filename + "-real.xma", FileMode.Create ) ) {
					source.Position = 0x100;
					int dataLength = (int)( source.Length - source.Position );

					dest.WriteAscii( "RIFF" );
					dest.WriteUInt32( (uint)dataLength + 0x34 );
					dest.WriteAscii( "WAVE" );
					dest.WriteAscii( "fmt " );

					dest.WriteUInt32( 0x20 );

					source.Position = 0xBC;
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );
					dest.WriteByte( (byte)source.ReadByte() );
					dest.WriteByte( (byte)source.ReadByte() );
					dest.WriteUInt32( source.ReadUInt32().SwapEndian() );
					dest.WriteUInt32( source.ReadUInt32().SwapEndian() );
					dest.WriteUInt32( source.ReadUInt32().SwapEndian() );
					dest.WriteUInt32( source.ReadUInt32().SwapEndian() );
					dest.WriteByte( (byte)source.ReadByte() );
					dest.WriteByte( (byte)source.ReadByte() );
					dest.WriteUInt16( source.ReadUInt16().SwapEndian() );

					dest.WriteAscii( "data" );
					dest.WriteUInt32( (uint)dataLength );

					source.Position = 0x100;
					Util.CopyStream( source, dest, dataLength );
				}
			}

			return 0;
		}
	}
}
