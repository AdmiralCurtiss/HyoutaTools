using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TSS {
	public class TSSEntry {
		public uint[] Entry;
		public String StringJPN;
		public String StringENG;
		public int StringJPNIndex;
		public int StringENGIndex;
		public int inGameStringId;

		public String StringEngOrJpn {
			get {
				return String.IsNullOrEmpty( StringENG ) ? StringJPN : StringENG;
			}
		}

		public TSSEntry( uint[] Entry, String StringJPN, String StringENG, int StringJPNIndex, int StringENGIndex, int inGameStringId ) {
			this.Entry = Entry;
			this.StringJPN = StringJPN;
			this.StringENG = StringENG;
			this.StringJPNIndex = StringJPNIndex;
			this.StringENGIndex = StringENGIndex;
			this.inGameStringId = inGameStringId;
		}

		private void SetPointer( int index, uint Pointer ) {
			Entry[index] = Pointer;
		}

		public void SetJPNPointer( uint Pointer ) {
			SetPointer( StringJPNIndex, Pointer );
		}

		public void SetENGPointer( uint Pointer ) {
			SetPointer( StringENGIndex, Pointer );
		}

		public byte[] SerializeScript() {
			List<byte> bytes = new List<byte>( Entry.Length );
			foreach ( uint e in Entry ) {
				bytes.AddRange( System.BitConverter.GetBytes( Util.SwapEndian( e ) ) );
			}
			return bytes.ToArray();
		}

		public override string ToString() {
			return "[" + inGameStringId + "] " + StringEngOrJpn;
		}
	}
}
