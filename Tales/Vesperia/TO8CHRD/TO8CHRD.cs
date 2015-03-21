using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.Tales.Vesperia.TO8CHRD {
	public class TO8CHRD {
		public TO8CHRD( String filename ) {
			using ( Stream stream = new System.IO.FileStream( filename, FileMode.Open ) ) {
				if ( !LoadFile( stream ) ) {
					throw new Exception( "Loading TO8CHRD failed!" );
				}
			}
		}

		public TO8CHRD( Stream stream ) {
			if ( !LoadFile( stream ) ) {
				throw new Exception( "Loading TO8CHRD failed!" );
			}
		}

		public List<CharacterModelDefinition> ModelDefList;
		public List<CustomModelAddition> ModelCustomList;
		public List<OtherModelAddition> ModelOtherList;
		public List<Unknown0x20byteAreaB> U20BList;
		public List<Unknown0x80byteArea> U80List;

		private bool LoadFile( Stream stream ) {
			string magic = stream.ReadAscii( 8 );
			uint filesize = stream.ReadUInt32().SwapEndian();
			uint modelDefStart = stream.ReadUInt32().SwapEndian();
			uint modelDefCount = stream.ReadUInt32().SwapEndian();
			uint refStringStart = stream.ReadUInt32().SwapEndian();
			uint customStart = stream.ReadUInt32().SwapEndian();
			uint customCount = stream.ReadUInt32().SwapEndian();
			uint otherStart = stream.ReadUInt32().SwapEndian();
			uint otherCount = stream.ReadUInt32().SwapEndian();
			uint u20BsectionStart = stream.ReadUInt32().SwapEndian();
			uint u20BsectionCount = stream.ReadUInt32().SwapEndian();
			uint u80sectionStart = stream.ReadUInt32().SwapEndian();
			uint u80sectionCount = stream.ReadUInt32().SwapEndian();
			stream.DiscardBytes( 8 );

			ModelDefList = new List<CharacterModelDefinition>( (int)modelDefCount );
			stream.Position = modelDefStart;
			for ( uint i = 0; i < modelDefCount; ++i ) {
				ModelDefList.Add( new CharacterModelDefinition( stream, refStringStart ) );
			}

			ModelCustomList = new List<CustomModelAddition>( (int)customCount );
			stream.Position = customStart;
			for ( uint i = 0; i < customCount; ++i ) {
				ModelCustomList.Add( new CustomModelAddition( stream, refStringStart ) );
			}

			ModelOtherList = new List<OtherModelAddition>( (int)otherCount );
			stream.Position = otherStart;
			for ( uint i = 0; i < otherCount; ++i ) {
				ModelOtherList.Add( new OtherModelAddition( stream, refStringStart ) );
			}

			U20BList = new List<Unknown0x20byteAreaB>( (int)u20BsectionCount );
			stream.Position = u20BsectionStart;
			for ( uint i = 0; i < u20BsectionCount; ++i ) {
				U20BList.Add( new Unknown0x20byteAreaB( stream, refStringStart ) );
			}

			U80List = new List<Unknown0x80byteArea>( (int)u80sectionCount );
			stream.Position = u80sectionStart;
			for ( uint i = 0; i < u80sectionCount; ++i ) {
				U80List.Add( new Unknown0x80byteArea( stream, refStringStart ) );
			}

			foreach ( var model in ModelDefList ) {
				// this isn't right but I'm not sure what the actual logic here is and this gives a decently close result
				if ( model.CustomIndex > 0 ) {
					model.Custom = ModelCustomList[(int)model.CustomIndex - 1];
				}
				if ( model.OtherIndex > 0 ) {
					model.Other = ModelOtherList[(int)model.OtherIndex - 1];
				}
				if ( model.U20BIndex > 0 ) {
					model.Unknown0x20Area = U20BList[(int)model.U20BIndex - 1];
				}
				if ( model.U80Index > 0 ) {
					model.Unknown0x80Area = U80List[(int)model.U80Index - 1];
				}
			}

			return true;
		}
	}
}
