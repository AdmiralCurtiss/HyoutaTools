using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools.Tales.Xillia.TldatExtract;

namespace HyoutaTools.Tales.Xillia.TldatPack {
	class Program {
		public static int Execute( List<string> args ) {
			if ( args.Count != 4 ) {
				Console.WriteLine( "Usage: OldTOFHDB Folder NewTLDAT NewTOFHDB" );
				return -1;
			}

			String OldTOFHDB = args[0];
			String ExtractFolder = args[1];
			String TLDAT = args[2];
			String TOFHDB = args[3];
			TOFHDBheader header = new TOFHDBheader( OldTOFHDB );

			// collect files
			SortedDictionary<uint, string> filenameMap = new SortedDictionary<uint, string>();
			foreach ( string dir in Directory.EnumerateDirectories( ExtractFolder ) ) {
				foreach ( string file in Directory.EnumerateFiles( dir ) ) {
					string filenumstr = Path.GetFileNameWithoutExtension( file );
					uint num = Util.ParseDecOrHex( filenumstr );
					filenameMap.Add( num, file );
				}
			}

			if ( filenameMap.Count != header.FileArray.Count ) {
				Console.WriteLine( "File count mismatch!" );
				return 1;
			}
			if ( filenameMap.First().Key != 1 || filenameMap.Last().Key != header.FileArray.Count ) {
				Console.WriteLine( "Filenames are wrong!" );
				return 2;
			}

			// write files and populate header
			using ( FileStream fsData = File.Open( TLDAT, FileMode.Create, FileAccess.ReadWrite ) ) {
				foreach ( var f in filenameMap ) {
					using ( FileStream fs = new FileStream( f.Value, FileMode.Open ) ) {
						header.FileArray[(int)( f.Key - 1 )].Filesize = (ulong)fs.Length;
						header.FileArray[(int)( f.Key - 1 )].CompressedSize = (ulong)fs.Length;
						header.FileArray[(int)( f.Key - 1 )].Offset = (ulong)fsData.Position;

						// check if TLZC compressed and write uncompressed size if so
						if ( fs.PeekUInt32() == 0x435A4C54 ) {
							fs.DiscardBytes( 12 );
							header.FileArray[(int)( f.Key - 1 )].Filesize = fs.ReadUInt32();
							fs.Position = 0;
						}

						fs.CopyTo( fsData );
						fs.Close();
					}
				}
				fsData.Close();
			}

			// write header
			using ( FileStream fs = new FileStream( TOFHDB, FileMode.Create, FileAccess.ReadWrite ) ) {
				header.Write( fs );
				fs.Close();
			}

			return 0;
		}
	}
}
