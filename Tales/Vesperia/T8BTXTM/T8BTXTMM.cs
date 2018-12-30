using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.T8BTXTM {
	public class T8BTXTMM {
		// map definitions
		public T8BTXTMM( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading T8BTXTMM failed!" );
				}
			}
		}

		public T8BTXTMM( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading T8BTXTMM failed!" );
			}
		}

		public List<MapTile> TileList;
		public uint HorizontalTiles;
		public uint VerticalTiles;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			uint unknown1 = stream.ReadUInt32().FromEndian( endian );
			uint bytesToEnd = stream.ReadUInt32().FromEndian( endian );
			uint unknown2 = stream.ReadUInt32().FromEndian( endian );
			uint unknown3 = stream.ReadUInt32().FromEndian( endian );
			HorizontalTiles = stream.ReadUInt32().FromEndian( endian );
			VerticalTiles = stream.ReadUInt32().FromEndian( endian );
			uint tileCount = stream.ReadUInt32().FromEndian( endian );

			TileList = new List<MapTile>( (int)tileCount );
			for ( uint i = 0; i < tileCount; ++i ) {
				MapTile mt = new MapTile( stream, endian );
				TileList.Add( mt );
			}

			return true;
		}

		public string GetDataAsHtml( string stratum, int floor, T8BTEMST.T8BTEMST Enemies, T8BTEMGP.T8BTEMGP EnemyGroups, T8BTEMEG.T8BTEMEG EncounterGroups, GameVersion version, T8BTXTMT treasures, ItemDat.ItemDat items, Dictionary<uint, TSS.TSSEntry> inGameIdDict, bool surroundingTable = true, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();

			if ( surroundingTable ) {
				sb.Append( "<div id=\"" + stratum + floor + "\">" );
				sb.Append( "<table class=\"necropolisfloor\">" );
				sb.Append( "<tr>" );
				sb.Append( "<th colspan=\"6\">" );
				sb.Append( "<div class=\"itemname\" style=\"text-align: center;\">" );
				sb.Append( stratum + "-" + floor );
				sb.Append( "</div>" );
				sb.Append( "</td>" );
				sb.Append( "</tr>" );
			}
			for ( int y = 0; y < VerticalTiles; y++ ) {
				sb.Append( "<tr>" );
				for ( int x = 0; x < HorizontalTiles; x++ ) {
					sb.Append( TileList[(int)( y * HorizontalTiles + x )].GetDataAsHtml( stratum, floor, Enemies, EnemyGroups, EncounterGroups, version, treasures, items, inGameIdDict, phpLinks: phpLinks ) );
				}
				sb.Append( "</tr>" );
				//sb.Append( "<tr><td colspan=\"" + HorizontalTiles + "\"><hr></td></tr>" );
			}
			if ( surroundingTable ) {
				sb.Append( "</table>" );
				sb.Append( "</div>" );
			}

			return sb.ToString();
		}
	}
}
