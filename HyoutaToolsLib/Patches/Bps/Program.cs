using System;
using System.Collections.Generic;
using System.IO;

namespace HyoutaTools.Patches.Bps {
	public static class Program {
		public static int ExecutePatch(List<string> args) {
			if (args.Count != 3) {
				Console.WriteLine("Usage: BpsPatch source patch target");
				return -1;
			}

			string sourceName = args[0];
			string patchName = args[1];
			string targetName = args[2];
			using (var source = new FileStream(sourceName, FileMode.Open, FileAccess.Read))
			using (var patch = new FileStream(patchName, FileMode.Open, FileAccess.Read))
			using (var target = new FileStream(targetName, FileMode.Create, FileAccess.ReadWrite)) {
				HyoutaUtils.Bps.BpsPatcher.ApplyPatchToStream(source, patch, target);
			}
			return 0;
		}

		public static int ExecuteCreate(List<string> args) {
			if (args.Count < 3) {
				Console.WriteLine("Usage: source.bin target.bin patch.bps");
				return -1;
			}

			string sourcepath = args[0];
			string targetpath = args[1];
			string patchpath = args[2];

			using (var sourcestream = new HyoutaUtils.Streams.DuplicatableFileStream(sourcepath))
			using (var targetstream = new HyoutaUtils.Streams.DuplicatableFileStream(targetpath))
			using (var outstream = new FileStream(patchpath, FileMode.Create)) {
				CreateSimplest.CreatePatch(sourcestream, targetstream, outstream);
			}

			return 0;
		}
	}
}
