using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Gust.ebm {
	public class TextDumper {
		public static int Execute( List<string> args ) {
			if ( args.Count < 2 ) {
				Console.WriteLine( "Usage: game infile.ebm [outfile.txt]" );
				Console.WriteLine( "  game can be any of:" );
				Console.WriteLine( "  - [AT3] for Ar tonelico 3" );
				Console.WriteLine( "  - [AnS] for Ar nosurge" );
				Console.WriteLine( "  All names will be printed as 'Unknown' if none of those are provided." );
				return -1;
			}

			string game = args[0];
			string infile = args[1];
			string outfile = args.Count >= 3 ? args[2] : args[1] + ".txt";

			var ebm = new ebm( infile );

			bool debug = args.Contains( "--debug" );

			Dictionary<int, string> names;
			switch ( game.ToLowerInvariant() ) {
				case "at3": names = GenerateAt3Dict(); break;
				case "ans": names = GenerateAnSDict(); break;
				default: names = new Dictionary<int, string>(); break;
			}

			List<string> text = new List<string>();
			foreach ( var e in ebm.EntryList ) {
				if ( debug ) {
					text.Add( "[Id  ] " + e.Ident );
					text.Add( "[Unk2] " + e.Unknown2 );
					text.Add( "[Unk3] " + e.Unknown3 );
					text.Add( "[Unk5] " + e.Unknown5 );
					text.Add( "[Unk6] " + e.Unknown6 );
					text.Add( "[Unk7] " + e.Unknown7 );
					text.Add( "[Unk8] " + e.Unknown8 );
				}
				if ( names.ContainsKey( e.CharacterId ) ) {
					text.Add( names[e.CharacterId] );
				} else {
					text.Add( "[Unknown_" + e.CharacterId + "]" );
				}
				text.Add( e.Text.Replace( "<CR>", "\r\n" ) );
				text.Add( "" );
			}

			System.IO.File.WriteAllLines( outfile, text.ToArray() );
			return 0;
		}

		public static Dictionary<int, string> GenerateAt3Dict() {
			return new Dictionary<int, string>() {
				{  -1, ""},
				{   0, "Aoto"},
				{   1, "Tatsumi"},
				{   2, "Cocona"},
				{   3, "Hikari Gojo"},
				{   4, "Saki"},
				{   5, "Sarapatra"},
				{   6, "Filament"},
				{   7, "Sakia Lumei"},
				{   8, "Finnel"},
				{   9, "Yurisica"},
				{  10, "Soma"},
				{  11, "Suzunomia"},
				{  12, "Tyria"},
				{  13, "Ar Ru"},
				{  14, "???"},
				{  15, "System"},
				{  16, "Harvestasha"},
				{  17, "Harvestasha"},
				{  18, "Harvestasha"},
				{  19, "Raphael"},
				{  20, "Richa"},
				{  21, "Gengai"},
				{  22, "Katene"},
				{  23, "Akane"},
				{  24, "Mute"},
				{  25, "Mute"},
				{  26, "Luphan"},
				{  27, "Ayatane"},
				{  28, "Jack"},
				{  29, "Krushe"},
				{  30, "Sasha"},
				{  31, "Teppo"},
				{  32, "Steeps"},
				{  33, "Auntie Leu"},
				{  34, "Nurse"},
				{  35, "Kiraha"},
				{  36, "Training Monk"},
				{  37, "Mayor"},
				{  38, "Eternus Mayor"},
				{  39, "Dive Shopkeeper"},
				{  40, "Man"},
				{  41, "Man"},
				{  42, "Elderly Man"},
				{  43, "Young Woman"},
				{  44, "Reyvateil"},
				{  45, "Reyvateil"},
				{  46, "Reyvateil"},
				{  47, "Reyvateil"},
				{  48, "Shopkeeper"},
				{  49, "Kairi"},
				{  50, "Totora"},
				{  51, "Rendeisha"},
				{  52, "Claude"},
				{  53, "Kukuro"},
				{  54, "Woman"},
				{  55, "Boy"},
				{  56, "Girl"},
				{  57, "Chicken"},
				{  58, "Chick"},
				{  59, "Hawker"},
				{  60, "Arms Merchant"},
				{  61, "General Store Manager"},
				{  62, "Ultimate Border Human"},
				{  63, "MYU"},
				{  64, "MYU Mk.2"},
				{  65, "Reyvaroid"},
				{  66, "Zzx_Ru"},
				{  67, "Egg"},
				{  68, "Ciela"},
				{  69, "Antibody"},
				{  70, "Announcer"},
				{  71, "Child"},
				{  72, "Aoto & Tatsumi"},
				{  73, "Aoto & Finnel"},
				{  74, "Aoto & Saki"},
				{  75, "Akane & Richa"},
				{  76, "Saki & Finnel"},
				{  77, "Clustanian Soldier"},
				{  78, "Clustanian Soldier"},
				{  79, "Clustanian Soldier"},
				{  80, "Clustanian Soldier"},
				{  81, "Great Fang Soldier"},
				{  82, "Great Fang Soldier"},
				{  83, "Great Fang Soldier"},
				{  84, "Great Fang Soldier"},
				{  85, "Archian Soldier"},
				{  86, "Archian Soldier"},
				{  87, "Archian Soldier"},
				{  88, "Archian Soldier"},
				{  89, "???"},
				{  90, "Bombastic Man"},
				{  91, "Arche"},
				{  92, "Ko-Kitty"},
				{  93, "Zwelivelle"},
				{  94, "Momoko"},
				{  95, "Haveli"},
				{  96, "Kabotatsu"},
				{  97, "Aretia"},
				{  98, "Flip Frog"},
				{  99, "Zoi"},
				{ 100, "Ternita"},
				{ 101, "Unita"},
				{ 102, "Ho-ryo-ryo"},
				{ 103, "Meryl"},
				{ 104, "Destiny"},
				{ 105, "Lorelei"},
				{ 106, "Slymer"},
				{ 107, "Thunderles"},
				{ 108, "Hagure"},
				{ 109, "Castellia"},
				{ 110, "Manasphere"},
				{ 111, "Home Aloner"},
				{ 112, "Executie"},
				{ 113, "HWXv2\xCE\xB2"},
				{ 114, "Enter Key"},
				{ 115, "Alt Key"},
				{ 116, "Shift Key"},
				{ 117, "Return Key"},
				{ 118, "Momoko"},
				{ 119, "Engage"},
				{ 120, "Modeco"},
				{ 121, "Buckle"},
				{ 122, "Animesia"},
				{ 123, "Archelesser"},
				{ 124, "Sheda"},
				{ 125, "Garter"},
				{ 126, "Theateria"},
				{ 127, "Mimic Mike"},
				{ 128, "Walkie"},
				{ 129, "Maidria"},
				{ 130, "Ulurua"},
				{ 131, "Sioni"},
				{ 132, "Ciela Cruto"},
				{ 133, "Patio Cruta"},
				{ 134, "Saki Portal"},
				{ 135, "Andante"},
				{ 136, "Allegretto"},
				{ 137, "Yeeel"},
				{ 138, "Manjo Cruto"},
				{ 139, "Memor Cruto"},
				{ 140, "Nam"},
				{ 141, "Tuna"},
				{ 142, "Moff Moff"},
				{ 143, "Auspia"},
				{ 144, "Medi"},
				{ 145, "Poemy"},
				{ 146, "Sooweets"},
				{ 147, "Spring Breeze"},
				{ 148, "Summer Gale"},
				{ 149, "Excellent"},
				{ 150, "Solar Moon"},
				{ 151, "Labo Labo"},
				{ 152, "Ninety-Nine"},
				{ 153, "Autumnal Chill"},
				{ 154, "Wintry Wind"},
				{ 155, "Nightmare"},
				{ 156, "Grace"},
				{ 157, "Pocco"},
				{ 158, "Pudding"},
				{ 159, "Mimi"},
				{ 160, "Gaylord"},
				{ 161, "Emerge"},
				{ 162, "Yoko Beach"},
				{ 163, "Oceanti"},
				{ 164, "Sakiel"},
				{ 165, "Sol Cielli"},
				{ 166, "Sis Cici"},
				{ 167, "Folster"},
				{ 168, "Fin'el"},
				{ 169, "Sol Fal"},
				{ 170, "Sayako"},
				{ 171, "Kuru Kuru"},
				{ 172, "Tyriel"},
				{ 173, "Sol Clusty"},
				{ 174, "Rei Kurosaki"},
				{ 175, "Masakado"},
				{ 176, "Tatsumina No Mikoto Kokonu"},
				{ 177, "Saki"},
				{ 178, "Finnel"},
				{ 179, "Ria"},
				{ 180, "Aoto & Cocona"},
				{ 181, "Gengai & Raphael"},
				{ 182, "Harvestasha and Others"},
				{ 183, "Steeps & Mayor"},
				{ 184, "Everyone"},
				{ 185, "Child"},
				{ 186, "Soldiers"},
				{ 187, "Aisha"},
				{ 188, "Man"},
				{ 189, "Man"},
				{ 190, "Ar Ru"},
				{ 191, "AOTO"},
				{ 192, "MUNYU MUNYU GIRL"},
				{ 193, "YURIS"},
				{ 194, "FINNE"},
				{ 195, "SHOGUN TATSUMI"},
				{ 196, "SOMA"},
				{ 197, "GENGAI"},
				{ 198, "Girl"},
				{ 199, "Girl"},
				{ 200, "Girl"},
				{ 201, "GUARDIAN"},
				{ 202, "Supervisor"},
				{ 203, "Lil Akane"},
				{ 204, "Lil Finnel"},
				{ 205, "Richa & Cocona & Mute"},
				{ 206, "???"},
				{ 207, "Rere"},
				{ 208, "Kurogane"},
				{ 209, "DEMON LORD CHISEL"},
				{ 210, "Weapon Demon"},
				{ 211, "Auntie Leu"},
				{ 212, "Old Man Mole"},
				{ 213, "Super S Gramps"},
				{ 214, "Zeneca"},
				{ 215, "Startled Old Man"},
				{ 216, "Sasha"},
				{ 217, "Waldale"},
				{ 218, "Love Missionary"},
				{ 219, "Retro RT"},
				{ 220, "Cyber RT"},
				{ 221, "Danpei"},
				{ 222, "Beautiful Merchant"},
				{ 223, "Hoppy"}
			};
		}

		public static Dictionary<int, string> GenerateAnSDict() {
			return new Dictionary<int, string>() {
				{ -1, ""},
				{  1, "???"},
				{  5, "Cass"},
				{  6, "Ion"},
				{  7, "Delta"},
				{ 11, "Zill"},
				{ 13, "Nelo"},
				{ 17, "Undou"},
				{ 18, "Shirotaka"},
				{ 20, "Sarly"},
				{ 26, "Prime"},
				{ 33, "Urijou"},
				{ 36, "Nyuroki"},
				{ 37, "Gennori"},
				{ 39, "Love Commander"},
				{ 42, "Urijou & Nyuroki"},
				{138, "Nyuroki & Urijou"},
			};
		}
	}
}
