using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Pokemon.Gen5.PWT {
	public class Tournament {
		public enum LevelScalingStyle : ushort {
			NoChange = 0,
			RejectLower = 1,
			RejectHigher = 2,
			ScaleDownToEnemy = 3,
			ScaleToEnemy = 4,
			ScaleEnemyUp = 5,
		}

		public Tournament( byte[] data, int offset ) {
			if ( !Load( data, offset ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}
		public Tournament( string filename ) {
			if ( !Load( System.IO.File.ReadAllBytes( filename ), 0 ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}

		public byte[] Data;

		public byte Versions { get { return Data[0x00]; } }	// 0 == W2, 1 == B2, 2 == both
		public byte Unknown0x01 { get { return Data[0x01]; } }
		public byte Unknown0x02 { get { return Data[0x02]; } }
		public byte Unknown0x03 { get { return Data[0x03]; } }
		public byte Unknown0x04 { get { return Data[0x04]; } }
		public byte Unknown0x05 { get { return Data[0x05]; } }
		public byte Unknown0x06 { get { return Data[0x06]; } }
		public byte Language { get { return Data[0x07]; } set { Data[0x07] = value; } }
		public ushort ID { get { return BitConverter.ToUInt16( Data, 0x08 ); } }

		public string Name { get { return Util.GetTextUnicode( Data, 0x0A, 0x4A ); } }
		public string Description { get { return Util.GetTextUnicode( Data, 0x54, 0x96 ); } }
		public string Rules { get { return Util.GetTextUnicode( Data, 0xEA, 0xDE ); } }

		public byte PokemonCountMin { get { return Data[0x1C8]; } }
		public byte PokemonCountMax { get { return Data[0x1C9]; } }
		public byte Level { get { return Data[0x1CA]; } }
		public LevelScalingStyle LevelStyle { get { return (LevelScalingStyle)Data[0x1CB]; } }
		public ushort LevelTotal { get { return BitConverter.ToUInt16( Data, 0x1CC ); } }
		public byte AllowDuplicatePokemon { get { return Data[0x1CE]; } }
		public byte AllowDuplicateItems { get { return Data[0x1CF]; } }
		// banned pokemon 0x1D0 - 0x221
		// banned items   0x222 - 0x26D
		public byte BattleStyle { get { return Data[0x26F]; } }
		public byte MusicModifier { get { return Data[0x270]; } }
		public byte AllowedPokemonType { get { return Data[0x271]; } }
		public ushort BannedMove { get { return BitConverter.ToUInt16( Data, 0x272 ); } }


		private bool Load( byte[] data, int offset ) {
			Data = new byte[4628];
			Util.CopyByteArrayPart( data, offset, this.Data, 0, this.Data.Length );

			return true;
		}

		public void RecalculateChecksum() {
			ushort checksum = Checksums.Ccitt.Update( Checksums.Ccitt.Init(), Data, 4624 );
			Util.CopyByteArrayPart( BitConverter.GetBytes( checksum ), 0, Data, 4624, 2 );
		}
	}
}
