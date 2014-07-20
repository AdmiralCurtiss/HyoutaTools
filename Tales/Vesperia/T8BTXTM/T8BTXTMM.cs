using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMM {
		// map definitions
		public T8BTXTMM( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading T8BTXTMM failed!" );
				}
			}
		}

		public T8BTXTMM( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading T8BTXTMM failed!" );
			}
		}

		public List<MapTile> TileList;
		public uint HorizontalTiles;
		public uint VerticalTiles;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown1 = stream.ReadUInt32().SwapEndian();
			uint bytesToEnd = stream.ReadUInt32().SwapEndian();
			uint unknown2 = stream.ReadUInt32().SwapEndian();
			uint unknown3 = stream.ReadUInt32().SwapEndian();
			HorizontalTiles = stream.ReadUInt32().SwapEndian();
			VerticalTiles = stream.ReadUInt32().SwapEndian();
			uint tileCount = stream.ReadUInt32().SwapEndian();

			TileList = new List<MapTile>( (int)tileCount );
			for ( uint i = 0; i < tileCount; ++i ) {
				MapTile mt = new MapTile( stream );
				TileList.Add( mt );
			}

			return true;
		}

		public string GetDataAsHtml( string stratum, int floor, GameVersion version, T8BTXTMT treasures, ItemDat.ItemDat items, Dictionary<uint, TSS.TSSEntry> inGameIdDict ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "<div id=\"" + stratum + floor + "\">" );
			sb.Append( "<div class=\"itemname\" style=\"text-align: center;\">" );
			sb.Append( stratum + "-" + floor );
			sb.Append( "</div>" );
			sb.Append( "<table class=\"necropolisfloor\">" );
			for ( int y = 0; y < VerticalTiles; y++ ) {
				sb.Append( "<tr>" );
				for ( int x = 0; x < HorizontalTiles; x++ ) {
					sb.Append( TileList[(int)( y * HorizontalTiles + x )].GetDataAsHtml( stratum, floor, version, treasures, items, inGameIdDict ) );
				}
				sb.Append( "</tr>" );
				//sb.Append( "<tr><td colspan=\"" + HorizontalTiles + "\"><hr></td></tr>" );
			}
			sb.Append( "</table>" );
			sb.Append( "</div>" );

			return sb.ToString();
		}
	}
}
