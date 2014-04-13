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
		static void Extract( FileStream file, String destination, Dictionary<uint, string> filenameHashDict = null ) {
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
				string filename;
				if ( filenameHashDict != null && filenameHashDict.ContainsKey( FileInfos[i].Hash ) ) {
					filename = System.IO.Path.Combine( destination, filenameHashDict[FileInfos[i].Hash] );
				} else {
					filename = System.IO.Path.Combine( destination, i.ToString( "D4" ) + Util.GuessFileExtension( file ) );
				}
				FileStream newFile = new FileStream( filename, FileMode.Create );
				Util.CopyStream( file, newFile, (int)FileInfos[i].Filesize );
				newFile.Close();
			}

			return;
		}

		public static int ExecuteExtract( List<string> args ) {
			if ( args.Count < 1 ) {
				Console.WriteLine( "Usage: HMH2_S2ARex file.s2a [HMH2.exe]" );
				return -1;
			}

			Dictionary<uint, string> filenameHashDict = null;
			if ( args.Count >= 2 ) {
				string exeFilename = args[1];
				var exe = new FileStream( exeFilename, FileMode.Open );

				filenameHashDict = new Dictionary<uint, string>();
				var crcCalc = new SevenZip.CRC();
				while ( exe.Position < exe.Length ) {
					var str = exe.ReadAsciiNullterm();
					if ( str != "" ) {
						crcCalc.Init();
						var bytes = Encoding.ASCII.GetBytes( str );
						crcCalc.Update( bytes, 0, (uint)bytes.Length );
						uint crc32 = crcCalc.GetDigest();
						if ( !filenameHashDict.ContainsKey( crc32 ) ) {
							filenameHashDict.Add( crc32, str );
						}
					}
				}

				exe.Close();
			}

			string filename = args[0];
			Extract( new FileStream( filename, FileMode.Open ), filename + ".ex", filenameHashDict );
			return 0;
		}
	}
}
