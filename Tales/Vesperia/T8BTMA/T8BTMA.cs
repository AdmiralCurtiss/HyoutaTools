using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	public class T8BTMA {
		public T8BTMA( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "Loading T8BTMA failed!" );
			}
		}

		public T8BTMA( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "Loading T8BTMA failed!" );
			}
		}

		public List<Arte> ArteList;

		private bool LoadFile( byte[] Bytes ) {
			uint arteCount = BitConverter.ToUInt32( Bytes, 0x8 ).SwapEndian();
			uint stringStart = BitConverter.ToUInt32( Bytes, 0xC ).SwapEndian();

			uint location = 0x10;
			ArteList = new List<Arte>( (int)arteCount );
			for ( uint i = 0; i < arteCount; ++i ) {
				uint entrySize = BitConverter.ToUInt32( Bytes, (int)location ).SwapEndian();
				Arte a = new Arte( Bytes, location, entrySize, stringStart );
				ArteList.Add( a );
				location += entrySize;
			}

			return true;
		}

		public void UpdateDatabaseWithArteProps( string ConnectionString ) {
			System.Data.SQLite.SQLiteConnection conn = new System.Data.SQLite.SQLiteConnection( ConnectionString );
			conn.Open();
			System.Data.SQLite.SQLiteTransaction transaction = conn.BeginTransaction();
			System.Data.SQLite.SQLiteCommand command = new System.Data.SQLite.SQLiteCommand( conn );
			command.Transaction = transaction;
			foreach ( var a in ArteList ) {
				string UpdateNames = "UPDATE Text SET IdentifyString = \"" + a.Type.ToString() + ";\" || IdentifyString WHERE IdentifyString LIKE \"%[" + a.StringIdName + " / 0x" + a.StringIdName.ToString( "X6" ) + "]\"";
				Console.WriteLine( UpdateNames );
				command.CommandText = UpdateNames;
				command.ExecuteNonQuery();
				string UpdateDescs = "UPDATE Text SET IdentifyString = \"Description;\" || IdentifyString WHERE IdentifyString LIKE \"%[" + a.StringIdDescription + " / 0x" + a.StringIdDescription.ToString( "X6" ) + "]\"";
				Console.WriteLine( UpdateDescs );
				command.CommandText = UpdateDescs;
				command.ExecuteNonQuery();

				if ( a.Type == Arte.ArteType.Generic ) {
					string UpdateStatus = "UPDATE Text SET status = 4, updated = 1, updatedby = \"[HyoutaTools]\", updatedtimestamp = " + Util.DateTimeToUnixTime( DateTime.UtcNow ) + " WHERE IdentifyString LIKE \"%[" + a.StringIdName + " / 0x" + a.StringIdName.ToString( "X6" ) + "]\"";
					Console.WriteLine( UpdateStatus );
					command.CommandText = UpdateStatus;
					command.ExecuteNonQuery();
				}
			}
			command.Dispose();
			transaction.Commit();
			conn.Close();
			conn.Dispose();
		}
	}
}
