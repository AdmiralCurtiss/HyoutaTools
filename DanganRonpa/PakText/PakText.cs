using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SQLite;

namespace HyoutaTools.DanganRonpa.PakText {
	struct PakTextEntry {
		public int Offset;
		public int OffsetLocation;
		public String Text;

		public override string ToString() {
			return Text;
		}
	}

	class PakText {
		public List<PakTextEntry> TextList;
		public int Offset = 0;

		public PakText() { }

		public PakText( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "DRMenuFile: Load Failed!" );
			}
		}

		public PakText( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "DRMenuFile: Load Failed!" );
			}
		}

		public byte[] CreateFile() {
			List<Byte> Bytes = new List<byte>();

			int TextAmount = TextList.Count;

			Bytes.AddRange( BitConverter.GetBytes( TextAmount ) );

			int[] startpoints = new int[TextAmount];
			int total = 8 + TextAmount * 4;
			for ( int i = 0; i < TextAmount; ++i ) {
				if ( ( i * 4 + 4 ) != TextList[i].OffsetLocation ) {
					Console.WriteLine( "Sanity Check failed at Entry " + i.ToString() + "!" );
					return null;
				}

				String Text = TextList[i].Text;
				if ( !Text.EndsWith( "\0" ) ) {
					Text = Text + '\0';
				}

				Byte[] bytetext = Encoding.Unicode.GetBytes( Text );
				if ( bytetext[0] != 0xFF || bytetext[1] != 0xFE ) {
					Byte[] tmp = new byte[bytetext.Length + 2];
					tmp[0] = 0xFF;
					tmp[1] = 0xFE;

					bytetext.CopyTo( tmp, 2 );

					bytetext = tmp;
				}
				startpoints[i] = total;
				total += bytetext.Length;
			}

			foreach ( int s in startpoints ) {
				Bytes.AddRange( BitConverter.GetBytes( s ) );
			}
			Bytes.AddRange( BitConverter.GetBytes( total ) );

			for ( int i = 0; i < TextAmount; ++i ) {
				String Text = TextList[i].Text;
				if ( !Text.EndsWith( "\0" ) ) {
					Text = Text + '\0';
				}

				Byte[] bytetext = Encoding.Unicode.GetBytes( Text );
				if ( bytetext[0] != 0xFF || bytetext[1] != 0xFE ) {
					Byte[] tmp = new byte[bytetext.Length + 2];
					tmp[0] = 0xFF;
					tmp[1] = 0xFE;

					bytetext.CopyTo( tmp, 2 );

					bytetext = tmp;
				}
				Bytes.AddRange( bytetext );
			}

			while ( Bytes.Count % 1024 != 0 ) {
				Bytes.Add( 0x00 );
			}

			return Bytes.ToArray();
		}

		public void GetSQL( String ConnectionString ) {
			SQLiteConnection Connection = new SQLiteConnection( ConnectionString );
			Connection.Open();

			TextList = new List<PakTextEntry>();

			using ( SQLiteTransaction Transaction = Connection.BeginTransaction() )
			using ( SQLiteCommand Command = new SQLiteCommand( Connection ) ) {
				Command.CommandText = "SELECT english, PointerRef FROM Text ORDER BY PointerRef";
				SQLiteDataReader r = Command.ExecuteReader();
				while ( r.Read() ) {
					String SQLText;

					try {
						SQLText = r.GetString( 0 ).Replace( "''", "'" );
					} catch ( System.InvalidCastException ) {
						SQLText = "";
					}

					int PointerRef = r.GetInt32( 1 );


					PakTextEntry d = new PakTextEntry();
					d.OffsetLocation = PointerRef;
					d.Text = SQLText;
					d.Offset = -1;
					TextList.Add( d );
				}

				Transaction.Rollback();
			}
			return;
		}


		private bool LoadFile( byte[] Bytes ) {
			int TextAmount = BitConverter.ToInt32( Bytes, 0 );

			if ( TextAmount == 0 ) {
				Console.WriteLine( "No text found!" );
				return false;
			}

			TextList = new List<PakTextEntry>( TextAmount );

			// sanity check: After EOF no further bytes
			int ProjectedEnd = BitConverter.ToInt32( Bytes, ( TextAmount + 1 ) * 4 );
			//ProjectedEnd = Util.AlignToByteBoundary(ProjectedEnd, 4);
			for ( int i = ProjectedEnd; i < Bytes.Length; ++i ) {
				if ( Bytes[i] != 0x00 ) {
					Console.WriteLine( "Found bytes after calculated EOF!" );
					return false;
				}
			}

			for ( int i = 1; i <= TextAmount; i++ ) {
				PakTextEntry e = new PakTextEntry();

				e.OffsetLocation = i * 4;
				e.Offset = BitConverter.ToInt32( Bytes, e.OffsetLocation );
				int NextOffset = BitConverter.ToInt32( Bytes, e.OffsetLocation + 4 );

				e.Text = Encoding.Unicode.GetString( Bytes, e.Offset, NextOffset - e.Offset );
				TextList.Add( e );
			}


			return true;
		}
	}
}
