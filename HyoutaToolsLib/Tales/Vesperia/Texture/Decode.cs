using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaPluginBase;
using HyoutaUtils;
using HyoutaUtils.Streams;

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
				DuplicatableFileStream stream = new DuplicatableFileStream(args[0]);
				if (stream.PeekUInt32(EndianUtils.Endianness.BigEndian) == 0x46505334) {
					FPS4.FPS4 fps4 = new FPS4.FPS4(stream);
					if (fps4.Files.Count == 2 || (fps4.Files.Count == 3 && fps4.Files[2].FileSize == 0)) {
						TXM txm = new TXM(fps4.GetChildByIndex(0).AsFile.DataStream);
						var txvs = fps4.GetChildByIndex(1).AsFile.DataStream;
						TXV txv = new TXV(txm, txvs.Duplicate(), AutodetectVesperiaPC(txvs.Duplicate()));
						return Extract(txv, outdir) ? 0 : -1;
					}
					Console.WriteLine(args[0] + " contains != 2 files, not a txm/txv pair");
				} else {
					int? tovpc = AutodetectVesperiaPC(stream);
					if (tovpc.HasValue) {
						Console.WriteLine(args[0] + " looks like a PC-style Vesperia texture, unpacking as that.");
						return ExtractPCStyleTexture(stream, outdir, tovpc.Value, EndianUtils.Endianness.BigEndian) ? 0 : -1;
					} else {
						int? tospc = AutodetectSymphoniaPC(stream);
						if (tospc.HasValue) {
							Console.WriteLine(args[0] + " looks like a PC-style Symphonia texture, unpacking as that.");
							return ExtractPCStyleTexture(stream, outdir, tospc.Value, EndianUtils.Endianness.LittleEndian) ? 0 : -1;
						} else {
							Console.WriteLine(args[0] + " not detected as any known single-file format.");
						}
					}
				}

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

		public static bool ExtractPCStyleTexture(DuplicatableStream stream, string outdir, int texcount, EndianUtils.Endianness endian) {
			TXV txv = new TXV(null, stream, texcount, endian);
			return Extract(txv, outdir);
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

		public static int? AutodetectVesperiaPC( Stream stream ) {
			try {
				int count = 0;
				while ( true ) {
					uint sz = stream.ReadUInt32(EndianUtils.Endianness.BigEndian);
					if ( stream.Position + sz <= stream.Length ) {
						if ( stream.ReadAscii( 4 ) != "DDS " ) {
							return null;
						}
						stream.Position = stream.Position + ( sz - 4 );
						++count;

						if ( stream.Position == stream.Length ) {
							return count;
						}
					} else {
						return null;
					}
				}
			} finally {
				stream.Position = 0;
			}
		}

		public static int? AutodetectSymphoniaPC(Stream stream) {
			// like vesperia PC but with little endian offsets
			try {
				int count = 0;
				while ( true ) {
					uint sz = stream.ReadUInt32(EndianUtils.Endianness.LittleEndian);
					if ( stream.Position + sz <= stream.Length ) {
						if ( stream.ReadAscii( 4 ) != "DDS " ) {
							return null;
						}
						stream.Position = stream.Position + ( sz - 4 );
						++count;

						if ( stream.Position == stream.Length ) {
							return count;
						}
					} else {
						return null;
					}
				}
			} finally {
				stream.Position = 0;
			}
		}
	}
}
