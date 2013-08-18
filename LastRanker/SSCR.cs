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

		uint NameBlockLocation;
		uint SystemTermBlockLocation;
		uint Something2BlockLocation;

		private bool LoadFile( byte[] File ) {
			this.File = File;

			// not a full load, just the parts we need to change
			uint NameCountLocation = BitConverter.ToUInt32( File, 0x0C );
			uint NameCount = BitConverter.ToUInt16( File, (int)NameCountLocation );
			NameBlockLocation = BitConverter.ToUInt32( File, 0x14 );
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

			SystemTermBlockLocation = BitConverter.ToUInt32( File, 0x34 );
			uint SystemTermCount = BitConverter.ToUInt16( File, (int)NameCountLocation + 6 );
			SystemTerms = new List<SscrSystemTerm>( (int)SystemTermCount );
			for ( uint i = 0; i < SystemTermCount; ++i ) {
				int loc = (int)( SystemTermBlockLocation + i * 0x17 );
				SscrSystemTerm st = new SscrSystemTerm();
				st.Term = Encoding.UTF8.GetString( File, loc, 0x16 ).TrimEnd( '\0' );
				st.Unknown = File[loc + 0x16];
				SystemTerms.Add( st );
			}

			Something2BlockLocation = BitConverter.ToUInt32( File, 0x3C );
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

		public void CreateFile( string OutFile ) {
			// very hacky, will break if someone changes the count of anything

			for ( int i = 0; i < Names.Count; ++i ) {
				int loc = (int)( NameBlockLocation + i * 0x34 );
				SscrName n = Names[i];

				byte[] Id = Encoding.ASCII.GetBytes( n.Id );
				Util.FillNull( File, loc, 0x08 );
				Util.CopyByteArrayPart( Id, 0, File, loc, Id.Length );
				byte[] Unknown = BitConverter.GetBytes( n.Unknown );
				Util.CopyByteArrayPart( Unknown, 0, File, loc + 0x08, Unknown.Length );
				byte[] Name = Encoding.UTF8.GetBytes( n.Name );
				Util.FillNull( File, loc + 0x0C, 0x20 );
				Util.CopyByteArrayPart( Name, 0, File, loc + 0x0C, Name.Length );
				byte[] Unknown2 = BitConverter.GetBytes( n.Unknown2 );
				Util.CopyByteArrayPart( Unknown2, 0, File, loc + 0x2C, Unknown2.Length );
				byte[] Unknown3 = BitConverter.GetBytes( n.Unknown3 );
				Util.CopyByteArrayPart( Unknown3, 0, File, loc + 0x30, Unknown3.Length );
			}

			for ( int i = 0; i < SystemTerms.Count; ++i ) {
				int loc = (int)( SystemTermBlockLocation + i * 0x17 );
				SscrSystemTerm st = SystemTerms[i];

				byte[] Term = Encoding.UTF8.GetBytes( st.Term );
				Util.FillNull( File, loc, 0x16 );
				Util.CopyByteArrayPart( Term, 0, File, loc, Term.Length );
				File[loc + 0x16] = st.Unknown;
			}

			for ( int i = 0; i < Somethings.Count; ++i ) {
				int loc = (int)( Something2BlockLocation + i * 0x2A );
				SscrSomething s = Somethings[i];

				byte[] Text = Encoding.UTF8.GetBytes( s.Text );
				Util.FillNull( File, loc, 0x20 );
				Util.CopyByteArrayPart( Text, 0, File, loc, Text.Length );

				byte[] Unknown1 = BitConverter.GetBytes( s.Unknown1 );
				Util.CopyByteArrayPart( Unknown1, 0, File, loc + 0x20, Unknown1.Length );
				byte[] Unknown2 = BitConverter.GetBytes( s.Unknown2 );
				Util.CopyByteArrayPart( Unknown2, 0, File, loc + 0x22, Unknown2.Length );
				byte[] Id = Encoding.ASCII.GetBytes( s.Id );
				Util.FillNull( File, loc + 0x24, 3 );
				Util.CopyByteArrayPart( Id, 0, File, loc + 0x24, Id.Length );
				File[loc + 0x27] = s.Unknown3;
				byte[] Unknown4 = BitConverter.GetBytes( s.Unknown4 );
				Util.CopyByteArrayPart( Unknown4, 0, File, loc + 0x28, Unknown4.Length );
			}

			System.IO.File.WriteAllBytes( OutFile, File );
		}
	}
}
