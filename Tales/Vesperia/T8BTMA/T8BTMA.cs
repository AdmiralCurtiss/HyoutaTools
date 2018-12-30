using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Tales.Vesperia.T8BTMA {
	public class T8BTMA {
		public T8BTMA( string filename, Util.Endianness endian ) {
			using ( System.IO.Stream stream = new System.IO.FileStream( filename, System.IO.FileMode.Open ) ) {
				if ( !LoadFile( stream, endian ) ) {
					throw new Exception( "Loading T8BTMA failed!" );
				}
			}
		}

		public T8BTMA( System.IO.Stream stream, Util.Endianness endian ) {
			if ( !LoadFile( stream, endian ) ) {
				throw new Exception( "Loading T8BTMA failed!" );
			}
		}

		public List<Arte> ArteList;
		public Dictionary<uint, Arte> ArteIdDict;

		private bool LoadFile( System.IO.Stream stream, Util.Endianness endian ) {
			string magic = stream.ReadAscii( 8 );
			if ( magic != "T8BTMA  " ) {
				return false;
			}

			uint arteCount = stream.ReadUInt32().FromEndian( endian );
			uint stringStart = stream.ReadUInt32().FromEndian( endian );

			ArteList = new List<Arte>( (int)arteCount );
			for ( uint i = 0; i < arteCount; ++i ) {
				Arte a = new Arte( stream, stringStart, endian );
				ArteList.Add( a );
			}

			ArteIdDict = new Dictionary<uint, Arte>( ArteList.Count );
			foreach ( var arte in ArteList ) {
				ArteIdDict.Add( arte.InGameID, arte );
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
				string UpdateNames = "UPDATE Text SET IdentifyString = \"" + a.Type.ToString() + ";\" || IdentifyString WHERE IdentifyString LIKE \"%[" + a.NameStringDicId + " / 0x" + a.NameStringDicId.ToString( "X6" ) + "]\"";
				Console.WriteLine( UpdateNames );
				command.CommandText = UpdateNames;
				command.ExecuteNonQuery();
				string UpdateDescs = "UPDATE Text SET IdentifyString = \"Description;\" || IdentifyString WHERE IdentifyString LIKE \"%[" + a.DescStringDicId + " / 0x" + a.DescStringDicId.ToString( "X6" ) + "]\"";
				Console.WriteLine( UpdateDescs );
				command.CommandText = UpdateDescs;
				command.ExecuteNonQuery();

				if ( a.Type == Arte.ArteType.Generic ) {
					string UpdateStatus = "UPDATE Text SET status = 4, updated = 1, updatedby = \"[HyoutaTools]\", updatedtimestamp = " + Util.DateTimeToUnixTime( DateTime.UtcNow ) + " WHERE IdentifyString LIKE \"%[" + a.NameStringDicId + " / 0x" + a.NameStringDicId.ToString( "X6" ) + "]\"";
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
