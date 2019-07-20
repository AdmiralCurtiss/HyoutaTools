using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EndianUtils = HyoutaUtils.EndianUtils;

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
		public string StringJpnHtml( GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			string jp = StringJpn != null ? StringJpn : "";
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( jp ), inGameIdDict, version, true ).ToString(), inGameIdDict, true, true ).Replace( "\n", "<br />" );
		}
		public string StringEngHtml( GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			string en = StringEng != null ? StringEng : "";
			return VesperiaUtil.RemoveTags( Website.WebsiteGenerator.ReplaceIconsWithHtml( new StringBuilder( en ), inGameIdDict, version, false ).ToString(), inGameIdDict, false, true ).Replace( "\n", "<br />" );
		}
		public string StringEngOrJpnHtml( GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameIdDict, WebsiteLanguage lang ) {
			if ( lang == WebsiteLanguage.Jp || lang == WebsiteLanguage.BothWithJpLinks ) {
				return String.IsNullOrEmpty( StringJpn ) ? StringEngHtml( version, inGameIdDict ) : StringJpnHtml( version, inGameIdDict );
			} else {
				return String.IsNullOrEmpty( StringEng ) ? StringJpnHtml( version, inGameIdDict ) : StringEngHtml( version, inGameIdDict );
			}
		}

		public string GetString( int index ) {
			if ( index == 0 ) { return StringJpn; } else { return StringEng; }
		}
		public string GetStringHtml( int index, GameVersion version, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			if ( index == 0 ) { return StringJpnHtml( version, inGameIdDict ); } else { return StringEngHtml( version, inGameIdDict ); }
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
				bytes.AddRange( System.BitConverter.GetBytes( EndianUtils.SwapEndian( e ) ) );
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
