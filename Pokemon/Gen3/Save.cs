using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;
using StreamUtils = HyoutaUtils.StreamUtils;

namespace HyoutaTools.Pokemon.Gen3 {
    public class Save {
        public enum Mode {
            Autodetect,
            Shrink,
            Expand,
        }

        public static bool VerifySave( out uint saveNumber, System.IO.Stream file, int position ) {
            saveNumber = 0;
            uint saveValidPage = 0;
            for ( int i = 0; i < 0xE; ++i ) {
                int sectorpos = position + 0x1000 * i;
                file.Position = sectorpos + 0xFF4;
                int saveLogicalPage = file.ReadByte();
                file.DiscardBytes( 1 );
                ushort readChecksum = file.ReadUInt16();
                uint magic = file.ReadUInt32();
                uint saveNumberTmp = file.ReadUInt32();

                // verify magic number
                if ( magic != 0x08012025 ) {
                    Console.WriteLine( "Magic number of sector at 0x" + sectorpos.ToString( "X" ) + " is wrong." );
                    continue;
                }

                // check if logical page is in valid range
                if ( saveLogicalPage > 0xD ) {
                    Console.WriteLine( "Logical page of sector at 0x" + sectorpos.ToString( "X" ) + " is outside of valid range." );
                    continue;
                }

                // verify checksum
                file.Position = sectorpos;
                ushort calculatedChecksum = Checksum.CalculateSaveChecksum( file, 0xF80 );
                if ( readChecksum != calculatedChecksum ) {
                    Console.WriteLine( "Checksum of sector at 0x" + sectorpos.ToString( "X" ) + " is wrong." );
                    continue;
                }

                // all checks passed, page is valid, remember that
                saveNumber = saveNumberTmp;
                saveValidPage |= ( 1u << saveLogicalPage );
            }

            return saveValidPage == 0x3FFF;
        }

        public static int Execute( List<string> args ) {
            if ( args.Count < 1 ) {
                Console.WriteLine( "Usage: [--shrink/--expand] pokemon-emerald.sav [pokemon-emerald-converted.sav]" );
                return -1;
            }

            try {
                Mode mode = Mode.Autodetect;
                int argcnt = 0;
                if ( args[argcnt].StartsWith( "--" ) ) {
                    switch ( args[argcnt] ) {
                        case "--shrink":
                            mode = Mode.Shrink;
                            break;
                        case "--expand":
                            mode = Mode.Expand;
                            break;
                        default:
                            Console.WriteLine( "Unrecognized option '" + args[argcnt] + "'." );
                            break;
                    }
                    ++argcnt;
                }

                String filename = args[argcnt++];
                using ( System.IO.Stream file = new System.IO.FileStream( filename, System.IO.FileMode.Open ) ) {
                    if ( mode == Mode.Autodetect ) {
                        switch ( file.Length ) {
                            case 0x10000:
                                mode = Mode.Expand;
                                break;
                            case 0x20000:
                                mode = Mode.Shrink;
                                break;
                            default:
                                Console.WriteLine( "Filesize is not valid for shrinking or expanding, mode autodection failed." );
                                return -1;
                        }
                    }
                    String outname;
                    if ( args.Count > argcnt ) {
                        outname = args[argcnt++];
                    } else {
                        String fnabs = System.IO.Path.GetFullPath( filename );
                        outname = System.IO.Path.Combine( System.IO.Path.GetDirectoryName( fnabs ), System.IO.Path.GetFileNameWithoutExtension( fnabs ) + "_" + ( mode == Mode.Expand ? "1Mb" : "512Kb" ) + System.IO.Path.GetExtension( fnabs ) );
                    }
                    using ( System.IO.Stream outfile = new System.IO.FileStream( outname, System.IO.FileMode.Create ) ) {
                        if ( mode == Mode.Shrink ) {
                            uint[] saveNumbers = new uint[2];
                            bool[] saveValid = new bool[2];

                            Console.WriteLine( "Reading save in first slot..." );
                            saveValid[0] = VerifySave( out saveNumbers[0], file, 0x0 );
                            if ( saveValid[0] ) {
                                Console.WriteLine( "Save in first slot is valid, is save #" + saveNumbers[0] + "." );
                            } else {
                                Console.WriteLine( "Save in first slot is invalid." );
                            }

                            Console.WriteLine( "Reading save in second slot..." );
                            saveValid[1] = VerifySave( out saveNumbers[1], file, 0xE000 );
                            if ( saveValid[1] ) {
                                Console.WriteLine( "Save in second slot is valid, is save #" + saveNumbers[1] + "." );
                            } else {
                                Console.WriteLine( "Save in second slot is invalid." );
                            }

                            int saveToRead = -1;
                            if ( saveValid[0] || saveValid[1] ) {
                                // at least one save is valid
                                if ( saveValid[0] && saveValid[1] ) {
                                    // both are valid, take the one with the higher number
                                    // TODO: Overflow protection, but let's be honest, who's gonna save 4 billion times...
                                    if ( saveNumbers[0] >= saveNumbers[1] ) {
                                        Console.WriteLine( "Save in first slot is newer." );
                                        saveToRead = 0;
                                    } else {
                                        Console.WriteLine( "Save in second slot is newer." );
                                        saveToRead = 1;
                                    }
                                } else if ( saveValid[0] ) {
                                    // only first is valid
                                    Console.WriteLine( "Save in first slot is the only valid one." );
                                    saveToRead = 0;
                                } else {
                                    // only second is valid
                                    Console.WriteLine( "Save in second slot is the only valid one." );
                                    saveToRead = 1;
                                }
                            }

                            if ( saveToRead == -1 ) {
                                // no save is valid
                                Console.WriteLine( "Both saves are invalid." );
                                return -1;
                            }

                            // copy save to outfile
                            Console.WriteLine( "Writing selected save to outfile..." );
                            file.Position = 0xE000 * saveToRead;
                            StreamUtils.CopyStream( file, outfile, 0xE000 );

                            // read hall of fame
                            Console.WriteLine( "Reading Hall of Fame..." );
                            HallOfFame.HallOfFameStructure hof = HallOfFame.ReadHallOfFameFromSave( file );

                            if ( hof != null ) {
                                // truncate hall of fame to 30 entries
                                int validHallOfFameEntires = hof.Entries.Count( x => x.IsValid() );
                                Console.WriteLine( "Found " + validHallOfFameEntires + " Hall of Fame entries." );
                                int toSkip;
                                if ( validHallOfFameEntires > 32 ) {
                                    toSkip = validHallOfFameEntires - 32;
                                    Console.WriteLine( "Removing the " + toSkip + " oldest entries to fit Hall of Fame into smaller save file." );
                                } else {
                                    toSkip = 0;
                                }
                                List<HallOfFame.HallOfFameEntry> entriesToKeep = new List<HallOfFame.HallOfFameEntry>();
                                for ( int i = 0; i < hof.Entries.Length; ++i ) {
                                    if ( hof.Entries[i].IsValid() ) {
                                        if ( toSkip > 0 ) {
                                            --toSkip;
                                        } else {
                                            entriesToKeep.Add( hof.Entries[i] );
                                        }
                                    }
                                }
                                while ( entriesToKeep.Count < 32 ) {
                                    entriesToKeep.Add( new HallOfFame.HallOfFameEntry() );
                                }
                                hof.Entries = entriesToKeep.ToArray();

                                // write truncated hall of fame to outfile
                                Console.WriteLine( "Writing Hall of Fame to outfile..." );
                                HallOfFame.WriteHallOfFameDataToSave( hof, outfile, 0xE000, 1 );
                            } else {
                                Console.WriteLine( "Couldn't read Hall of Fame, writing empty sector..." );
                                StreamUtils.WriteAlign( outfile, 0xF000, 0xFF );
                            }

                            // and just copy the battle recording sector as-is
                            Console.WriteLine( "Copying battle recording sector directly..." );
                            file.Position = 0x1F000;
                            outfile.Position = 0xF000;
                            StreamUtils.CopyStream( file, outfile, 0x1000 );

                            Console.WriteLine( "Done!" );
                        } else if ( mode == Mode.Expand ) {
                            // copy save twice
                            Console.WriteLine( "Copying save into both slots..." );
                            file.Position = 0x0;
                            StreamUtils.CopyStream( file, outfile, 0xE000 );
                            file.Position = 0x0;
                            StreamUtils.CopyStream( file, outfile, 0xE000 );

                            // expand hall of fame to two sectors
                            Console.WriteLine( "Copying and expanding Hall of Fame..." );
                            HallOfFame.HallOfFameStructure hof = HallOfFame.ReadHallOfFameFromSave( file, 0xE000, 1 );
                            if ( hof != null ) {
                                HallOfFame.WriteHallOfFameDataToSave( hof, outfile, 0x1C000, 2 );
                            } else {
                                Console.WriteLine( "Couldn't read Hall of Fame, writing empty sectors..." );
                                StreamUtils.WriteAlign( outfile, 0x1E000, 0xFF );
                            }

                            // write empty trainer hill sector
                            Console.WriteLine( "Writing empty Trainer Hill sector..." );
                            StreamUtils.WriteAlign( outfile, 0x1F000, 0xFF );

                            // copy battle recording
                            Console.WriteLine( "Copying battle recording sector directly..." );
                            file.Position = 0xF000;
                            StreamUtils.CopyStream( file, outfile, 0x1000 );

                            Console.WriteLine( "Done!" );
                        } else {
                            Console.WriteLine( "Shouldn't get here." );
                            return -1;
                        }
                    }
                }
            } catch ( Exception ex ) {
                Console.WriteLine( "Error: " + ex.ToString() );
                return -1;
            }

            return 0;
        }
    }
}
