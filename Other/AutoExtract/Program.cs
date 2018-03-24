using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace HyoutaTools.Other.AutoExtract {
	class FileStruct {
		public String Filename;
		public int Indirection;

		public FileStruct( String Filename, int Indirection ) {
			this.Filename = Filename;
			this.Indirection = Indirection;
		}
	}

	class Program {
		static bool RunProgram( String prog, String args ) {
			// Use ProcessStartInfo class
			ProcessStartInfo startInfo = new ProcessStartInfo();
			startInfo.CreateNoWindow = false;
			startInfo.UseShellExecute = false;
			startInfo.FileName = prog;
			startInfo.WindowStyle = ProcessWindowStyle.Hidden;
			startInfo.Arguments = args;
			startInfo.RedirectStandardOutput = true;
			startInfo.RedirectStandardError = true;

			try {
				// Start the process with the info we specified.
				// Call WaitForExit and then the using statement will close.
				using ( Process exeProcess = Process.Start( startInfo ) ) {
					exeProcess.WaitForExit();
					string output = exeProcess.StandardOutput.ReadToEnd();

					if ( exeProcess.ExitCode != 0 ) {
						Console.WriteLine( prog + " returned nonzero:" );
						Console.WriteLine( output );
						return false;
					}

					bool success = false;
					switch ( prog ) {
						case "comptoe":
							success = output.EndsWith( "Success\r\n" );
							break;
						default:
							return true;
							break;
					}

					if ( !success ) {
						Console.WriteLine( prog + " reported failure:" );
						Console.WriteLine( output );
					}

					return success;
				}
			} catch ( Exception ) {
				return false;
			}
		}


		public static void EnqueueDirectoryRecursively( Queue<FileStruct> queue, string Directory ) {
			foreach ( string f in System.IO.Directory.GetFiles( Directory ) ) {
				queue.Enqueue( new FileStruct( f, 0 ) );
			}
			foreach ( string d in System.IO.Directory.GetDirectories( Directory ) ) {
				EnqueueDirectoryRecursively( queue, d );
			}
		}


		public static int Execute( List<string> argumentsGiven ) {
			if ( !argumentsGiven.Contains( "start" ) ) {
				Console.WriteLine( "Include \"start\" in the arguments to confirm execution!" );
				Console.WriteLine( "Other arguments to enable specific file types:" );
				Console.WriteLine( "  comptoe     Tales Compressor files" );
				Console.WriteLine( "  DRpak       Dangan Ronpa PAK files" );
				Console.WriteLine( "  LRNPK       Last Ranker NPK files (aggressive)" );
				Console.WriteLine( "  LRSCMP      Last Ranker SCMP files" );
				Console.WriteLine( "  LRPTMDdeep  Last Ranker PTMD textues, deep scan" );
				Console.WriteLine( "Usage of these parameters could break unrelated files, be careful." );
				return -1;
			}


			Queue<FileStruct> queue = new Queue<FileStruct>();

			Console.WriteLine( "Adding all files and folders recursively..." );
			EnqueueDirectoryRecursively( queue, System.Environment.CurrentDirectory );

			bool AllowComptoe = argumentsGiven.Contains( "comptoe" );
			bool AllowDrPak = argumentsGiven.Contains( "DRpak" );
			bool AggressiveLastRankerNPK = argumentsGiven.Contains( "LRNPK" );
			bool AllowLastRankerSCMP = argumentsGiven.Contains( "LRSCMP" );
			bool DeepSearchForPTMD = argumentsGiven.Contains( "LRPTMDdeep" );

			while ( queue.Count > 0 ) {
				FileStruct fstr = queue.Dequeue();
				Console.WriteLine( fstr.Filename );
				string f = System.IO.Path.GetFullPath( fstr.Filename );
				string prog, args;

				if ( !System.IO.File.Exists( f ) ) continue;

				try {
					bool isMaybeVesperiaStyleTexture;
					isMaybeVesperiaStyleTexture = f.EndsWith( ".TXV" );
					if ( isMaybeVesperiaStyleTexture ) {
						string txm = f.Substring( 0, f.Length - 1 ) + "M";
						string txv = f;
						Tales.Vesperia.Texture.Decode.Extract(txm, txv, f + ".ext" );
					}

					using ( FileStream fs = new FileStream( f, FileMode.Open ) ) {
						long filesize = fs.Length;
						int firstbyte = fs.ReadByte();
						int secondbyte = fs.ReadByte();
						int thirdbyte = fs.ReadByte();
						int fourthbyte = fs.ReadByte();
						int fifthbyte = fs.ReadByte();

						bool HasBeenProcessed = false;

						// maybe a comptoe file
						if ( AllowComptoe ) {
							if ( firstbyte == 0x01 || firstbyte == 0x03 ) {

								uint maybefilesizeBigEndian = ( (uint)secondbyte ) << 24 | ( (uint)thirdbyte ) << 16 | ( (uint)fourthbyte ) << 8 | (uint)fifthbyte;
								uint maybefilesizeLitEndian = ( (uint)fifthbyte ) << 24 | ( (uint)fourthbyte ) << 16 | ( (uint)thirdbyte ) << 8 | (uint)secondbyte;
								if ( ( maybefilesizeBigEndian == filesize ) ||
									 ( maybefilesizeLitEndian + 9 == filesize ) ) {
									int b6 = fs.ReadByte();
									int b7 = fs.ReadByte();
									int b8 = fs.ReadByte();
									int b9 = fs.ReadByte();
									uint uncompressedfilesizeBigEndian = ( (uint)b6 ) << 24 | ( (uint)b7 ) << 16 | ( (uint)b8 ) << 8 | (uint)b9;
									uint uncompressedfilesizeLitEndian = ( (uint)b6 ) | ( (uint)b7 ) << 8 | ( (uint)b8 ) << 16 | ( (uint)b9 ) << 24;
									fs.Close();
									prog = "comptoe";
									args = "-d \"" + f + "\" \"" + f + ".d\"";
									Console.WriteLine();
									Console.WriteLine( prog + " " + args );
									if ( RunProgram( prog, args ) ) {
										queue.Enqueue( new FileStruct( f + ".d", fstr.Indirection ) );
										FileInfo decInfo = new FileInfo( f + ".d" );
										if ( ( decInfo.Length == uncompressedfilesizeBigEndian ) || ( decInfo.Length == uncompressedfilesizeLitEndian ) ) {
											System.IO.File.Delete( f );
											HasBeenProcessed = true;
										} else {
											Console.WriteLine( "Uncompressed comptoe Filesize does not match!" );
										}
									}
								} else {
									Console.WriteLine();
									Console.WriteLine( f );
									Console.WriteLine( "Suspected comptoe, but compressed Filesize does not match!" );

									Console.WriteLine();
									Console.WriteLine( "Tales.Abyss.PKF.Split.SplitPkf " + f );
									fs.Close();
									if ( Tales.Abyss.PKF.Split.SplitPkf( System.IO.File.ReadAllBytes( f ), f + ".ex" ) ) {
										EnqueueDirectoryRecursively( queue, f + ".ex" );
										System.IO.File.Delete( f );
										HasBeenProcessed = true;
									}

								}

							}
						}

						if ( firstbyte == (int)'T' ) {
							if ( secondbyte == (int)'L' && thirdbyte == (int)'Z' && fourthbyte == (int)'C' ) {
								fs.Close();
								List<string> tlzcargs = new List<string>();
								tlzcargs.Add( "-d" );
								tlzcargs.Add( f );
								tlzcargs.Add( f + ".dec" );
								Console.WriteLine();
								Console.WriteLine( "tlzc -d \"" + f + "\" \"" + f + ".dec\"" );
								if ( HyoutaTools.Tales.tlzc.tlzcmain.Execute( tlzcargs ) == 0 ) {
									queue.Enqueue( new FileStruct( f + ".dec", fstr.Indirection ) );
									System.IO.File.Delete( f );
									HasBeenProcessed = true;
								}
							}
						}

						if ( firstbyte == (int)'F' ) {
							if ( secondbyte == (int)'P' && thirdbyte == (int)'S' && fourthbyte == (int)'4' ) {
								fs.Close();
								prog = "tovfps4e";
								args = "\"" + f + "\"";
								Console.WriteLine();
								Console.WriteLine( prog + " " + args );

								var fps4 = new Tales.Vesperia.FPS4.FPS4( f );
								fps4.Extract( f + ".ext" );
								fps4.Close();

								EnqueueDirectoryRecursively( queue, f + ".ext" );
								System.IO.File.Delete( f );
								HasBeenProcessed = true;
							}

							if ( secondbyte == (int)'P' && thirdbyte == (int)'S' && fourthbyte == (int)'2' ) {
								fs.Close();
								Console.WriteLine();
								Console.WriteLine( "Tales.Abyss.FPS2.Extract " + f );
								List<string> argList = new List<string>( 1 );
								argList.Add( f );
								if ( 0 == Tales.Abyss.FPS2.Program.Execute( argList ) ) {
									EnqueueDirectoryRecursively( queue, f + ".ext" );
									System.IO.File.Delete( f );
									HasBeenProcessed = true;
								}
							}
							if ( secondbyte == (int)'P' && thirdbyte == (int)'S' && fourthbyte == (int)'3' ) {
								fs.Close();
								Console.WriteLine();
								Console.WriteLine( "Tales.Abyss.FPS3.Extract " + f );
								List<string> argList = new List<string>( 1 );
								argList.Add( f );
								if ( 0 == Tales.Abyss.FPS3.Program.Execute( argList ) ) {
									EnqueueDirectoryRecursively( queue, f + ".ext" );
									System.IO.File.Delete( f );
									HasBeenProcessed = true;
								}
							}
						}

						string fname = System.IO.Path.GetFileName( f );
						if (
							( firstbyte == 0x00 && secondbyte == 0x02 && thirdbyte == 0x00 && fourthbyte == 0x00 &&
							!isMaybeVesperiaStyleTexture
							&& !( fname.EndsWith( ".TXM" ) || fname.EndsWith( ".TXV" ) ) )
							 ) {

							FileStruct nextfile = queue.Peek();
							fs.Close();

							string txm = f + ".TXM";
							string txv = f + ".TXV";

							Console.WriteLine( "ren " + f + " " + txm );
							Console.WriteLine( "ren " + nextfile.Filename + " " + txv );
							System.IO.File.Move( f, txm );
							System.IO.File.Move( nextfile.Filename, txv );

							queue.Enqueue( new FileStruct( txv, fstr.Indirection ) );

							HasBeenProcessed = true;
						}


						if ( firstbyte == 0x4D ) {
							if ( secondbyte == 0x49 && thirdbyte == 0x47 && fourthbyte == 0x2E ) {
								fs.Close();
								Console.WriteLine();
								Console.WriteLine( "GimToPng " + f );
								List<string> converted = PSP.GIM.GimToPng.GimToPng.ConvertGimFileToPngFiles( f );

								if ( converted != null && converted.Count > 0 ) {
									System.IO.File.Delete( f );
									HasBeenProcessed = true;
								}
							}
						}
						if ( firstbyte == 'O' && secondbyte == 'M' && thirdbyte == 'G' && fourthbyte == 0x2E ) {
							fs.Close();
							f = RenameToWithExtension( f, ".gmo" );
							HasBeenProcessed = true;
						}

						if ( firstbyte == 'L' && secondbyte == 'L' && thirdbyte == 'F' && fourthbyte == 'S' ) {
							fs.Close();
							f = RenameToWithExtension( f, ".llfs" );
							HasBeenProcessed = true;
						}

						if ( AllowDrPak ) {
							if ( f.ToLowerInvariant().EndsWith( ".pak" ) || ( secondbyte < 0x10 && thirdbyte == 0x00 && fourthbyte == 0x00 ) ) {
								// could maybe possibly be a PAK file who knows
								bool drpaksuccess = false;
								try {
									fs.Position = 0;
									DanganRonpa.Pak.Program.Extract( fs, f + ".ex" );
									fs.Close();
									drpaksuccess = true;
								} catch ( Exception ex ) {
									Console.WriteLine( "Extracting " + f + " as DanganRonpa PAK file failed: " + ex.ToString() );
								}

								if ( drpaksuccess ) {
									EnqueueDirectoryRecursively( queue, f + ".ex" );
									System.IO.File.Delete( f );
									HasBeenProcessed = true;
								}
							}
						}

						if ( firstbyte == 0x1F && secondbyte == 0x8B && thirdbyte == 0x08 ) {
							// gzip compressed file
							GZip.GZipHandler.Extract( fs, f + ".dec" );
							queue.Enqueue( new FileStruct( f + ".dec", fstr.Indirection ) );
							fs.Close();
							System.IO.File.Delete( f );
							HasBeenProcessed = true;
						}

						if ( AllowLastRankerSCMP ) {
							if ( firstbyte == 'S' && secondbyte == 'C' && thirdbyte == 'M' && fourthbyte == 'P' ) {
								fs.Close();
								List<string> strl = new List<string>();
								strl.Add( f );
								Console.WriteLine( strl[0] );
								LastRanker.SCMP.ExecuteExtract( strl );
								HasBeenProcessed = true;
							}
						}
						if ( firstbyte == 'C' && secondbyte == 'Z' && thirdbyte == 'A' && fourthbyte == 'A' ) {
							fs.Close();
							System.IO.File.WriteAllBytes( f + ".dec", new LastRanker.CZAA( f ).ExtractedFile );
							queue.Enqueue( new FileStruct( f + ".dec", fstr.Indirection ) );
							System.IO.File.Delete( f );
							HasBeenProcessed = true;
						}
						if ( f.ToUpperInvariant().EndsWith( ".NPK" )
						 || (
								AggressiveLastRankerNPK && (
									( ( firstbyte | ( secondbyte << 8 ) ) * 3 + 3 + 2 ).Align( 0x10 )
									== ( thirdbyte | ( fourthbyte << 8 ) | ( fifthbyte << 16 ) )
								)
							) ) {
							fs.Close();
							List<string> strl = new List<string>();
							strl.Add( f );
							Console.WriteLine( strl[0] );
							LastRanker.NPK.ExecuteExtract( strl );
							EnqueueDirectoryRecursively( queue, f + ".ex" );
							System.IO.File.Delete( f );
							HasBeenProcessed = true;
						}
						if ( firstbyte == 'R' && secondbyte == 'T' && thirdbyte == 'D' && fourthbyte == 'P' ) {
							fs.Close();
							List<string> strl = new List<string>();
							strl.Add( f );
							Console.WriteLine( strl[0] );
							LastRanker.RTDP.ExecuteExtract( strl );
							EnqueueDirectoryRecursively( queue, f + ".ex" );
							System.IO.File.Delete( f );
							HasBeenProcessed = true;
						}

						if ( firstbyte == 'P' && secondbyte == 'T' && thirdbyte == 'M' && fourthbyte == 'D' ) {
							fs.Close();
							new LastRanker.PTMD( f ).SaveAsPNG( f + ".png" );
							//System.IO.File.Delete( f );
							HasBeenProcessed = true;
						}

						if ( !HasBeenProcessed && DeepSearchForPTMD ) {
							fs.Close();

							byte[] file = System.IO.File.ReadAllBytes( f );

							for ( int i = 0; i < file.Length - 3; ++i ) {
								if ( file[i] == 'P' && file[i + 1] == 'T' && file[i + 2] == 'M' && file[i + 3] == 'D' ) {
									byte[] temp = new byte[file.Length - i];
									Util.CopyByteArrayPart( file, i, temp, 0, temp.Length );
									try {
										new LastRanker.PTMD( temp ).SaveAsPNG( f + "." + i.ToString( "X8" ) + ".png" );
									} catch ( Exception ex ) { Console.WriteLine( ex.ToString() ); }
								}
							}


							HasBeenProcessed = true;
						}
					}
				} catch ( FileNotFoundException ) { } catch ( Exception ex ) {
					Console.WriteLine( ex.ToString() );
				}
			}
			return 0;
		}

		public static String RenameToWithExtension( String filename, String extension ) {
			if ( !filename.EndsWith( extension ) ) {
				string extensionname = filename + extension;
				Console.WriteLine( "ren " + filename + " " + extensionname );
				System.IO.File.Move( filename, extensionname );
				return extensionname;
			}
			return filename;
		}

	}
}
