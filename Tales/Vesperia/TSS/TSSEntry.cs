using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TSS {
	public class TSSEntry {
		public uint[] Entry;
		public String StringJpn;
		public String StringEng;
		public int StringJpnIndex;
		public int StringEngIndex;
		public int inGameStringId;

		public String StringEngOrJpn {
			get {
				return String.IsNullOrEmpty( StringEng ) ? StringJpn : StringEng;
			}
		}
		public string StringJpnHtml( GameVersion version ) {
			string jp = StringJpn != null ? StringJpn : "";
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( jp ), version, true ).ToString(), true, true ).Replace( "\n", "<br />" );
		}
		public string StringEngHtml( GameVersion version ) {
			string en = StringEng != null ? StringEng : "";
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( en ), version, false ).ToString(), false, true ).Replace( "\n", "<br />" );
		}
		public string StringEngOrJpnHtml( GameVersion version ) {
			return String.IsNullOrEmpty( StringEng ) ? StringJpnHtml( version ) : StringEngHtml( version );
		}

		public string GetString( int index ) {
			if ( index == 0 ) { return StringJpn; } else { return StringEng; }
		}
		public string GetStringHtml( int index, GameVersion version ) {
			if ( index == 0 ) { return StringJpnHtml( version ); } else { return StringEngHtml( version ); }
		}

		public TSSEntry( uint[] entry, String stringJpn, String stringEng, int stringJpnIndex, int stringEngIndex, int inGameStringId ) {
			this.Entry = entry;
			this.StringJpn = stringJpn;
			this.StringEng = stringEng;
			this.StringJpnIndex = stringJpnIndex;
			this.StringEngIndex = stringEngIndex;
			this.inGameStringId = inGameStringId;
		}

		private void SetPointer( int index, uint pointer ) {
			Entry[index] = pointer;
		}

		public void SetJPNPointer( uint pointer ) {
			SetPointer( StringJpnIndex, pointer );
		}

		public void SetENGPointer( uint pointer ) {
			SetPointer( StringEngIndex, pointer );
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
			/*
			string s = "";
			foreach ( var e in Entry ) {
				s += e.ToString( "X8" ) + " ";
			}
			return s;
			//*/
		}
	}
}
