using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class TO8CHLI {
		public TO8CHLI( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TO8CHLI failed!" );
				}
			}
		}

		public TO8CHLI( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TO8CHLI failed!" );
			}
		}

		public List<SkitInfo> SkitInfoList;
		public List<SkitConditionForwarder> SkitConditionForwarderList;
		public List<SkitCondition> SkitConditionList;
		public List<UnknownSkitData4> UnknownSkitData4List;
		public List<UnknownSkitData5> UnknownSkitData5List;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint fileSize = stream.ReadUInt32().SwapEndian();
			uint skitInfoCount = stream.ReadUInt32().SwapEndian();
			uint skitInfoOffset = stream.ReadUInt32().SwapEndian();
			uint conditionForwarderCount = stream.ReadUInt32().SwapEndian();
			uint conditionForwarderOffset = stream.ReadUInt32().SwapEndian();
			uint conditionCount = stream.ReadUInt32().SwapEndian();
			uint conditionOffset = stream.ReadUInt32().SwapEndian();
			uint uCount4 = stream.ReadUInt32().SwapEndian();
			uint uOffset4 = stream.ReadUInt32().SwapEndian();
			uint uCount5 = stream.ReadUInt32().SwapEndian();
			uint uOffset5 = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();

			SkitInfoList = new List<SkitInfo>( (int)skitInfoCount );
			stream.Position = skitInfoOffset;
			for ( uint i = 0; i < skitInfoCount; ++i ) {
				SkitInfo s = new SkitInfo( stream, refStringStart );
				SkitInfoList.Add( s );
			}

			SkitConditionForwarderList = new List<SkitConditionForwarder>( (int)conditionForwarderCount );
			stream.Position = conditionForwarderOffset;
			for ( uint i = 0; i < conditionForwarderCount; ++i ) {
				var s = new SkitConditionForwarder( stream );
				SkitConditionForwarderList.Add( s );
			}

			SkitConditionList = new List<SkitCondition>( (int)conditionCount );
			stream.Position = conditionOffset;
			for ( uint i = 0; i < conditionCount; ++i ) {
				var s = new SkitCondition( stream );
				SkitConditionList.Add( s );
			}

			UnknownSkitData4List = new List<UnknownSkitData4>( (int)uCount4 );
			stream.Position = uOffset4;
			for ( uint i = 0; i < uCount4; ++i ) {
				var s = new UnknownSkitData4( stream );
				UnknownSkitData4List.Add( s );
			}

			UnknownSkitData5List = new List<UnknownSkitData5>( (int)uCount5 );
			stream.Position = uOffset5;
			for ( uint i = 0; i < uCount5; ++i ) {
				var s = new UnknownSkitData5( stream );
				UnknownSkitData5List.Add( s );
			}

			return true;
		}
	}
}
