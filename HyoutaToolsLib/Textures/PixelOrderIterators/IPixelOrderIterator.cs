using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HyoutaTools.Textures.PixelOrderIterators {
	public interface IPixelOrderIterator : IEnumerable<(int X, int Y)> {
	}
}
