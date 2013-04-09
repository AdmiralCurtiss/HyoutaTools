using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.N64.OoTSaveEditor {
	public class OoTSingleSave {
		private byte[] File;
		private int Offset;


		public ushort EntranceIndex; // 0x0002
		public uint Age; // 0x0004, 0 -> Adult, 1 -> Child
		public uint IdentifierZELD; // 0x001C
		public ushort IdentifierAZ; // 0x0020, combined with previous, must be "ZELDAZ" if file exists
		public ushort DeathCounter; // 0x0022
		public string PlayerName; // 0x0024, 8 bytes, padded with 0xDF
		public ushort DiskDriveSaveFlag; // 0x0030, unknown if location/size matches, 0 = no, 1 = yes
		public uint Health; // 0x0032, in 16th of a heart
		public uint Rupees; // 0x0036
		public ushort UnknownClockOnCurrentMap; // 0x003A

		public float FaroreWarpPosX; // 0x0E64
		public float FaroreWarpPosY; // 0x0E68
		public float FaroreWarpPosZ; // 0x0E6C
		public float FaroreWarpRotY; // 0x0E70, 2 bytes ????
		public ushort FaroreWarpEntranceIndex; // 0x0E7A ????
		public uint FaroreWarpMapNumber; // 0x0E7C ????
		public uint FaroreWarpIsSet; // 0x0E80 ????

		public ushort Checksum; // 0x1352

		public OoTSingleSave( byte[] File, int Offset ) {
			this.File = File;
			this.Offset = Offset;
			EntranceIndex = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x02 ) );
			Age = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x04 ) );
			IdentifierZELD = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x1C ) );
			IdentifierAZ = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x20 ) );
			DeathCounter = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x22 ) );
			PlayerName = BitConverter.ToString( File, Offset + 0x24, 8 );
			DiskDriveSaveFlag = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x30 ) );
			Health = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x32 ) );
			Rupees = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x36 ) );
			UnknownClockOnCurrentMap = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x3A ) );
			FaroreWarpPosX = BitConverter.ToSingle( File, Offset + 0x0E64 );
			FaroreWarpPosY = BitConverter.ToSingle( File, Offset + 0x0E68 );
			FaroreWarpPosZ = BitConverter.ToSingle( File, Offset + 0x0E6C );
			FaroreWarpRotY = BitConverter.ToSingle( File, Offset + 0x0E70 );
			FaroreWarpEntranceIndex = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x0E7A ) );
			FaroreWarpMapNumber = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x0E7C ) );
			FaroreWarpIsSet = Util.SwapEndian( BitConverter.ToUInt32( File, Offset + 0x0E80 ) );
			Checksum = Util.SwapEndian( BitConverter.ToUInt16( File, Offset + 0x1352 ) );
		}

		public void RecalculateChecksum() {
			ushort sum = 0;
			for ( int i = 0; i < 0x9A9; i++ ) {
				sum += Util.SwapEndian( BitConverter.ToUInt16( File, Offset + i * 2 ) );
			}
			Checksum = sum;
		}

		public void WriteToFile() {


			RecalculateChecksum();
		}
	}
}
