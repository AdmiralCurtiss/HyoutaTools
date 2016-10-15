using System;
using System.Collections.Generic;
using System.IO;
using HyoutaTools.Tales.Vesperia.ItemDat;
using HyoutaTools.Tales.Vesperia.TSS;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.TOVSEAF {
	public class TOVSEAF {
		// Search Point definition file
		public TOVSEAF( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TOVSEAF failed!" );
				}
			}
		}

		public TOVSEAF( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TOVSEAF failed!" );
			}
		}

		public List<SearchPointDefinition> SearchPointDefinitions;
		public List<SearchPointContent> SearchPointContents;
		public List<SearchPointItem> SearchPointItems;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint fileSize = stream.ReadUInt32().SwapEndian();
			uint definitionsStart = stream.ReadUInt32().SwapEndian();
			uint definitionsCount = stream.ReadUInt32().SwapEndian(); // 64 bytes per entry
			uint contentsStart = stream.ReadUInt32().SwapEndian();
			uint contentsCount = stream.ReadUInt32().SwapEndian(); // 16 bytes per entry
			uint itemsStart = stream.ReadUInt32().SwapEndian();
			uint itemsCount = stream.ReadUInt32().SwapEndian(); // 8 bytes per entry
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			stream.Position = definitionsStart;
			SearchPointDefinitions = new List<SearchPointDefinition>( (int)definitionsCount );
			for ( uint i = 0; i < definitionsCount; ++i ) {
				SearchPointDefinitions.Add( new SearchPointDefinition( stream ) );
			}

			stream.Position = contentsStart;
			SearchPointContents = new List<SearchPointContent>( (int)contentsCount );
			for ( uint i = 0; i < contentsCount; ++i ) {
				SearchPointContents.Add( new SearchPointContent( stream ) );
			}

			stream.Position = itemsStart;
			SearchPointItems = new List<SearchPointItem>( (int)itemsCount );
			for ( uint i = 0; i < itemsCount; ++i ) {
				SearchPointItems.Add( new SearchPointItem( stream ) );
			}

			return true;
		}
	}

	public class SearchPointDefinition {
		public uint Index;
		public uint ScenarioBegin; // shows up once scenario counter is this value, usually 1000999 (start of game)
		public uint ScenarioEnd; // disappears once scenario counter is this value, always 9999999 (which means none disappear)
		public uint Unknown4; // range [0,3]

		public uint Unknown5; // always 0
		public int CoordX; // these are definitely signed ints but I'm not sure what they actually mean, they feel like world position but why are they ints and not floats then?
		public int CoordY;
		public int CoordZ;

		public uint Unknown9; // spawn chance, maybe? has numbers like 75, 80, 85, 90, etc, rarely lower, always in 5 increments
		public uint Unknown10; // always 10000
		public uint Unknown11; // usually 1, rarely 10
		public uint Unknown12; // always 0

		public uint Unknown13; // always 0
		public ushort Unknown14a; // range [2,7]
		public ushort Unknown14b; // always 100
		public uint SearchPointContentIndex;
		public uint SearchPointContentCount; // usually 5, rarely 3

		public SearchPointDefinition( System.IO.Stream stream ) {
			Index = stream.ReadUInt32().SwapEndian();
			ScenarioBegin = stream.ReadUInt32().SwapEndian();
			ScenarioEnd = stream.ReadUInt32().SwapEndian();
			Unknown4 = stream.ReadUInt32().SwapEndian();

			Unknown5 = stream.ReadUInt32().SwapEndian();
			CoordX = stream.ReadUInt32().SwapEndian().AsSigned();
			CoordY = stream.ReadUInt32().SwapEndian().AsSigned();
			CoordZ = stream.ReadUInt32().SwapEndian().AsSigned();

			Unknown9 = stream.ReadUInt32().SwapEndian();
			Unknown10 = stream.ReadUInt32().SwapEndian();
			Unknown11 = stream.ReadUInt32().SwapEndian();
			Unknown12 = stream.ReadUInt32().SwapEndian();

			Unknown13 = stream.ReadUInt32().SwapEndian();
			Unknown14a = stream.ReadUInt16().SwapEndian();
			Unknown14b = stream.ReadUInt16().SwapEndian();
			SearchPointContentIndex = stream.ReadUInt32().SwapEndian();
			SearchPointContentCount = stream.ReadUInt32().SwapEndian();
		}

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, TSSFile stringDic, Dictionary<uint, TSSEntry> inGameIdDict, List<SearchPointContent> searchPointContents, List<SearchPointItem> searchPointItems, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();

			sb.Append( "Index: " ).Append( Index ).Append( "<br>" );
			sb.Append( "2: " ).Append( ScenarioBegin ).Append( "<br>" );
			sb.Append( "3: " ).Append( ScenarioEnd ).Append( "<br>" );
			sb.Append( "4: " ).Append( Unknown4 ).Append( "<br>" );

			sb.Append( "5: " ).Append( Unknown5 ).Append( "<br>" );
			sb.Append( "6: " ).Append( CoordX ).Append( "<br>" );
			sb.Append( "7: " ).Append( CoordY ).Append( "<br>" );
			sb.Append( "8: " ).Append( CoordZ ).Append( "<br>" );

			sb.Append( "9: " ).Append( Unknown9 ).Append( "<br>" );
			sb.Append( "10: " ).Append( Unknown10 ).Append( "<br>" );
			sb.Append( "11: " ).Append( Unknown11 ).Append( "<br>" );
			sb.Append( "12: " ).Append( Unknown12 ).Append( "<br>" );

			sb.Append( "13: " ).Append( Unknown13 ).Append( "<br>" );
			sb.Append( "14a: " ).Append( Unknown14a ).Append( "<br>" );
			sb.Append( "14b: " ).Append( Unknown14b ).Append( "<br>" );

			sb.Append( "<br>" );

			for ( uint i = 0; i < SearchPointContentCount; ++i ) {
				sb.Append( "Content #" ).Append( i ).Append( ":" ).Append( "<br>" );
				sb.Append( searchPointContents[(int)( SearchPointContentIndex + i )].GetDataAsHtml( version, items, stringDic, inGameIdDict, searchPointItems ) );
			}

			return sb.ToString();
		}
	}

	public class SearchPointContent {
		public uint Percentage;
		public uint SearchPointItemIndex;
		public uint SearchPointItemCount;
		public uint Padding;

		public SearchPointContent( System.IO.Stream stream ) {
			Percentage = stream.ReadUInt32().SwapEndian();
			SearchPointItemIndex = stream.ReadUInt32().SwapEndian();
			SearchPointItemCount = stream.ReadUInt32().SwapEndian();
			Padding = stream.ReadUInt32().SwapEndian();
		}

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, TSSFile stringDic, Dictionary<uint, TSSEntry> inGameIdDict, List<SearchPointItem> searchPointItems, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();
			sb.Append( "Percentage: " ).Append( Percentage ).Append( "%" ).Append( "<br>" );
			for ( uint i = 0; i < SearchPointItemCount; ++i ) {
				sb.Append( "Item #" ).Append( i ).Append( ":" );
				sb.Append( searchPointItems[(int)( SearchPointItemIndex + i )].GetDataAsHtml( version, items, stringDic, inGameIdDict ) );
				sb.Append( "<br>" );
			}
			return sb.ToString();
		}
	}

	public class SearchPointItem {
		public uint Item;
		public uint Count;

		public SearchPointItem( System.IO.Stream stream ) {
			Item = stream.ReadUInt32().SwapEndian();
			Count = stream.ReadUInt32().SwapEndian();
		}

		public string GetDataAsHtml( GameVersion version, ItemDat.ItemDat items, TSSFile stringDic, Dictionary<uint, TSSEntry> inGameIdDict, bool phpLinks = false ) {
			StringBuilder sb = new StringBuilder();
			var item = items.itemIdDict[Item];
			sb.Append( "<img src=\"item-icons/ICON" + item.Data[(int)ItemData.Icon] + ".png\" height=\"16\" width=\"16\"> " );
			sb.Append( "<a href=\"" + Website.GenerateWebsite.GetUrl( Website.WebsiteSection.Item, version, phpLinks, id: (int)item.Data[(int)ItemData.ID], icon: (int)item.Data[(int)ItemData.Icon] ) + "\">" );
			sb.Append( inGameIdDict[item.NamePointer].StringEngOrJpnHtml( version ) + "</a> x" + Count );
			return sb.ToString();
		}
	}
}
