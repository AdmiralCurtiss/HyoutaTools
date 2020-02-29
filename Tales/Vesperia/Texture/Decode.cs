using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.Texture {
	public class Decode {
		public static int Execute( List<string> args ) {
			if ( args.Count != 1 && args.Count != 2 ) {
				Console.WriteLine( "Usage: texture.txm texture.txv" );
				Console.WriteLine( "   or: texture.tex (FPS4 file containing TXM and TXV)" );
				return -1;
			}

			String outdir = args[0] + ".ext";

			if ( args.Count == 1 ) {
				FPS4.FPS4 fps4 = new FPS4.FPS4(args[0]);
				if (fps4.Files.Count == 2 || (fps4.Files.Count == 3 && fps4.Files[2].FileSize == 0)) {
					TXM txm = new TXM(fps4.GetChildByIndex(0).AsFile.DataStream);
					var txvs = fps4.GetChildByIndex(1).AsFile.DataStream;
					TXV txv = new TXV(txm, txvs.Duplicate(), AutodetectVesperiaPC(txvs.Duplicate()));
					return Extract(txv, outdir) ? 0 : -1;
				}
				Console.WriteLine(args[0] + " contains != 2 files, not a txm/txv pair");
				return -1;
			}

			return Extract( args[0], args[1], outdir ) ? 0 : -1;
		}

		public static bool Extract( string txmpath, string txvpath, string outdir ) {
			TXM txm = new TXM( txmpath );
			TXV txv;
			using ( Stream stream = new System.IO.FileStream( txvpath, FileMode.Open ) ) {
				txv = new TXV( txm, stream, AutodetectVesperiaPC( stream ) );
			}
			return Extract( txv, outdir );
		}
		
		public static bool Extract( TXV txv, string outdir ) {
			Directory.CreateDirectory( outdir );

			int counter = 0;
			foreach ( TXVSingle ts in txv.textures ) {
				foreach ( var tex in ts.GetDiskWritableStreams() ) {
					using ( var fs = new FileStream( Path.Combine( outdir, counter.ToString( "D4" ) + "_" + tex.name ), FileMode.Create ) ) {
						tex.data.Position = 0;
						StreamUtils.CopyStream( tex.data, fs, tex.data.Length );
					}
					++counter;
				}
			}

			return true;
		}

		public static bool AutodetectVesperiaPC( Stream stream ) {
			try {
				while ( true ) {
					uint sz = stream.ReadUInt32().FromEndian( EndianUtils.Endianness.BigEndian );
					if ( stream.Position + sz <= stream.Length ) {
						if ( stream.ReadAscii( 4 ) != "DDS " ) {
							return false;
						}
						stream.Position = stream.Position + ( sz - 4 );

						if ( stream.Position == stream.Length ) {
							return true;
						}
					} else {
						return false;
					}
				}
			} finally {
				stream.Position = 0;
			}
		}
	}
}
