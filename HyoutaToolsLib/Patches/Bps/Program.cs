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

		private static void PrintUsageCreate() {
			Console.WriteLine("Usage: [options] source.bin target.bin patch.bps");
			Console.WriteLine("Options:");
			Console.WriteLine("  --limit integer   Amount of bytes that must be the same for a run of TargetRead.");
		}

		public static int ExecuteCreate(List<string> args) {
			if (args.Count < 3) {
				PrintUsageCreate();
				return -1;
			}

			string sourcepath = null;
			string targetpath = null;
			string patchpath  = null;
			ulong limit = 0;

			try {
				for (int i = 0; i < args.Count; ++i) {
					if (args[i] == "--limit") {
						++i;
						limit = ulong.Parse(args[i]);
					} else if (sourcepath == null) {
						sourcepath = args[i];
					} else if (targetpath == null) {
						targetpath = args[i];
					} else if (patchpath == null) {
						patchpath = args[i];
					}
				}
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
				PrintUsageCreate();
				return -1;
			}

			if (sourcepath == null || targetpath == null || patchpath == null) {
				PrintUsageCreate();
				return -1;
			}

			using (var sourcestream = new HyoutaUtils.Streams.DuplicatableFileStream(sourcepath))
			using (var targetstream = new HyoutaUtils.Streams.DuplicatableFileStream(targetpath))
			using (var outstream = new FileStream(patchpath, FileMode.Create)) {
				CreateSimplest.CreatePatch(sourcestream, targetstream, outstream, limit);
			}

			return 0;
		}
	}
}
