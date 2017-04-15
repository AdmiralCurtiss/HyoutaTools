using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Pokemon.Gen3 {
    public class HallOfFame {
        public class HallOfFamePokemon {
            public ushort TrainerId;
            public ushort SecretId;
            public uint PersonalityValue;
            public ushort Species;
            public byte Level;
            public String3 Nickname;
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
            public HallOfFameEntry( System.IO.Stream stream ) {
                Pokemon = new HallOfFamePokemon[6];
                for ( int i = 0; i < 6; ++i ) {
                    Pokemon[i] = new HallOfFamePokemon( stream );
                }
            }
            public void Serialize( System.IO.Stream stream ) {
                for ( int i = 0; i < Pokemon.Length; ++i ) {
                    Pokemon[i].Serialize( stream );
                }
            }
        }

        public class HallOfFameStructure {
            public HallOfFameEntry[] Entries;
            public HallOfFameStructure( System.IO.Stream stream ) {
                Entries = new HallOfFameEntry[50];
                for ( int i = 0; i < 50; ++i ) {
                    Entries[i] = new HallOfFameEntry( stream );
                }
            }
            public void Serialize( System.IO.Stream stream ) {
                for ( int i = 0; i < Entries.Length; ++i ) {
                    Entries[i].Serialize( stream );
                }
            }
        }

        public static int Execute( List<string> args ) {
            if ( args.Count < 1 ) {
                Console.WriteLine( "Usage: pokemon-emerald.sav (maybe other Gen3 works too?)" );
                return -1;
            }

            String filename = args[0];
            using ( System.IO.Stream file = new System.IO.FileStream( filename, System.IO.FileMode.Open ) )
            using ( System.IO.Stream buffer = new System.IO.MemoryStream( 0xF80 * 2 ) ) {
                // Read
                file.Position = 0x1C000;
                Util.CopyStream( file, buffer, 0xF80 );
                file.Position = 0x1D000;
                Util.CopyStream( file, buffer, 0xF80 );
                buffer.Position = 0;

                // Deserialize
                HallOfFameStructure hofs = new HallOfFameStructure( buffer );

                // Do something, whatever
                {
                    for ( int i = 2; i < 50; i += 2 ) {
                        hofs.Entries[i + 0] = hofs.Entries[0];
                        hofs.Entries[i + 1] = hofs.Entries[1];
                    }
                }

                // Serialize
                buffer.Position = 0;
                hofs.Serialize( buffer );

                // Calculate checksums
                buffer.Position = 0;
                ushort checksum1C = Checksum.CalculateSaveChecksum( buffer, 0xF80 );
                ushort checksum1D = Checksum.CalculateSaveChecksum( buffer, 0xF80 );

                // Write back into save
                file.Position = 0x1C000;
                buffer.Position = 0;
                Util.CopyStream( buffer, file, 0xF80 );
                file.Position = 0x1CFF4;
                file.WriteUInt16( checksum1C );

                file.Position = 0x1D000;
                Util.CopyStream( buffer, file, 0xF80 );
                file.Position = 0x1DFF4;
                file.WriteUInt16( checksum1D );
            }

            return 0;
        }
    }
}
