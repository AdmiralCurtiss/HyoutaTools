using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HyoutaTools.Streams;
using HyoutaPluginBase;

namespace HyoutaTools.Tales.Vesperia.SaveData {
	// 0x400 bytes in all versions of the game
	// contains flags for which save points have been visited
	// used for triggering the 'first save point' system message and the 'all save points' achievement
	public class SaveDataBlockSavePoint {
		public DuplicatableStream Stream;

		public SaveDataBlockSavePoint( DuplicatableStream blockStream ) {
			Stream = blockStream.Duplicate();
		}

		public void PrintData() {
			// save point flags, one byte each, 0x00 not visted 0x01 visited
			byte[] savePointFlags;
			using ( DuplicatableStream stream = Stream.Duplicate() ) {
				savePointFlags = stream.ReadUInt8Array( 0x59 );
			}

			PrintSavePoint( savePointFlags, 0x00, "Fiertia Deck (Docked at Atherum)" );
			PrintSavePoint( savePointFlags, 0x01, "Atherum" );
			PrintSavePoint( savePointFlags, 0x02, "Fiertia Hold" ); // same flag for both opportunities?
			PrintSavePoint( savePointFlags, 0x03, "Keiv Moc (Middle)" );
			PrintSavePoint( savePointFlags, 0x04, "Keiv Moc (Boss)" );
			PrintSavePoint( savePointFlags, 0x05, "Zaphias (Lower Quarter)" );
			PrintSavePoint( savePointFlags, 0x06, "Zaphias (Royal Quarter)" );
			PrintSavePoint( savePointFlags, 0x07, "Zaphias Castle (Prison)" );
			PrintSavePoint( savePointFlags, 0x08, "Zaphias Castle (Kitchen)" ); // 2nd visit only
			PrintSavePoint( savePointFlags, 0x09, "Zaphias Castle (Hallways)" ); // before zagi fight
			PrintSavePoint( savePointFlags, 0x0A, "Zaphias Castle (Sword Stair)" );
			PrintSavePoint( savePointFlags, 0x0B, "Zaphias Castle (Big Hall)" ); // 2nd visit only, that big room that leads to the sword stair
			PrintSavePoint( savePointFlags, 0x0C, "Weasand of Cados (Middle)" );
			PrintSavePoint( savePointFlags, 0x0D, "Weasand of Cados (Exit)" );
			PrintSavePoint( savePointFlags, 0x0E, "Halure (Inn)" );
			PrintSavePoint( savePointFlags, 0x0F, "Ghasfarost (Bottom)" );
			PrintSavePoint( savePointFlags, 0x10, "Ghasfarost (Top)" );
			PrintSavePoint( savePointFlags, 0x11, "Myorzo (Vacant House)" );
			PrintSavePoint( savePointFlags, 0x12, "Mt. Temza (Middle)" );
			PrintSavePoint( savePointFlags, 0x13, "Mt. Temza (Boss)" );
			PrintSavePoint( savePointFlags, 0x14, "Deidon Hold" );
			PrintSavePoint( savePointFlags, 0x15, "Northeastern Hypionia" ); // aurnion before it's built
			PrintSavePoint( savePointFlags, 0x16, "Aurnion (Developing)" );
			PrintSavePoint( savePointFlags, 0x17, "Aurnion (Developed)" );
			PrintSavePoint( savePointFlags, 0x18, "Caer Bocram" );
			PrintSavePoint( savePointFlags, 0x19, "Quoi Woods" );
			PrintSavePoint( savePointFlags, 0x1A, "Dahngrest (Inn)" );
			PrintSavePoint( savePointFlags, 0x1B, "Ehmead Hill" );
			PrintSavePoint( savePointFlags, 0x1C, "Erealumen (Middle)" );
			PrintSavePoint( savePointFlags, 0x1D, "Erealumen (Boss)" );
			PrintSavePoint( savePointFlags, 0x1E, "Heracles (Near Engine Room)" );
			PrintSavePoint( savePointFlags, 0x1F, "Heracles (Near Control Room)" ); // zagi fight
			PrintSavePoint( savePointFlags, 0x20, "Zopheir (Boss)" ); // 1st visit only
			PrintSavePoint( savePointFlags, 0x21, "Zopheir (Near Aer Krene)" ); // 2nd visit only
			PrintSavePoint( savePointFlags, 0x22, "Manor of the Wicked" );
			PrintSavePoint( savePointFlags, 0x23, "Tarqaron (Middle)" );
			PrintSavePoint( savePointFlags, 0x24, "Tarqaron (Top)" );
			PrintSavePoint( savePointFlags, 0x25, "Baction B1F" );
			PrintSavePoint( savePointFlags, 0x26, "Baction B2F" ); // both save points on B2F share this flag...?
			PrintSavePoint( savePointFlags, 0x27, "Mantaic (Inn)" );
			PrintSavePoint( savePointFlags, 0x28, "Relewiese (Middle)" );
			PrintSavePoint( savePointFlags, 0x29, "Relewiese (Boss)" );
			PrintSavePoint( savePointFlags, 0x2A, "Capua Nor (Outside Ragou's Mansion)" );
			PrintSavePoint( savePointFlags, 0x2B, "Capua Nor (Inn)" );
			PrintSavePoint( savePointFlags, 0x2C, "Capua Torim (Inn)" );
			PrintSavePoint( savePointFlags, 0x2D, "Shaikos Ruins" );
			PrintSavePoint( savePointFlags, 0x2E, "Zaude (Side Entrance)" );
			PrintSavePoint( savePointFlags, 0x2F, "Zaude (Alexei)" );
			PrintSavePoint( savePointFlags, 0x30, "Zaude (Yeager)" );
			PrintSavePoint( savePointFlags, 0x31, "Aspio (Inn)" );
			PrintSavePoint( savePointFlags, 0x32, "Nordopolica (Inn)" );
			PrintSavePoint( savePointFlags, 0x33, "Heliord (Inn)" );
			PrintSavePoint( savePointFlags, 0x34, "Yormgen (Inn)" );
			PrintSavePoint( savePointFlags, 0x35, "Weasand of Kogorh (Oasis)" );
			PrintSavePoint( savePointFlags, 0x36, "Weasand of Kogorh (Exit)" );
			PrintSavePoint( savePointFlags, 0x37, "Egothor Forest" );
			PrintSavePoint( savePointFlags, 0x38, "Dahngrest Underpass (Oath)" );
			PrintSavePoint( savePointFlags, 0x39, "Ragou's Mansion" ); // basement dungeon midpoint
			PrintSavePoint( savePointFlags, 0x3A, "Dahngrest Underpass (Exit)" );
			PrintSavePoint( savePointFlags, 0x3B, "Abysmal Hollow (Aer Krene near Yumanju)" );
			PrintSavePoint( savePointFlags, 0x3C, "? Abysmal Hollow (Aer Krene near Zaphias)" );
			PrintSavePoint( savePointFlags, 0x3D, "Abysmal Hollow (Aer Krene near Heliord)" );
			PrintSavePoint( savePointFlags, 0x3E, "Abysmal Hollow (Aer Krene near Nordopolica)" );
			PrintSavePoint( savePointFlags, 0x3F, "? Abysmal Hollow (Center)" );
			PrintSavePoint( savePointFlags, 0x40, "City of the Waning Moon" );
			PrintSavePoint( savePointFlags, 0x41, "Necropolis of Nostalgia A3" );
			PrintSavePoint( savePointFlags, 0x42, "Necropolis of Nostalgia A6" );
			PrintSavePoint( savePointFlags, 0x43, "Necropolis of Nostalgia A9" );
			PrintSavePoint( savePointFlags, 0x44, "Necropolis of Nostalgia A Bottom" );
			PrintSavePoint( savePointFlags, 0x45, "Necropolis of Nostalgia B2" );
			PrintSavePoint( savePointFlags, 0x46, "Necropolis of Nostalgia B5" );
			PrintSavePoint( savePointFlags, 0x47, "Necropolis of Nostalgia B8" );
			PrintSavePoint( savePointFlags, 0x48, "Necropolis of Nostalgia B Bottom" );
			PrintSavePoint( savePointFlags, 0x49, "Necropolis of Nostalgia C3" );
			PrintSavePoint( savePointFlags, 0x4A, "Necropolis of Nostalgia C6" );
			PrintSavePoint( savePointFlags, 0x4B, "Necropolis of Nostalgia C9" );
			PrintSavePoint( savePointFlags, 0x4C, "Necropolis of Nostalgia C Bottom" );
			PrintSavePoint( savePointFlags, 0x4D, "Necropolis of Nostalgia D3" );
			PrintSavePoint( savePointFlags, 0x4E, "Necropolis of Nostalgia D6" );
			PrintSavePoint( savePointFlags, 0x4F, "Necropolis of Nostalgia D9" );
			PrintSavePoint( savePointFlags, 0x50, "Necropolis of Nostalgia D Bottom" );
			PrintSavePoint( savePointFlags, 0x51, "Necropolis of Nostalgia E3" );
			PrintSavePoint( savePointFlags, 0x52, "Necropolis of Nostalgia E6" );
			PrintSavePoint( savePointFlags, 0x53, "Necropolis of Nostalgia E9" );
			PrintSavePoint( savePointFlags, 0x54, "Necropolis of Nostalgia E Bottom" );
			PrintSavePoint( savePointFlags, 0x55, "Necropolis of Nostalgia F3" );
			PrintSavePoint( savePointFlags, 0x56, "Necropolis of Nostalgia F6" );
			PrintSavePoint( savePointFlags, 0x57, "Necropolis of Nostalgia F9" );
			PrintSavePoint( savePointFlags, 0x58, "Necropolis of Nostalgia F Bottom" );
		}

		private static void PrintSavePoint( byte[] flags, int index, string where ) {
			Console.WriteLine( "Save Point 0x" + index.ToString( "X2" ) + " " + ( flags[index] > 0 ? "[ ok ]" : "[MISS]" ) + ": " + where );
		}
	}
}
