using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.SpkdUnpack {
	class Program {
		static void Execute( string[] args ) {
			//args = new string[] { @"c:\Users\Georg\Documents\Visual Studio 2008\Projects\slz\slz\bin\Release\STRCONFIG.STP" };

			if ( args.Length != 1 ) {
				Console.WriteLine( "Usage: SPKDunpack file" );
				return;
			}

			byte[] spkd = File.ReadAllBytes( args[0] );

			uint FileAmount = Util.SwapEndian( BitConverter.ToUInt32( spkd, 4 ) );
			uint BlockSize = Util.SwapEndian( BitConverter.ToUInt32( spkd, 12 ) );
			FileData[] FileInfos = new FileData[FileAmount + 1];

			for ( int i = 0; i < FileAmount; i++ ) {
				FileInfos[i] = new FileData();
				FileInfos[i].Name = ASCIIEncoding.GetEncoding( 0 ).GetString( spkd, ( i + 1 ) * (int)BlockSize, 16 );
				FileInfos[i].Name = FileInfos[i].Name.Substring( 0, FileInfos[i].Name.IndexOf( '\0' ) );
				FileInfos[i].Unknown = Util.SwapEndian( BitConverter.ToUInt32( spkd, ( i + 1 ) * (int)BlockSize + 16 ) );
				FileInfos[i].FileStart1 = Util.SwapEndian( BitConverter.ToUInt32( spkd, ( i + 1 ) * (int)BlockSize + 20 ) );
				FileInfos[i].FileStart2 = Util.SwapEndian( BitConverter.ToUInt32( spkd, ( i + 1 ) * (int)BlockSize + 24 ) );
				FileInfos[i].Something = Util.SwapEndian( BitConverter.ToUInt32( spkd, ( i + 1 ) * (int)BlockSize + 28 ) );
			}
			FileInfos[FileAmount] = new FileData();
			FileInfos[FileAmount].FileStart1 = (UInt32)spkd.Length;

			DirectoryInfo d = System.IO.Directory.CreateDirectory( args[0] + ".ext" );
			for ( int i = 0; i < FileAmount; i++ ) {
				int Filesize = (int)FileInfos[i + 1].FileStart1 - (int)FileInfos[i].FileStart1;
				byte[] b = new byte[Filesize];
				Buffer.BlockCopy( spkd, (int)FileInfos[i].FileStart1, b, 0, Filesize );
				File.WriteAllBytes( d.FullName + "\\" + FileInfos[i].Name, b );
			}
		}


		class FileData {
			public String Name;
			public UInt32 Unknown;
			public UInt32 FileStart1;
			public UInt32 FileStart2; // Why is this twice?
			public UInt32 Something; // Honestly I have no clue.
		}
	}
}
