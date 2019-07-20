using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaUtils;

namespace HyoutaTools.Pokemon.Gen3 {
    public static class Checksum {
        public static ushort CalculateSaveChecksum( System.IO.Stream stream, int length ) {
            uint checksum = 0;
            for ( int i = 0; i < length / 4; ++i ) {
                checksum += stream.ReadUInt32();
            }
            return (ushort)( ( ( checksum & 0xFFFF ) + ( ( checksum & 0xFFFF0000 ) >> 16 ) ) & 0xFFFF );
        }
    }
}
