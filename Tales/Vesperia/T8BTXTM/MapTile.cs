using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class MapTile {
		public uint RoomType;
		public int Unknown2;
		public uint Unknown3;
		public uint Unknown4;

		public uint Unknown5;
		public uint Unknown6;
		public uint MoveUpAllowed;
		public uint MoveDownAllowed;

		public uint MoveLeftAllowed;
		public uint MoveRightAllowed;

		public MapTile( System.IO.Stream stream ) {
			RoomType = stream.ReadUInt32().SwapEndian();
			Unknown2 = (int)stream.ReadUInt32().SwapEndian();
			Unknown3 = stream.ReadUInt32().SwapEndian();
			Unknown4 = stream.ReadUInt32().SwapEndian();

			Unknown5 = stream.ReadUInt32().SwapEndian();
			Unknown6 = stream.ReadUInt32().SwapEndian();
			MoveUpAllowed = stream.ReadUInt32().SwapEndian();
			MoveDownAllowed = stream.ReadUInt32().SwapEndian();

			MoveLeftAllowed = stream.ReadUInt32().SwapEndian();
			MoveRightAllowed = stream.ReadUInt32().SwapEndian();
		}

		public string GetDataAsHtml() {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<td>" );
			if ( RoomType != 0 ) {
				sb.Append( "Type: " + RoomType + "<br>" );
				switch ( RoomType ) {
					case 1: sb.Append( "Entrance<br>" ); break;
					case 2: sb.Append( "Default Exit<br>" ); break;
					case 3: sb.Append( "Regular Room<br>" ); break;
					case 4: sb.Append( "Treasure Room<br>" ); break;
					case 5: sb.Append( "Alternate Exit, moves " + Unknown2 + " floors.<br>" ); break;
				}
				sb.Append( "Enemy Group: " + Unknown3 + " (?)<br>" );
				sb.Append( "Time to move: " + Unknown4 + " (?)<br>" );
				if ( Unknown5 > 0 ) { sb.Append( "Normal Treasure: " + Unknown5 + "<br>" ); }
				if ( Unknown6 > 0 ) { sb.Append( "One-Time Treasure: " + Unknown6 + "<br>" ); }
				if ( MoveUpAllowed > 0 ) { sb.Append( "↑" ); }
				if ( MoveDownAllowed > 0 ) { sb.Append( "↓" ); }
				if ( MoveLeftAllowed > 0 ) { sb.Append( "←" ); }
				if ( MoveRightAllowed > 0 ) { sb.Append( "→" ); }
			} else {
				sb.Append( "" );
			}
			sb.Append( "</td>" );

			return sb.ToString();
		}
	}
}
