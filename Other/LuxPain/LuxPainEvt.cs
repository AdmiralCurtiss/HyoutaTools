using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.Other.LuxPain
{
	public class LuxPainEvtHeader {
		public UInt32 Magic;
		public UInt32 Unknown1;
		public UInt32 Unknown2;
		public UInt32 Unknown3;
		public UInt32 TextOffsetsLocation;
		public UInt32 TextLocation;
	}

	public class LuxPainEvtText : IComparable {
		public UInt32 OffsetLocation;
		public String Text;

		public UInt16 Offset;

		internal void FormatForEditing() {
			Text = LuxPainUtil.FormatForEditing( Text );
			return;
		}

		internal void FormatForGameInserting() {
			for ( int i = 0; i < 26; ++i ) {
				Text = Text.Replace( (char)( 'A' + i ), (char)( 'Ａ' + i ) );
				Text = Text.Replace( (char)( 'a' + i ), (char)( 'ａ' + i ) );
			}
			for ( int i = 0; i < 10; ++i ) {
				Text = Text.Replace( (char)( '0' + i ), (char)( '０' + i ) );
			}

			Text = Text.Replace( ' ', '　' );
			Text = Text.Replace( '!', '！' );
			Text = Text.Replace( '.', '．' );
			Text = Text.Replace( '?', '？' );
			Text = Text.Replace( '\'', '’' );
			Text = Text.Replace( ',', '，' );
			Text = Text.Replace( '"', '”' );
			Text = Text.Replace( '-', '‐' );
			Text = Text.Replace( ':', '：' );
			Text = Text.Replace( ';', '；' );
			Text = Text.Replace( '(', '（' );
			Text = Text.Replace( ')', '）' );
			Text = Text.Replace( "\n", "<ff00>" );

			return;
		}

		public override string ToString() {
			return Text;
		}

		public int CompareTo( object obj ) {
			return this.Offset - ( (LuxPainEvtText)obj ).Offset;
		}
	}

	public class LuxPainEvt {
		public LuxPainEvtHeader Header;
		public List<LuxPainEvtText> TextEntries;

		private byte[] originalFile;

		public LuxPainEvt( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "LuxPainEvt: Load Failed!" );
			}
		}

		public LuxPainEvt( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "LuxPainEvt: Load Failed!" );
			}
		}

		private bool LoadFile( byte[] Bytes ) {
			originalFile = Bytes;

			Header = new LuxPainEvtHeader();
			Header.Magic = BitConverter.ToUInt32( Bytes, 0x00 );
			if ( Header.Magic != 0x31304353 ) return false;
			Header.Unknown1 = BitConverter.ToUInt32( Bytes, 0x04 );
			Header.Unknown2 = BitConverter.ToUInt32( Bytes, 0x08 );
			Header.Unknown3 = BitConverter.ToUInt32( Bytes, 0x0C );
			Header.TextOffsetsLocation = BitConverter.ToUInt32( Bytes, 0x10 );
			Header.TextLocation = BitConverter.ToUInt32( Bytes, 0x14 );

			UInt32 PredictedTextAmount = ( Header.TextLocation - Header.TextOffsetsLocation ) / 2;
			TextEntries = new List<LuxPainEvtText>( (int)PredictedTextAmount );

			for ( UInt32 loc = Header.TextOffsetsLocation; loc < Header.TextLocation; loc += 2 ) {
				LuxPainEvtText txt = new LuxPainEvtText();
				txt.OffsetLocation = loc;
				txt.Offset = BitConverter.ToUInt16( Bytes, (int)loc );

				txt.Text = LuxPainUtil.GetTextLuxPain( (int)( Header.TextLocation + ( txt.Offset * 2 ) ), Bytes );

				TextEntries.Add( txt );
			}

			//foreach (LuxPainEvtText t in TextEntries)
			//{
			//    Console.WriteLine(t.Text);
			//}

			return true;
		}

		public void FormatTextForEditing() {
			foreach ( LuxPainEvtText t in TextEntries ) {
				t.FormatForEditing();
			}
		}

		public void FormatTextForGameInserting() {
			foreach ( LuxPainEvtText t in TextEntries ) {
				t.FormatForGameInserting();
			}
		}



		public void GetSQL( String ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			List<LuxPainEvtText> OriginalTextEntries = TextEntries;
			TextEntries = new List<LuxPainEvtText>();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef FROM Text WHERE PointerRef != 2147483647 ORDER BY ID";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = "";
					}

					int PointerRef = r.GetInt32( 1 );


					LuxPainEvtText e = new LuxPainEvtText();
					e.OffsetLocation = (uint)PointerRef;
					e.Text = SQLText;
					TextEntries.Add( e );
				}

				Transaction.Rollback();
			}

			if ( OriginalTextEntries.Count != TextEntries.Count ) {
				throw new Exception( "Entry count mismatch when reading from database!" );
			}

			for ( int i = 0; i < OriginalTextEntries.Count; ++i ) {
				TextEntries[i].Offset = OriginalTextEntries[i].Offset;
			}

			return;
		}


		internal byte[] CreateTextBlock( int Size ) {
			UInt32 TextLocationRelative = Header.TextLocation - Header.TextOffsetsLocation;

			Byte[] bytes = new Byte[Size];

			TextEntries.Sort();

			UInt16 CurrentTextOffset = 0;
			foreach ( LuxPainEvtText t in TextEntries ) {
				byte[] CurrentTextOffsetBytes = BitConverter.GetBytes( (UInt16)( CurrentTextOffset ) );
				CurrentTextOffsetBytes.CopyTo( bytes, t.OffsetLocation - Header.TextOffsetsLocation );

				byte[] text = LuxPainUtil.BackConvertLuxPainText( t.Text );
				text.CopyTo( bytes, TextLocationRelative );

				TextLocationRelative += (uint)text.Length;
				CurrentTextOffset += (ushort)( text.Length / 2 );
			}

			return bytes.Take( (int)TextLocationRelative ).ToArray();
		}
	}
}
