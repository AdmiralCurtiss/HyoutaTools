using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Pokemon.Gen3 {
    public class HallOfFame {
        public class HallOfFamePokemon {
            public ushort TrainerId;
            public ushort SecretId;
            public uint PersonalityValue;
            public ushort Species;
            public byte Level;
            public String3 Nickname;

            public HallOfFamePokemon() {
                TrainerId = 0;
                SecretId = 0;
                PersonalityValue = 0;
                Species = 0;
                Level = 0;
                Nickname = new String3( String3.Region.Western, 10 );
            }

            public HallOfFamePokemon( System.IO.Stream stream ) {
                TrainerId = stream.ReadUInt16();
                SecretId = stream.ReadUInt16();
                PersonalityValue = stream.ReadUInt32();
                int speciesLow = stream.ReadByte();
                int speciesHighLv = stream.ReadByte();
                Species = (ushort)( speciesLow | ( ( speciesHighLv & 0x1 ) << 8 ) );
                Level = (byte)( ( speciesHighLv & 0xFE ) >> 1 );
                // Nickname is always English, Japanese names are prepended with 0xFC15 which switches the text renderer to JP mode and appended with 0xFC16 to switch back.
                // This works because Japanese names are limited to 5 characters.
                Nickname = new String3( String3.Region.Western, stream, 10 );
            }

            public bool IsValid() {
                return Species != 0;
            }

            public void Serialize( System.IO.Stream stream ) {
                stream.WriteUInt16( TrainerId );
                stream.WriteUInt16( SecretId );
                stream.WriteUInt32( PersonalityValue );
                byte speciesLow = (byte)( Species & 0xFF );
                byte speciesHighLv = (byte)( ( ( Species & 0x100 ) >> 8 ) | ( ( Level << 1 ) & 0xFE ) );
                stream.WriteByte( speciesLow );
                stream.WriteByte( speciesHighLv );
                Nickname.Serialize( stream );
            }

            public override string ToString() {
                return "#" + Species + " \"" + Nickname.ToString() + "\", Lv " + Level + ", ID " + TrainerId + ", Secret ID " + SecretId;
            }
        }
        public class HallOfFameEntry {
            public HallOfFamePokemon[] Pokemon;

            public HallOfFameEntry() {
                Pokemon = new HallOfFamePokemon[6];
                for ( int i = 0; i < 6; ++i ) {
                    Pokemon[i] = new HallOfFamePokemon();
                }
            }

            public HallOfFameEntry( System.IO.Stream stream ) {
                Pokemon = new HallOfFamePokemon[6];
                for ( int i = 0; i < 6; ++i ) {
                    Pokemon[i] = new HallOfFamePokemon( stream );
                }
            }

            public bool IsValid() {
                return Pokemon[0].IsValid();
            }

            public void Serialize( System.IO.Stream stream ) {
                for ( int i = 0; i < Pokemon.Length; ++i ) {
                    Pokemon[i].Serialize( stream );
                }
            }
        }

        public class HallOfFameStructure {
            public HallOfFameEntry[] Entries;
            public HallOfFameStructure( System.IO.Stream stream, int maxEntries ) {
                Entries = new HallOfFameEntry[maxEntries];
                for ( int i = 0; i < maxEntries; ++i ) {
                    Entries[i] = new HallOfFameEntry( stream );
                }
            }
            public void Serialize( System.IO.Stream stream ) {
                for ( int i = 0; i < Entries.Length; ++i ) {
                    Entries[i].Serialize( stream );
                }
            }
        }

        public static HallOfFameStructure ReadHallOfFameFromSave( System.IO.Stream file, int offset = 0x1C000, int pages = 2 ) {
            using ( System.IO.Stream buffer = new System.IO.MemoryStream( 0xF80 * pages ) ) {
                // Read
                for ( int i = 0; i < pages; ++i ) {
                    int secpos = offset + i * 0x1000;
                    file.Position = secpos + 0xFF8;
                    uint magic = file.ReadUInt32();
                    if ( magic != 0x08012025 ) {
                        Console.WriteLine( "Magic number of sector at 0x" + secpos.ToString( "X" ) + " is wrong, Hall of Fame is probably empty." );
                        return null;
                    }

                    file.Position = secpos + 0xFF4;
                    ushort checksumRead = file.ReadUInt16();

                    file.Position = secpos;
                    ushort checksum = Checksum.CalculateSaveChecksum( file, 0xF80 );
                    if ( checksumRead != checksum ) {
                        Console.WriteLine( "Checksum of sector at 0x" + secpos.ToString( "X" ) + " is wrong." );
                        return null;
                    }

                    file.Position = secpos;
                    StreamUtils.CopyStream( file, buffer, 0xF80 );
                }

                // Deserialize
                buffer.Position = 0;
                HallOfFameStructure hofs = new HallOfFameStructure( buffer, ( pages * 0xF80 ) / 0x78 );
                return hofs;
            }
        }

        public static void WriteHallOfFameDataToSave( HallOfFameStructure hofs, System.IO.Stream file, int offset = 0x1C000, int pages = 2 ) {
            using ( System.IO.Stream buffer = new System.IO.MemoryStream( 0xF80 * pages ) ) {
                // Serialize
                buffer.Position = 0;
                hofs.Serialize( buffer );
                buffer.WriteAlign( 0xF80 * pages, 0x00 );

                // Calculate checksums
                buffer.Position = 0;
                ushort[] checksums = new ushort[pages];
                for ( int i = 0; i < pages; ++i ) {
                    checksums[i] = Checksum.CalculateSaveChecksum( buffer, 0xF80 );
                }

                // Write back into save
                buffer.Position = 0;
                for ( int i = 0; i < pages; ++i ) {
                    int secpos = offset + i * 0x1000;
                    file.WriteAlign( secpos + 0x1000, 0x00 );

                    file.Position = secpos;
                    StreamUtils.CopyStream( buffer, file, 0xF80 );
                    file.Position = secpos + 0xFF4;
                    file.WriteUInt16( checksums[i] );
                    file.Position = secpos + 0xFF8;
                    file.WriteUInt32( 0x08012025 );

                    file.Position = secpos + 0x1000;
                }
            }
        }

        public static int Execute( List<string> args ) {
            if ( args.Count < 1 ) {
                Console.WriteLine( "Usage: pokemon-emerald.sav (maybe other Gen3 works too?)" );
                return -1;
            }

            String filename = args[0];
            using ( System.IO.Stream file = new System.IO.FileStream( filename, System.IO.FileMode.Open ) ) {
                // Read
                HallOfFameStructure hofs = ReadHallOfFameFromSave( file );

                // Do something, whatever
                {
                    for ( int i = 2; i < 50; i += 2 ) {
                        hofs.Entries[i + 0] = hofs.Entries[0];
                        hofs.Entries[i + 1] = hofs.Entries[1];
                    }
                }

                // Write
                WriteHallOfFameDataToSave( hofs, file );
            }

            return 0;
        }
    }
}
