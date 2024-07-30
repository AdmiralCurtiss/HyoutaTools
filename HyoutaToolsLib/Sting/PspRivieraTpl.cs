using HyoutaPluginBase;
using HyoutaTools.FileContainer;
using HyoutaTools.Tales.Vesperia.NUB;
using HyoutaTools.Textures;
using HyoutaUtils;
using HyoutaUtils.Streams;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Sting {
	public class PspRivieraTpl {

		public static int ExecuteConvertToPng(List<string> args) {
			if (args.Count == 0) {
				Console.WriteLine("Sting.PspRivieraTpl.ConvertToPng file.tpl [output_folder]");
				return -1;
			}

			try {
				ConvertImagesToPng(new DuplicatableFileStream(args[0]), args.Count > 1 ? args[1] : (args[0] + ".ex"));
				return 0;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				return -1;
			}
		}

		public static void ConvertImagesToPng(DuplicatableStream inputFile, string outputFolder) {
			var file = inputFile.CopyToByteArrayStream();
			System.IO.Directory.CreateDirectory(outputFolder);

			uint imageCount = file.ReadUInt32();
			uint unknown = file.ReadUInt32();

			for (uint i = 0; i < imageCount; ++i) {
				uint offsetHeader1 = file.ReadUInt32();
				uint offsetHeader2 = file.ReadUInt32();

				long pos = file.Position;

				file.Position = offsetHeader1;
				ushort unknown0 = file.ReadUInt16();
				ushort unknown1 = file.ReadUInt16();
				ushort width = file.ReadUInt16();
				ushort unknown7 = file.ReadUInt16();
				uint offsetData = file.ReadUInt32();
				ushort unknown2 = file.ReadUInt16();
				ushort unknown3 = file.ReadUInt16();
				ushort unknown4 = file.ReadUInt16();
				ushort unknown5 = file.ReadUInt16();

				file.Position = offsetHeader2;
				ushort colorCount = file.ReadUInt16();
				ushort unknown6 = file.ReadUInt16(); // color format?
				uint offsetPalette = file.ReadUInt32();

				DuplicatableStream palette = file.ReadDuplicatableSubstreamFromLocationAndReset(offsetPalette, colorCount * 4);
				Stream pngStream = null;

				// bizarrely, height is not stored. so guess that based on offsets
				int bpp = colorCount == 16 ? 4 : 8;
				ushort height = (ushort)((((offsetPalette - offsetData) * 8) / bpp) / width);

				if (colorCount == 16) {
					pngStream = TextureUtil.WriteSingleImageToPngStream(new Textures.ColorFetchingIterators.ColorFetcherIndexed4Bits(
						file.ReadDuplicatableSubstreamFromLocationAndReset(offsetData, (width * height) / 2),
						width,
						height,
						new Textures.ColorFetchingIterators.ColorFetcherRGBA8888(palette.Duplicate(), colorCount, 1),
						flipped: true
					), new Textures.PixelOrderIterators.TiledPixelOrderIterator(width, (int)height, 32, 8), width, height);
				} else if (colorCount == 256) {
					pngStream = TextureUtil.WriteSingleImageToPngStream(new Textures.ColorFetchingIterators.ColorFetcherIndexed8Bits(
						file.ReadDuplicatableSubstreamFromLocationAndReset(offsetData, width * height),
						width,
						height,
						new Textures.ColorFetchingIterators.ColorFetcherRGBA8888(palette.Duplicate(), colorCount, 1)
					), new Textures.PixelOrderIterators.TiledPixelOrderIterator(width, (int)height, 16, 8), width, height);
				} else {
					Console.WriteLine("unimplemented format");
				}
				if (pngStream != null) {
					using (FileStream newFile = new FileStream(System.IO.Path.Combine(outputFolder, i.ToString() + ".png"), FileMode.Create)) {
						pngStream.Position = 0;
						StreamUtils.CopyStream(pngStream, newFile);
					}
				}

				file.Position = pos;
			}
		}
	}
}
