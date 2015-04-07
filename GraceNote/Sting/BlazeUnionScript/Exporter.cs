using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HyoutaTools.GraceNote.Sting.BlazeUnionScript {
	public class Exporter {
		public static int Export( List<string> args ) {
			if ( args.Count < 3 ) {
				Console.WriteLine( "Usage: OriginalScript DB GJ NewScript" );
				return -1;
			}
			return Export( args[0], args[1], args[2], args[3] ) ? 0 : -1;
		}

		public static bool Export( string originalScriptFilename, string databaseFilename, string gracesJapaneseFilename, string newScriptFilename ) {
			byte[] script = System.IO.File.ReadAllBytes( originalScriptFilename );
			uint sectionPointerLocation = BitConverter.ToUInt32( script, 0 );

			// this is SUPER HACKY but should work
			using ( var stream = new System.IO.FileStream( newScriptFilename, System.IO.FileMode.Create ) ) {
				var entries = GraceNoteDatabaseEntry.GetAllEntriesFromDatabase( "Data Source=" + databaseFilename, "Data Source=" + gracesJapaneseFilename );

				// copy whole flie except for the section pointers
				for ( int i = 0; i < sectionPointerLocation; ++i ) {
					stream.WriteByte( script[i] );
				}

				long pos = stream.Position;
				// remove original strings from the file
				foreach ( var entry in entries ) {
					stream.Position = entry.PointerRef;
					RemoveString( stream, stream.ReadUInt32() );
				}
				stream.Position = pos;

				// now write the modified strings from the GN db at the end of the file
				foreach ( var entry in entries ) {
					uint stringLocation = Convert.ToUInt32( stream.Position );
					stream.Position = entry.PointerRef;
					stream.WriteUInt32( stringLocation );
					stream.Position = stringLocation;
					stream.Write( StringToBytesBlazeUnion( entry.TextEN ) );
					stream.WriteByte( 0 );
				}

				// write the section pointers and replace position
				stream.Position = stream.Position.Align( 4 );
				uint newSectionPointerLocation = Convert.ToUInt32( stream.Position );
				for ( uint i = sectionPointerLocation; i < script.Length; ++i ) {
					stream.WriteByte( script[i] );
				}
				stream.Position = 0;
				stream.WriteUInt32( newSectionPointerLocation );
			}

			return true;
		}

		public static byte[] StringToBytesBlazeUnion( string s ) {
			List<byte> data = new List<byte>();
			for ( int i = 0; i < s.Length; ++i ) {
				char c = s[i];
				if ( c == '<' && s.Substring( i, "<Voice:".Length ) == "<Voice:" ) {
					StringBuilder sb = new StringBuilder();
					i += "<Voice:".Length;
					while ( s[i] != '>' ) {
						sb.Append( s[i++] );
					}
					data.Add( 0x02 );
					data.Add( 0x00 );
					data.AddRange( Util.HexStringToByteArray( sb.ToString().Trim() ).Reverse() );
				} else {
					data.AddRange( Util.ShiftJISEncoding.GetBytes( c.ToString() ) );
				}
			}
			return data.ToArray();
		}

		public static void RemoveString( Stream stream, long position ) {
			stream.Position = position;

			byte b = stream.PeekByte();
			while ( b != 0x00 ) {
				if ( b == 0x02 ) {
					stream.WriteUInt32( 0 );
				} else {
					stream.WriteByte( 0x00 );
				}
				b = stream.PeekByte();
			}
		}
	}
}
