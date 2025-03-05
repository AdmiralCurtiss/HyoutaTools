using HyoutaPluginBase;
using HyoutaUtils.Streams;
using HyoutaUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Textures;

namespace HyoutaTools.Other {
	internal class Splice {
		public uint X;
		public uint Y;
		public uint Width;
		public uint Height;
	};

	public class DDSSplicer {
		private static DDS DoSplice(DuplicatableFileStream input, DuplicatableFileStream overlay, List<Splice> splices) {
			input.Position = 0;
			var dds_input = new DDS(input);
			input.Position = 0;
			var dds_output = new DDS(input);
			overlay.Position = 0;
			var dds_overlay = new DDS(overlay);

			if (dds_input.Header.MipMapCount != 1) {
				Console.WriteLine("Warning: Input texture has mipmaps (" + dds_input.Header.MipMapCount + "). Mips should be regenerated or discarded after splice.");
			}
			if (dds_input.Header.PixelFormat.FourCC != dds_overlay.Header.PixelFormat.FourCC) {
				throw new Exception("Inconsistent formats between input and overlay.");
			}
			if (dds_input.Header.PixelFormat.FourCC == 0x30315844) {
				if (dds_input.Header.DXT10_DxgiFormat != dds_overlay.Header.DXT10_DxgiFormat) {
					throw new Exception("Inconsistent formats between input and overlay.");
				}
			}

			// DDS is stored in 4x4 blocks
			uint input_width_pixels = dds_input.Header.Width;
			uint input_height_pixels = dds_input.Header.Height;
			uint input_width_blocks = NumberUtils.Align(input_width_pixels, 4) / 4;
			uint input_height_blocks = NumberUtils.Align(input_height_pixels, 4) / 4;
			uint overlay_width_pixels = dds_overlay.Header.Width;
			uint overlay_height_pixels = dds_overlay.Header.Height;
			uint overlay_width_blocks = NumberUtils.Align(overlay_width_pixels, 4) / 4;
			uint overlay_height_blocks = NumberUtils.Align(overlay_height_pixels, 4) / 4;

			foreach (Splice splice in splices) {
				uint splice_x_pixels = splice.X;
				uint splice_y_pixels = splice.Y;
				uint splice_width_pixels = splice.Width;
				uint splice_height_pixels = splice.Height;

				// align the request to blocks
				splice_width_pixels += (splice_x_pixels & 3);
				splice_x_pixels -= (splice_x_pixels & 3);
				splice_height_pixels += (splice_y_pixels & 3);
				splice_y_pixels -= (splice_y_pixels & 3);
				splice_width_pixels = NumberUtils.Align(splice_width_pixels, 4);
				splice_height_pixels = NumberUtils.Align(splice_height_pixels, 4);

				uint splice_x_blocks = splice_x_pixels / 4;
				uint splice_y_blocks = splice_y_pixels / 4;
				uint splice_width_blocks = splice_width_pixels / 4;
				uint splice_height_blocks = splice_height_pixels / 4;

				for (uint y = 0; y < splice_height_blocks; ++y) {
					for (uint x = 0; x < splice_width_blocks; ++x) {
						uint splice_x_block = splice_x_blocks + x;
						uint splice_y_block = splice_y_blocks + y;

						uint splice_offset = (splice_y_block * overlay_width_blocks + splice_x_block) * 16;
						uint output_offset = (splice_y_block * input_width_blocks + splice_x_block) * 16;

						dds_overlay.Data.Position = splice_offset;
						byte[] data = dds_overlay.Data.ReadBytes(16);
						dds_output.Data.Position = output_offset;
						dds_output.Data.Write(data);
					}
				}
			}

			return dds_output;
		}

		public static int Execute(List<string> args) {
			if (args.Count == 0) {
				Console.WriteLine("Other.DDSSplicer input.dds overlay.dds output.dds [splices...]");
				Console.WriteLine("where each splice is in basic imagemagick geometry notation, ie. 100x200+12+8 for a rectangle starting at 100/200 and ending at 112/108");
				return -1;
			}

			try {
				List<Splice> splices = new List<Splice>();

				for (int i = 3; i < args.Count; ++i) {
					string s = args[i].Trim();
					int last = -1;
					var numbersWithPrefix = new List<(uint, char)>();
					for (int j = 0; j <= s.Length; ++j) {
						char c = j < s.Length ? s[j] : '\0';
						if (c >= '0' && c <= '9') {
							continue;
						}
						string numberstring = s.Substring(last + 1, j - (last + 1));
						uint number = uint.Parse(numberstring);
						char prefix = last == -1 ? '\0' : s[last];
						last = j;
						numbersWithPrefix.Add((number, prefix));
					}
					// TODO: support more formats?
					if (numbersWithPrefix.Count != 4) {
						throw new Exception("invalid geometry format for argument: " + s);
					}
					var (x, x_prefix) = numbersWithPrefix[0];
					var (y, y_prefix) = numbersWithPrefix[1];
					var (w, w_prefix) = numbersWithPrefix[2];
					var (h, h_prefix) = numbersWithPrefix[3];
					if (x_prefix != '\0' || y_prefix != 'x' || w_prefix != '+' || h_prefix != '+') {
						throw new Exception("invalid geometry format for argument: " + s);
					}

					splices.Add(new Splice() { X = x, Y = y, Width = w, Height = h });
				}

				DDS tex;
				using (var input = new DuplicatableFileStream(args[0]))
				using (var overlay = new DuplicatableFileStream(args[1])) {
					tex = DoSplice(input, overlay, splices);
				}

				using (var output = new FileStream(args[2], FileMode.Create)) {
					byte[] data = tex.ToBytes();
					output.Write(data);
				}

				return 0;
			} catch (Exception e) {
				Console.WriteLine(e.Message);
				return -1;
			}
		}
	}
}
