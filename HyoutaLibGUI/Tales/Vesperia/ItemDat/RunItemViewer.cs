using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HyoutaPluginBase;
using HyoutaTools;
using HyoutaTools.Tales.Vesperia;
using HyoutaTools.Tales.Vesperia.FPS4;
using HyoutaTools.Tales.Vesperia.TSS;
using HyoutaUtils;
using HyoutaUtils.Streams;

namespace HyoutaLibGUI.Tales.Vesperia.ItemDat {
	class RunItemViewer {
		public static int Execute(List<string> args) {
			bool gamedirmode = args.Count == 3 || args.Count == 4;
			bool manualmode = args.Count == 9;

			if (!gamedirmode && !manualmode) {
				System.Windows.Forms.MessageBox.Show(
					"Requires arguments:\n\n" +
					"  For automatically fetching files from game directory (point at folder containing item.svo):\n" +
					"    Version Locale GameDirectory [STRING_DIC.SO]\n" +
					"  STRING_DIC.SO can be overridden to select a different language file.\n" +
					"\n" +
					"  For manually providing files:\n" +
					"    Version Locale ITEM.DAT STRING_DIC.SO T8BTSK T8BTEMST COOKDAT WRLDDAT ITEMSORT.DAT\n" +
					"\n" +
					"'Version' should be one of: 360, 360EU, PS3, PC\n" +
					"'Locale' should be 1 for the 1st language in the given STRING_DIC, or 2 for the second"
				);
				return -1;
			}

			GameVersion? version = null;
			EndianUtils.Endianness endian = EndianUtils.Endianness.BigEndian;
			BitUtils.Bitness bits = BitUtils.Bitness.B32;
			TextUtils.GameTextEncoding encoding = TextUtils.GameTextEncoding.ShiftJIS;
			switch (args[0].ToUpperInvariant()) {
				case "360":
					version = GameVersion.X360_US;
					break;
				case "360EU":
					version = GameVersion.X360_EU;
					encoding = TextUtils.GameTextEncoding.UTF8;
					break;
				case "PS3":
					version = GameVersion.PS3;
					break;
				case "PC":
					version = GameVersion.PC;
					endian = EndianUtils.Endianness.LittleEndian;
					bits = BitUtils.Bitness.B64;
					encoding = TextUtils.GameTextEncoding.UTF8;
					break;
			}
			int locale = 0;
			if (args[1] == "1") {
				locale = 1;
			} else if (args[1] == "2") {
				locale = 2;
			}

			if (version == null || locale == 0) {
				Console.WriteLine("First parameter must indicate game version, second parameter must indicate locale!");
				return -1;
			}

			DuplicatableStream itemDatStream;
			DuplicatableStream itemSortStream;
			DuplicatableStream stringDicStream;
			DuplicatableStream skillsStream;
			DuplicatableStream enemiesStream;
			DuplicatableStream cookdatStream;
			DuplicatableStream locationsStream;
			if (manualmode) {
				itemDatStream = new DuplicatableFileStream(args[2]).CopyToByteArrayStreamAndDispose();
				itemSortStream = new DuplicatableFileStream(args[8]).CopyToByteArrayStreamAndDispose();
				stringDicStream = new DuplicatableFileStream(args[3]).CopyToByteArrayStreamAndDispose();
				skillsStream = new DuplicatableFileStream(args[4]).CopyToByteArrayStreamAndDispose();
				enemiesStream = new DuplicatableFileStream(args[5]).CopyToByteArrayStreamAndDispose();
				cookdatStream = new DuplicatableFileStream(args[6]).CopyToByteArrayStreamAndDispose();
				locationsStream = new DuplicatableFileStream(args[7]).CopyToByteArrayStreamAndDispose();
			} else {
				bool hasCooksvo = VesperiaUtil.Is360(version.Value);

				using (var itemsvo = new FPS4(Path.Combine(args[2], "item.svo"))) {
					itemDatStream = itemsvo.GetChildByName("ITEM.DAT").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
					itemSortStream = itemsvo.GetChildByName("ITEMSORT.DAT").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
				}
				using (var menusvo = new FPS4(Path.Combine(args[2], "menu.svo"))) {
					if (!hasCooksvo) {
						cookdatStream = menusvo.GetChildByName("COOKDATA.BIN").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
					} else {
						using (var cooksvo = new FPS4(Path.Combine(args[2], "cook.svo"))) {
							cookdatStream = cooksvo.GetChildByName("COOKDATA.BIN").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
						}
					}
					locationsStream = menusvo.GetChildByName("WORLDDATA.BIN").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
				}
				if (args.Count == 3) {
					if (version == GameVersion.X360_EU) {
						stringDicStream = new DuplicatableFileStream(Path.Combine(args[2], "language", "string_dic_uk.so")).CopyToByteArrayStreamAndDispose();
					} else if (version == GameVersion.PC) {
						stringDicStream = new DuplicatableFileStream(Path.Combine(args[2], "language", "string_dic_ENG.so")).CopyToByteArrayStreamAndDispose();
					} else {
						using (var stringsvo = new FPS4(Path.Combine(args[2], "string.svo"))) {
							stringDicStream = stringsvo.GetChildByName("STRING_DIC.SO").AsFile.DataStream.CopyToByteArrayStreamAndDispose();
						}
					}
				} else {
					stringDicStream = new DuplicatableFileStream(args[3]).CopyToByteArrayStreamAndDispose();
				}
				using (var btlsvo = new FPS4(Path.Combine(args[2], "btl.svo"))) {
					using (var btlpack = new FPS4(btlsvo.GetChildByName(version == GameVersion.X360_EU ? "BTL_PACK_UK.DAT" : "BTL_PACK.DAT").AsFile.DataStream)) {
						using (var all = new FPS4(btlpack.GetChildByIndex(10).AsFile.DataStream)) {
							skillsStream = all.GetChildByIndex(0).AsFile.DataStream.CopyToByteArrayStreamAndDispose();
						}
						using (var all = new FPS4(btlpack.GetChildByIndex(5).AsFile.DataStream)) {
							enemiesStream = all.GetChildByIndex(0).AsFile.DataStream.CopyToByteArrayStreamAndDispose();
						}
					}
				}
			}


			HyoutaTools.Tales.Vesperia.ItemDat.ItemDat items = new HyoutaTools.Tales.Vesperia.ItemDat.ItemDat(itemDatStream, itemSortStream, EndianUtils.Endianness.BigEndian);

			TSSFile TSS;
			try {
				TSS = new TSSFile(stringDicStream, encoding, endian);
			} catch (System.IO.FileNotFoundException) {
				Console.WriteLine("Could not open STRING_DIC.SO, exiting.");
				return -1;
			}

			HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK skills = new HyoutaTools.Tales.Vesperia.T8BTSK.T8BTSK(skillsStream, endian, bits);
			HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST enemies = new HyoutaTools.Tales.Vesperia.T8BTEMST.T8BTEMST(enemiesStream, endian, bits);
			HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT cookdat = new HyoutaTools.Tales.Vesperia.COOKDAT.COOKDAT(cookdatStream, endian);
			HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT locations = new HyoutaTools.Tales.Vesperia.WRLDDAT.WRLDDAT(locationsStream, endian);

			Console.WriteLine("Initializing GUI...");
			ItemForm itemForm = new ItemForm(version.Value, locale - 1, items, TSS, skills, enemies, cookdat, locations);
			itemForm.Show();
			return 0;
		}
	}
}
