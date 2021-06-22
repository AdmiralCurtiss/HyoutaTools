using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaUtils;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public class TO8CHLI {
		public TO8CHLI( String filename, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian, bits ) ) {
					throw new Exception( "Loading TO8CHLI failed!" );
				}
			}
		}

		public TO8CHLI( Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			if ( !LoadFile( stream, endian, bits ) ) {
				throw new Exception( "Loading TO8CHLI failed!" );
			}
		}

		public List<SkitInfo> SkitInfoList;
		public List<SkitConditionForwarder> SkitConditionForwarderList;
		public List<SkitCondition> SkitConditionList;
		public List<UnknownSkitData4> UnknownSkitData4List;
		public List<UnknownSkitData5> UnknownSkitData5List;

		private bool LoadFile( Stream stream, EndianUtils.Endianness endian, BitUtils.Bitness bits ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "TO8CHLI\0" ) {
				throw new Exception( "Invalid magic." );
			}
			uint fileSize = stream.ReadUInt32().FromEndian( endian );
			uint skitInfoCount = stream.ReadUInt32().FromEndian( endian );
			uint skitInfoOffset = stream.ReadUInt32().FromEndian( endian );
			uint conditionForwarderCount = stream.ReadUInt32().FromEndian( endian );
			uint conditionForwarderOffset = stream.ReadUInt32().FromEndian( endian );
			uint conditionCount = stream.ReadUInt32().FromEndian( endian );
			uint conditionOffset = stream.ReadUInt32().FromEndian( endian );
			uint uCount4 = stream.ReadUInt32().FromEndian( endian );
			uint uOffset4 = stream.ReadUInt32().FromEndian( endian );
			uint uCount5 = stream.ReadUInt32().FromEndian( endian );
			uint uOffset5 = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );

			SkitInfoList = new List<SkitInfo>( (int)skitInfoCount );
			stream.Position = skitInfoOffset;
			for ( uint i = 0; i < skitInfoCount; ++i ) {
				SkitInfo s = new SkitInfo( stream, refStringStart, endian, bits );
				SkitInfoList.Add( s );
			}

			SkitConditionForwarderList = new List<SkitConditionForwarder>( (int)conditionForwarderCount );
			stream.Position = conditionForwarderOffset;
			for ( uint i = 0; i < conditionForwarderCount; ++i ) {
				var s = new SkitConditionForwarder( stream, endian, bits );
				SkitConditionForwarderList.Add( s );
			}

			SkitConditionList = new List<SkitCondition>( (int)conditionCount );
			stream.Position = conditionOffset;
			for ( uint i = 0; i < conditionCount; ++i ) {
				var s = new SkitCondition( stream, endian );
				SkitConditionList.Add( s );
			}

			UnknownSkitData4List = new List<UnknownSkitData4>( (int)uCount4 );
			stream.Position = uOffset4;
			for ( uint i = 0; i < uCount4; ++i ) {
				var s = new UnknownSkitData4( stream, endian, bits );
				UnknownSkitData4List.Add( s );
			}

			UnknownSkitData5List = new List<UnknownSkitData5>( (int)uCount5 );
			stream.Position = uOffset5;
			for ( uint i = 0; i < uCount5; ++i ) {
				var s = new UnknownSkitData5( stream, endian, bits );
				UnknownSkitData5List.Add( s );
			}

			return true;
		}
	}
}
