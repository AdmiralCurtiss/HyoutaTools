using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class TO8CHRD {
		public TO8CHRD( String filename, Util.Endianness endian ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open, System.IO.FileAccess.Read ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading TO8CHRD failed!" );
				}
			}
		}

		public TO8CHRD( Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading TO8CHRD failed!" );
			}
		}

		public List<CharacterModelDefinition> ModelDefList;
		public List<CustomModelAddition> ModelCustomList;
		public List<OtherModelAddition> ModelOtherList;
		public List<Unknown0x20byteAreaB> U20BList;
		public List<Unknown0x80byteArea> U80List;

		private bool LoadFile( Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "TO8CHRD\0" ) {
				throw new Exception( "Invalid magic." );
			}
			uint filesize = stream.ReadUInt32().FromEndian( endian );
			uint modelDefStart = stream.ReadUInt32().FromEndian( endian );
			uint modelDefCount = stream.ReadUInt32().FromEndian( endian );
			uint refStringStart = stream.ReadUInt32().FromEndian( endian );
			uint customStart = stream.ReadUInt32().FromEndian( endian );
			uint customCount = stream.ReadUInt32().FromEndian( endian );
			uint otherStart = stream.ReadUInt32().FromEndian( endian );
			uint otherCount = stream.ReadUInt32().FromEndian( endian );
			uint u20BsectionStart = stream.ReadUInt32().FromEndian( endian );
			uint u20BsectionCount = stream.ReadUInt32().FromEndian( endian );
			uint u80sectionStart = stream.ReadUInt32().FromEndian( endian );
			uint u80sectionCount = stream.ReadUInt32().FromEndian( endian );
			stream.DiscardBytes( 8 );

			ModelDefList = new List<CharacterModelDefinition>( (int)modelDefCount );
			stream.Position = modelDefStart;
			for ( uint i = 0; i < modelDefCount; ++i ) {
				ModelDefList.Add( new CharacterModelDefinition( stream, refStringStart, endian ) );
			}

			ModelCustomList = new List<CustomModelAddition>( (int)customCount );
			stream.Position = customStart;
			for ( uint i = 0; i < customCount; ++i ) {
				ModelCustomList.Add( new CustomModelAddition( stream, refStringStart, endian ) );
			}

			ModelOtherList = new List<OtherModelAddition>( (int)otherCount );
			stream.Position = otherStart;
			for ( uint i = 0; i < otherCount; ++i ) {
				ModelOtherList.Add( new OtherModelAddition( stream, refStringStart, endian ) );
			}

			U20BList = new List<Unknown0x20byteAreaB>( (int)u20BsectionCount );
			stream.Position = u20BsectionStart;
			for ( uint i = 0; i < u20BsectionCount; ++i ) {
				U20BList.Add( new Unknown0x20byteAreaB( stream, refStringStart, endian ) );
			}

			U80List = new List<Unknown0x80byteArea>( (int)u80sectionCount );
			stream.Position = u80sectionStart;
			for ( uint i = 0; i < u80sectionCount; ++i ) {
				U80List.Add( new Unknown0x80byteArea( stream, refStringStart, endian ) );
			}

			foreach ( var model in ModelDefList ) {
				model.Custom = new CustomModelAddition[model.CustomCount];
				for ( int i = 0; i < model.Custom.Length; ++i ) {
					model.Custom[i] = ModelCustomList[(int)model.CustomIndex + i];
				}
				model.Other = new OtherModelAddition[model.OtherCount];
				for ( int i = 0; i < model.Other.Length; ++i ) {
					model.Other[i] = ModelOtherList[(int)model.OtherIndex + i];
				}
				model.Unknown0x20Area = new Unknown0x20byteAreaB[model.Unknown0x20AreaCount];
				for ( int i = 0; i < model.Unknown0x20Area.Length; ++i ) {
					model.Unknown0x20Area[i] = U20BList[(int)model.Unknown0x20AreaIndex + i];
				}
				model.Unknown0x80Area = new Unknown0x80byteArea[model.Unknown0x80AreaCount];
				for ( int i = 0; i < model.Unknown0x80Area.Length; ++i ) {
					model.Unknown0x80Area[i] = U80List[(int)model.Unknown0x80AreaIndex + i];
				}
			}

			return true;
		}
	}
}
