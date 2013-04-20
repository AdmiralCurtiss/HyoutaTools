using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.TrailsInTheSkyScenarioDump {

	class E1 {
		public static int size = 4;
		public ushort u1;
		public ushort u2;
		public E1( byte[] File, int Loc ) {
			u1 = BitConverter.ToUInt16( File, Loc );
			u2 = BitConverter.ToUInt16( File, Loc + 2 );
			return;
		}
	}
	class E2 {
		public static int size = 4;
		public ushort u1;
		public ushort u2;
		public E2( byte[] File, int Loc ) {
			u1 = BitConverter.ToUInt16( File, Loc );
			u2 = BitConverter.ToUInt16( File, Loc + 2 );
			return;
		}
	}
	class E3 {
		public static int size = 0x20;
		public ushort[] u;
		public E3( byte[] File, int Loc ) {
			u = new ushort[size / 2];
			for ( int i = 0; i < size / 2; ++i ) {
				u[i] = BitConverter.ToUInt16( File, Loc + i * 2 );
			}
			return;
		}
	}
	class E4 {
		public static int size = 0x1C;
		public ushort[] u;
		public E4( byte[] File, int Loc ) {
			u = new ushort[size / 2];
			for ( int i = 0; i < size / 2; ++i ) {
				u[i] = BitConverter.ToUInt16( File, Loc + i * 2 );
			}
			return;
		}
	}
	class E5 {
		public static int size = 0x20;
		public ushort[] u;
		public E5( byte[] File, int Loc ) {
			u = new ushort[size / 2];
			for ( int i = 0; i < size / 2; ++i ) {
				u[i] = BitConverter.ToUInt16( File, Loc + i * 2 );
			}
			return;
		}
	}
	class E6 {
		public static int size = 0x24;
		public ushort[] u;
		public E6( byte[] File, int Loc ) {
			u = new ushort[size / 2];
			for ( int i = 0; i < size / 2; ++i ) {
				u[i] = BitConverter.ToUInt16( File, Loc + i * 2 );
			}
			return;
		}
	}
	class E7 {
		public static int size = 0;
		public ushort[] u;
		public E7( byte[] File, int Loc ) {
			u = new ushort[size / 2];
			for ( int i = 0; i < size / 2; ++i ) {
				u[i] = BitConverter.ToUInt16( File, Loc + i * 2 );
			}
			return;
		}
	}

	class ScenarioBin {
		public string Location;
		public string Id;

		public ushort e1loc;
		public ushort e1cnt;
		public ushort e2loc;
		public ushort e2cnt;
		public ushort e3loc;
		public ushort e3cnt;
		public ushort e4loc;
		public ushort e4cnt;
		public ushort e5loc;
		public ushort e5cnt;
		public ushort e6loc;
		public ushort e6cnt;
		public ushort esomething;
		public ushort e7loc;

		public List<E1> e1;
		public List<E2> e2;
		public List<E3> e3;
		public List<E4> e4;
		public List<E5> e5;
		public List<E6> e6;
		//public List<E7> e7;

		public ushort TextPtrsLoc;

		public ScenarioBin( string Filename ) {
			Load( System.IO.File.ReadAllBytes( Filename ) );
		}

		private void Load( byte[] File ) {
			Location = Util.GetTextShiftJis( File, 0 );
			Id = Util.GetTextShiftJis( File, 0x0A );

			e1loc = BitConverter.ToUInt16( File, 0x42 );
			e1cnt = BitConverter.ToUInt16( File, 0x44 );
			e2loc = BitConverter.ToUInt16( File, 0x46 );
			e2cnt = BitConverter.ToUInt16( File, 0x48 );
			e3loc = BitConverter.ToUInt16( File, 0x4A );
			e3cnt = BitConverter.ToUInt16( File, 0x4C );
			e4loc = BitConverter.ToUInt16( File, 0x4E );
			e4cnt = BitConverter.ToUInt16( File, 0x50 );
			e5loc = BitConverter.ToUInt16( File, 0x52 );
			e5cnt = BitConverter.ToUInt16( File, 0x54 );
			e6loc = BitConverter.ToUInt16( File, 0x56 );
			e6cnt = BitConverter.ToUInt16( File, 0x58 );
			esomething = BitConverter.ToUInt16( File, 0x5A );
			e7loc = BitConverter.ToUInt16( File, 0x5C );

			TextPtrsLoc = BitConverter.ToUInt16( File, 0x60 );

			e1 = new List<E1>( e1cnt );
			for ( int i = 0; i < e1cnt; ++i ) {
				e1.Add( new E1( File, e1loc + i * E1.size ) );
			}
			e2 = new List<E2>( e2cnt );
			for ( int i = 0; i < e2cnt; ++i ) {
				e2.Add( new E2( File, e2loc + i * E2.size ) );
			}
			e3 = new List<E3>( e3cnt );
			for ( int i = 0; i < e3cnt; ++i ) {
				e3.Add( new E3( File, e3loc + i * E3.size ) );
			}
			e4 = new List<E4>( e4cnt );
			for ( int i = 0; i < e4cnt; ++i ) {
				e4.Add( new E4( File, e4loc + i * E4.size ) );
			}
			e5 = new List<E5>( e5cnt );
			for ( int i = 0; i < e5cnt; ++i ) {
				e5.Add( new E5( File, e5loc + i * E5.size ) );
			}
			e6 = new List<E6>( e6cnt );
			for ( int i = 0; i < e6cnt; ++i ) {
				e6.Add( new E6( File, e6loc + i * E6.size ) );
			}




			return;
		}



	}
}
