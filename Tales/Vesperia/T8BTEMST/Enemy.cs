using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTEMST {
	public class Enemy {
		public uint[] Data;
		public float[] DataFloat;

		public uint NameStringDicID;
		public uint InGameID;
		public string RefString;
		public uint IconID;

		public uint Category;
		public uint Level;

		public uint HP;
		public uint TP;
		public uint PATK;
		public uint PDEF;
		public uint MATK;
		public uint MDEF;
		public uint AGL;

		public int AttrFire;
		public int AttrEarth;
		public int AttrWind;
		public int AttrWater;
		public int AttrLight;
		public int AttrDark;

		public uint EXP;
		public uint LP;
		public uint Gald;

		public uint InMonsterBook;
		public uint Location;
		public int LocationWeather;
		public uint[] DropItems;
		public uint[] DropChances;
		public uint StealItem;
		public uint StealChance;

		public Enemy( System.IO.Stream stream, uint refStringStart ) {
			uint entryLength = stream.ReadUInt32().SwapEndian();
			Data = new uint[entryLength / 4];
			DataFloat = new float[entryLength / 4];
			Data[0] = entryLength;

			for ( int i = 1; i < Data.Length; ++i ) {
				Data[i] = stream.ReadUInt32().SwapEndian();
				DataFloat[i] = Data[i].UIntToFloat();
			}

			NameStringDicID = Data[2];
			InGameID = Data[5];
			IconID = Data[57];

			Level = Data[7];
			Category = Data[24];

			HP = Data[8];
			TP = Data[9];
			PATK = Data[10];
			PDEF = Data[11];
			MATK = Data[12];
			MDEF = Data[13];
			AGL = Data[15];

			// > 100 weak, < 100 resist, 0 nullify, negative absorb
			// effectively a damage multiplier in percent
			AttrFire = (int)Data[26];
			AttrEarth = (int)Data[27];
			AttrWind = (int)Data[28];
			AttrWater = (int)Data[29];
			AttrLight = (int)Data[30];
			AttrDark = (int)Data[31];

			EXP = Data[33];
			Gald = Data[34];
			LP = Data[35];

			DropItems = new uint[4];
			DropItems[0] = Data[36];
			DropItems[1] = Data[37];
			DropItems[2] = Data[38];
			DropItems[3] = Data[39];
			DropChances = new uint[4];
			DropChances[0] = Data[40];
			DropChances[1] = Data[41];
			DropChances[2] = Data[42];
			DropChances[3] = Data[43];
			StealItem = Data[44];
			StealChance = Data[45];

			InMonsterBook = Data[59];
			Location = Data[60];
			// -1: None shown, 0: Sun, 1: Rain, 2: Snow, 3: Windy, 4: Night, 5: Cloudy
			LocationWeather = (int)Data[61];

			uint refStringLocation = Data[6];
			long pos = stream.Position;
			stream.Position = refStringStart + refStringLocation;
			RefString = stream.ReadAsciiNullterm();
			stream.Position = pos;
		}

		public override string ToString() {
			return RefString;
		}
	}
}
