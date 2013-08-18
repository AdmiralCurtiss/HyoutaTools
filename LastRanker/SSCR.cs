using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.LastRanker {
	class SscrName {
		public string Id; // 8
		public uint Unknown;
		public string Name; // 0x20
		public uint Unknown2;
		public uint Unknown3;
	}
	class SscrSystemTerm {
		public string Term;	// 0x16
		public byte Unknown;
	}
	class SscrSomething {
		public string Text; // 0x20
		public ushort Unknown1;
		public ushort Unknown2;
		public string Id; // 0x03
		public byte Unknown3;
		public ushort Unknown4;
	}

	class SSCR {
		public SSCR( String filename ) {
			if ( !LoadFile( System.IO.File.ReadAllBytes( filename ) ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}
		public SSCR( byte[] Bytes ) {
			if ( !LoadFile( Bytes ) ) {
				throw new Exception( "bscr: Load Failed!" );
			}
		}

		public byte[] File;
		public List<SscrName> Names;
		public List<SscrSystemTerm> SystemTerms;
		public List<SscrSomething> Somethings;

		private bool LoadFile( byte[] File ) {
			this.File = File;

			// not a full load, just the parts we need to change
			uint NameCountLocation = BitConverter.ToUInt32( File, 0x0C );
			uint NameCount = BitConverter.ToUInt16( File, (int)NameCountLocation );
			uint NameBlockLocation = BitConverter.ToUInt32( File, 0x14 );
			Names = new List<SscrName>( (int)NameCount );
			for ( uint i = 0; i < NameCount; ++i ) {
				int loc = (int)( NameBlockLocation + i * 0x34 );
				SscrName n = new SscrName();
				n.Id = Encoding.ASCII.GetString( File, loc, 0x08 ).TrimEnd( '\0' );
				n.Unknown = BitConverter.ToUInt32( File, loc + 0x08 );
				n.Name = Encoding.UTF8.GetString( File, loc + 0x0C, 0x20 ).TrimEnd( '\0' );
				n.Unknown2 = BitConverter.ToUInt32( File, loc + 0x2C );
				n.Unknown3 = BitConverter.ToUInt32( File, loc + 0x30 );
				Names.Add( n );
			}

			uint SystemTermBlockLocation = BitConverter.ToUInt32( File, 0x34 );
			uint SystemTermCount = BitConverter.ToUInt16( File, (int)NameCountLocation + 6 );
			SystemTerms = new List<SscrSystemTerm>( (int)SystemTermCount );
			for ( uint i = 0; i < SystemTermCount; ++i ) {
				int loc = (int)( SystemTermBlockLocation + i * 0x17 );
				SscrSystemTerm st = new SscrSystemTerm();
				st.Term = Encoding.UTF8.GetString( File, loc, 0x16 ).TrimEnd( '\0' );
				st.Unknown = File[loc + 0x16];
				SystemTerms.Add( st );
			}

			uint Something2BlockLocation = BitConverter.ToUInt32( File, 0x3C );
			uint Something2Count = BitConverter.ToUInt16( File, (int)NameCountLocation + 8 );
			Somethings = new List<SscrSomething>( (int)Something2Count );
			for ( uint i = 0; i < Something2Count; ++i ) {
				int loc = (int)( Something2BlockLocation + i * 0x2A );
				SscrSomething s = new SscrSomething();
				s.Text = Encoding.UTF8.GetString( File, loc, 0x20 ).TrimEnd( '\0' );
				s.Unknown1 = BitConverter.ToUInt16( File, loc + 0x20 );
				s.Unknown2 = BitConverter.ToUInt16( File, loc + 0x22 );
				s.Id = Encoding.ASCII.GetString( File, loc + 0x24, 3 ).TrimEnd( '\0' );
				s.Unknown3 = File[loc + 0x27];
				s.Unknown4 = BitConverter.ToUInt16( File, loc + 0x28 );
				Somethings.Add( s );
			}

			return true;
		}
	}
}
