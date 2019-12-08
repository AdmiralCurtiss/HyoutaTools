using HyoutaPluginBase.FileContainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.FileContainer {
	// Helper class to add a proper directory hierarchy to containers that present them as a flat list.
	public class ContainerPathSplitWrapper : IContainer {
		public bool IsFile => false;
		public bool IsContainer => true;
		public IFile AsFile => null;
		public IContainer AsContainer => this;

		private struct Element {
			public string Fullname;
			public string Name;
			public IContainer Subdir;
		}
		private IContainer WrappedContainer;
		private List<Element> Elements;

		private ContainerPathSplitWrapper(IContainer wrappedContainer, List<Element> elements) {
			WrappedContainer = wrappedContainer;
			Elements = elements;
		}

		public static ContainerPathSplitWrapper CreateFromContainer(IContainer container) {
			return CreateFromContainer(container, new char[] { '/' });
		}

		public static ContainerPathSplitWrapper CreateFromContainer(IContainer container, char[] separators) {
			return CreateFromContainer(container, separators, container.GetChildNames().ToList(), 0);
		}

		private static ContainerPathSplitWrapper CreateFromContainer(IContainer container, char[] separators, List<string> remainingFiles, int currentPrefixDepth) {
			SortedDictionary<string, List<string>> directSubdirs = new SortedDictionary<string, List<string>>();
			List<string> filesInCurrentDir = new List<string>();
			for (int i = 0; i < remainingFiles.Count; ++i) {
				string fn = remainingFiles[i];
				int sepIdx = fn.IndexOfAny(separators, currentPrefixDepth);
				if (sepIdx < 0) {
					// is a file in current dir
					filesInCurrentDir.Add(fn);
				} else {
					// is a directory
					string dirname = fn.Substring(0, sepIdx);
					List<string> filesInSubdir;
					if (!directSubdirs.TryGetValue(dirname, out filesInSubdir)) {
						filesInSubdir = new List<string>();
						directSubdirs.Add(dirname, filesInSubdir);
					}
					filesInSubdir.Add(fn);
				}
			}

			List<Element> elements = new List<Element>(directSubdirs.Count + filesInCurrentDir.Count);
			foreach (var subdir in directSubdirs) {
				ContainerPathSplitWrapper c = CreateFromContainer(container, separators, subdir.Value, subdir.Key.Length + 1);
				elements.Add(new Element() { Fullname = subdir.Key, Name = subdir.Key.Substring(currentPrefixDepth), Subdir = c });
			}
			foreach (string f in filesInCurrentDir) {
				elements.Add(new Element() { Fullname = f, Name = f.Substring(currentPrefixDepth), Subdir = null });
			}

			return new ContainerPathSplitWrapper(container, elements);
		}

		public INode GetChildByIndex(long index) {
			if (index < 0 && index >= Elements.Count) {
				return null;
			}

			Element e = Elements[(int)index];
			if (e.Subdir == null) {
				return WrappedContainer.GetChildByName(e.Fullname);
			} else {
				return e.Subdir;
			}
		}

		public INode GetChildByName(string name) {
			for (int i = 0; i < Elements.Count; i++) {
				Element e = Elements[i];
				if (e.Name == name) {
					return GetChildByIndex(i);
				}
			}

			return null;
		}

		public IEnumerable<string> GetChildNames() {
			foreach (Element e in Elements) {
				yield return e.Name;
			}
		}

		#region IDisposable Support
		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing) {
			if (!disposedValue) {
				if (disposing) {
					WrappedContainer.Dispose();
				}

				WrappedContainer = null;
				Elements = null;
				disposedValue = true;
			}
		}

		public void Dispose() {
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
		}
		#endregion
	}
}
