using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.HalfMinuteHeroSecond {
	public class S2ARHeaderFileInfo {
		public uint Hash;
		public uint Filesize;
	}

	public class S2AR {
		static void Extract( FileStream file, String destination ) {
			System.IO.Directory.CreateDirectory( destination );

			uint magic = file.ReadUInt32();
			uint filecount = file.ReadUInt32();

			var FileInfos = new S2ARHeaderFileInfo[filecount];
			for ( int i = 0; i < filecount; ++i ) {
				FileInfos[i] = new S2ARHeaderFileInfo();
				FileInfos[i].Hash = file.ReadUInt32();
				FileInfos[i].Filesize = file.ReadUInt32();
			}

			for ( int i = 0; i < filecount; ++i ) {
				FileStream newFile = new FileStream( System.IO.Path.Combine( destination, i.ToString( "D4" ) ), FileMode.Create );
				Util.CopyStream( file, newFile, (int) FileInfos[i].Filesize );
				newFile.Close();
			}

			return;
		}

		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: HMH2_S2ARex file.s2a" );
				return -1;
			}

			string filename = args[0];
			Extract( new FileStream( filename, FileMode.Open ), filename + ".ex" );
			return 0;
		}
	}
}
