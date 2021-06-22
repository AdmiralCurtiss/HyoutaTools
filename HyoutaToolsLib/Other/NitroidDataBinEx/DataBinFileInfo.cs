using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.NitroidDataBinEx {
	class DataBinFileInfo {
		byte[] DataBin;

		String Filepath;

		uint Unknown1;
		uint Unknown2;
		uint Location;
		uint Length;

		public DataBinFileInfo( byte[] DataBin, int Offset ) {
			this.DataBin = DataBin;

			Filepath = Encoding.ASCII.GetString( DataBin, Offset, 0x40 ).Trim( new char[] { '\0' } );
			Unknown1 = BitConverter.ToUInt32( DataBin, Offset + 0x40 );
			Unknown2 = BitConverter.ToUInt32( DataBin, Offset + 0x44 );
			Location = BitConverter.ToUInt32( DataBin, Offset + 0x48 );
			Length = BitConverter.ToUInt32( DataBin, Offset + 0x4C );
		}

		public void ExtractFile( String Path ) {
			Console.WriteLine( "Dumping: " + Filepath );

			//byte[] DumpFile = new byte[Length];
			//Buffer.BlockCopy(DataBin, Location, DumpFile, 0, Length);

			//byte[] DumpFile = DataBin.Skip((int)Location).Take((int)Length).ToArray();

			String FPath = Path + Filepath;
			CreateDirectory( FPath.Substring( 0, FPath.LastIndexOf( '\\' ) ) );
			//System.IO.File.WriteAllBytes(FPath, DumpFile);
			System.IO.FileStream stream = System.IO.File.Create( FPath );
			stream.Write( DataBin, (int)Location, (int)Length );
			stream.Close();
		}

		public static void CreateDirectory( String Directory ) {
			System.IO.Directory.CreateDirectory( Directory );
			return;
		}
	}
}
