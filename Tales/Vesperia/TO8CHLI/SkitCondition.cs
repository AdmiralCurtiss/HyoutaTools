using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace HyoutaTools.Tales.Vesperia.TO8CHLI {
	public enum SkitConditionType : ushort {
		EventFlag = 1,
		SeenSkit = 2,
		TotalPlaytime = 9,
		TotalEnemyEncounters = 13,
		PartySlot1 = 16,
		ArteUsage = 17,
		BattleLength = 18,
		BattleParticipation = 19,
		BattleParticipationPercent = 20,
		CookingCharacterBitmask = 30,
		CookingSkill = 31,
		NotCookedTime = 32,
		TitleEquipped = 55,
		GigantosKilled = 56,
		AllGigantosKilled = 57,
		NotAirshipTime = 64,
		OnShip = 65,
		InArea = 74,
		TimeSpentInArea = 76,
		AttachmentEquipped = 82,
		AllPartyMemberLevels = 90,
		TotalSearchPointCollected = 96,
		NewGamePlus = 98,
	}


	public class SkitCondition {
		// what this condition checks for
		public SkitConditionType Type;
		// the type of comparison this condition performs, i.e. 4 <=, 5 >=
		public ushort MathOp;
		public int Value1;
		public int Value2;
		public int Value3;

		private string MathOpString {
			get {
				switch ( MathOp ) {
					case 4: return "<=";
					case 5: return ">=";
				}
				throw new Exception( "Unsupported Math Op." );
			}
		}

		public SkitCondition( System.IO.Stream stream ) {
			Type = (SkitConditionType)stream.ReadUInt16().SwapEndian();
			MathOp = stream.ReadUInt16().SwapEndian();
			Value1 = (int)stream.ReadUInt32().SwapEndian();
			Value2 = (int)stream.ReadUInt32().SwapEndian();
			Value3 = (int)stream.ReadUInt32().SwapEndian();
		}

		public void GetDataAsHtml( StringBuilder sb ) {
			switch ( Type ) {
				case SkitConditionType.AllPartyMemberLevels:
					sb.AppendFormat( "All Party Member Levels {0} {1}", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.TotalEnemyEncounters:
					sb.AppendFormat( "Total Enemy Encounters {0} {1}", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.EventFlag:
					sb.AppendFormat( "Event Flag {0} {1}", Value1, MathOp == 0 ? "on" : "off" );
					Util.Assert( ( MathOp == 0 || MathOp == 1 ) && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.SeenSkit:
					sb.AppendFormat( "Have seen other skit ({0} ????)", Value1 );
					Util.Assert( MathOp == 0 && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.OnShip:
					sb.Append( "Currently on Ship" );
					Util.Assert( MathOp == 0 && Value1 == 0 && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.InArea:
					sb.AppendFormat( "Currently in area {0}", Value1 );
					Util.Assert( MathOp == 0 && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.TimeSpentInArea:
					sb.AppendFormat( "Spent {0} {2} frames in area {1}", MathOpString, Value1, Value2 );
					Util.Assert( Value3 == 0 );
					return;
				case SkitConditionType.GigantosKilled:
					sb.AppendFormat( "Defeated {0} {1} Giganto Monsters", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.AllGigantosKilled:
					sb.Append( "Defeated all Giganto Monsters" );
					Util.Assert( MathOp == 0 && Value1 == 0 && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.ArteUsage:
					sb.AppendFormat( "Character {1} used Arte {2} {0} {3} times", MathOpString, Value1, Value2, Value3 );
					return;
				case SkitConditionType.TotalSearchPointCollected:
					sb.AppendFormat( "Collected {0} {1} Items from Search Points", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.BattleLength:
					sb.AppendFormat( "(last? shortest?) Battle lasted {0} {1} frames", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.BattleParticipation:
					sb.AppendFormat( "Character {1} has been in {0} {2} battles", MathOpString, Value1, Value2 );
					Util.Assert( Value3 == 0 );
					return;
				case SkitConditionType.BattleParticipationPercent:
					sb.AppendFormat( "Character {1} has been in {0} {2}% of battles", MathOpString, Value1, Value2 );
					Util.Assert( Value3 == 0 );
					return;
				case SkitConditionType.CookingSkill:
					sb.AppendFormat( "Character {1}'s cooking skill for recipe {2} is {0} {3}", MathOpString, Value1, Value2, Value3 );
					return;
				case SkitConditionType.CookingCharacterBitmask:
					sb.Append( "Characters " );
					Website.GenerateWebsite.AppendCharacterBitfieldAsImageString( sb, GameVersion.PS3, (uint)Value1 );
					sb.AppendFormat( " have cooked {0} {1} times", MathOpString, Value2 );
					Util.Assert( Value3 == 0 );
					return;
				case SkitConditionType.NotCookedTime:
					sb.AppendFormat( "{0} {1} frames since last cooking", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.NotAirshipTime:
					sb.AppendFormat( "{0} {1} frames since last time flying on Ba'ul", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.TotalPlaytime:
					sb.AppendFormat( "{0} {1} frames total playtime", MathOpString, Value1 );
					Util.Assert( Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.NewGamePlus:
					sb.Append( "In New Game Plus" );
					Util.Assert( MathOp == 0 && Value1 == 0 && Value2 == 0 && Value3 == 0 );
					return;
				case SkitConditionType.TitleEquipped:
					sb.AppendFormat( "Character {0} has title {1} equipped", Value1, Value2 );
					Util.Assert( MathOp == 0 && Value3 == 0 );
					return;
				case SkitConditionType.AttachmentEquipped:
					sb.AppendFormat( "Character {0} has attachment {1} equipped", Value1, Value2 );
					Util.Assert( MathOp == 0 && Value3 == 0 );
					return;
				case SkitConditionType.PartySlot1:
					sb.AppendFormat( "Character {0} in 1st party slot", Value1 );
					Util.Assert( MathOp == 0 && Value2 == 0 && Value3 == 0 );
					return;
			}

			sb.Append( " ========== #type: " + (ushort)Type );
			sb.Append( " / #math: " + MathOp );
			sb.Append( " / #val1: " + Value1 );
			sb.Append( " / #val2: " + Value2 );
			sb.Append( " / #val3: " + Value3 );
		}
	}
}
