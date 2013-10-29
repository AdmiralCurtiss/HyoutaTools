using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HyoutaTools;

namespace HyoutaTools.AceAttorney {
	// a lot of this stuff here is taken or modified from CC3, http://sourceforge.net/projects/comebackcourt/

	enum ScriptEntryEnum : ushort {
		Noop = 0x00, // 0
		B = 0x01, // 0
		P = 0x02, // 0
		Color = 0x03, // 1
		Pause = 0x04, // 0
		Music = 0x05, // 2
		Sound = 0x06, // 2
		FullscreenText = 0x07, // 0
		FingerChoice2argsJmp = 0x08, // 2
		FingerChoice3argsJmp = 0x09, // 3
		ReJmp = 0x0A, // 1
		Speed = 0x0B, // 1
		Wait = 0x0C, // 1
		EndJmp = 0x0D, // 0
		Name = 0x0E, // 1
		TestimonyBox = 0x0F, // 2
		Unk10 = 0x10, // 1
		EvidenceWindowPlain = 0x11, // 0
		BGColor = 0x12, // 3
		ShowPhoto = 0x13, // 1
		RemovePhoto = 0x14, // 0
		SpecialJmp = 0x15, // 0
		SaveGame = 0x16, // 0
		NewEvidence = 0x17, // 1
		Unk18 = 0x18, // 1
		UpdateEvidence = 0x19, // 2
		Swoosh = 0x1A, // 4
		BG = 0x1B, // 1
		HideTextbox = 0x1C, // 1
		Unk1D = 0x1D, // 1
		Person = 0x1E, // 3
		HidePerson = 0x1F, // 0
		Unk20 = 0x20, // 1
		EvidenceWindowLifebar = 0x21, // 0
		FadeMusic = 0x22, // 2
		Unk23 = 0x23, // 2
		Reset = 0x24, // 0
		Unk25 = 0x25, // 1
		Unk26 = 0x26, // 1
		Shake = 0x27, // 2
		TestimonyAnimation = 0x28, // 1
		Unk29 = 0x29, // 1
		Unk2A = 0x2A, // 3
		Penalty = 0x2B, // 0
		Jmp = 0x2C, // 1
		NextpageButton = 0x2D, // 0
		NextpageNobutton = 0x2E, // 0
		Animation = 0x2F, // 2
		Unk30 = 0x30, // 1
		PersonVanish = 0x31, // 2
		Unk32 = 0x32, // 2
		Unk33 = 0x33, // 2
		FadeToBlack = 0x34, // 1
		Unk35 = 0x35, // 2
		Unk36 = 0x36, // -1
		Unk37 = 0x37, // 2
		Unk38 = 0x38, // 1
		LittleSprite = 0x39, // 1
		Unk3A = 0x3A, // 2
		Unk3B = 0x3B, // 2
		Unk3C = 0x3C, // 1
		Unk3D = 0x3D, // 1
		Unk3E = 0x3E, // 1
		Unk3F = 0x3F, // 0
		Unk40 = 0x40, // 0
		Unk41 = 0x41, // 0
		SoundToggle = 0x42, // 1
		Lifebar = 0x43, // 1
		Guilty = 0x44, // 1
		Unk45 = 0x45, // 0
		BGTile = 0x46, // 1
		Unk47 = 0x47, // 2
		Unk48 = 0x48, // 2
		WinGame = 0x49, // 0
		Unk4A = 0x4A, // -1
		Unk4B = 0x4B, // 1
		Unk4C = 0x4C, // 0
		Unk4D = 0x4D, // 2
		WaitNoAnim = 0x4E, // 1
		Unk4F = 0x4F, // 7
		Unk50 = 0x50, // 1
		Unk51 = 0x51, // 2
		Unk52 = 0x52, // 1
		Unk53 = 0x53, // 0
		LifebarSet = 0x54, // 2
		Unk55 = 0x55, // 2
		Unk56 = 0x56, // 2
		PsychoLock = 0x57, // 1
		Unk58 = 0x58, // 0
		Unk59 = 0x59, // 1
		Unk5A = 0x5A, // 1
		Unk5B = 0x5B, // 2
		Unk5C = 0x5C, // -1
		Unk5D = 0x5D, // 1
		Unk5E = 0x5E, // 1
		Unk5F = 0x5F, // 3
		Unk60 = 0x60, // -1
		Unk61 = 0x61, // 3
		Unk62 = 0x62, // 0
		Unk63 = 0x63, // -1
		Unk64 = 0x64, // 1
		Unk65 = 0x65, // 2
		Unk66 = 0x66, // 3
		Unk67 = 0x67, // 0
		Unk68 = 0x68, // 0
		BGAnim = 0x69, // 2
		SwitchScript = 0x6A, // 1
		Unk6B = 0x6B, // 3
		Unk6C = 0x6C, // 1
		Unk6D = 0x6D, // 1
		Unk6E = 0x6E, // 1
		Unk6F = 0x6F, // 1
		Unk70 = 0x70, // 3
		Unk71 = 0x71, // 3
		Unk72 = 0x72, // 0
		Unk73 = 0x73, // 0
		Unk74 = 0x74, // -1
		Unk75 = 0x75, // -1
		Unk76 = 0x76, // -1
		Unk77 = 0x77, // -1
		Unk78 = 0x78, // -1
		Unk79 = 0x79, // -1
		Unk7A = 0x7A, // -1
		Unk7B = 0x7B, // -1
		Unk7C = 0x7C, // -1
		Unk7D = 0x7D, // -1
		Unk7E = 0x7E, // -1
		Unk7F = 0x7F, // -1
	}

	class ScriptEntry {
		public ScriptEntryEnum Type;
		public ushort[] Data;

		private static int[] _argCount = null;
		protected static int[] ArgCount {
			get {
				if ( _argCount == null ) {
					_argCount = new int[] {
						0 , 0 , 0 , 1 , 0 , 2 , 2 , 0 , 2 , 3 , 1 , 1 , 1 , 0 , 1 , 2 ,
						1 , 0 , 3 , 1 , 0 , 0 , 0 , 1 , 1 , 2 , 4 , 1 , 1 , 1 , 3 , 0 ,
						1 , 0 , 2 , 2 , 0 , 1 , 1 , 2 , 1 , 1 , 3 , 0 , 1 , 0 , 0 , 2 ,
						1 , 2 , 2 , 2 , 1 , 2 , -1, 2 , 1 , 1 , 2 , 2 , 1 , 1 , 1 , 0 ,
						0 , 0 , 1 , 1 , 1 , 0 , 1 , 2 , 2 , 0 , -1, 1 , 0 , 2 , 1 , 7 ,
						1 , 2 , 1 , 0 , 2 , 2 , 2 , 1 , 0 , 1 , 1 , 2 , -1, 1 , 1 , 3 ,
						-1, 3 , 0 , -1, 1 , 2 , 3 , 0 , 0 , 2 , 1 , 3 , 1 , 1 , 1 , 1 ,
						3 , 3 , 0 , 0 , -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1,
					};
				}
				return _argCount;
			}
		}
		private static char[] _glyphTable = null;
		protected static char[] GlyphTable {
			get {
				if ( _glyphTable == null ) {
					_glyphTable = new char[] {
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						'_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', '_', 
						// 0x80
						'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 
						'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 
						'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 
						'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '!', '?', 
						'あ', 'い', 'う', 'え', 'お', 'か', 'き', 'く',
						'け', 'こ', 'さ', 'し', 'す', 'せ', 'そ', 'た',
						'ち', 'つ', 'て', 'と', 'な', 'に', 'ぬ', 'ね',
						'の', 'は', 'ひ', 'ふ', 'へ', 'ほ', 'ま', 'み',
						'む', 'め', 'も', 'や', 'ゆ', 'よ', 'ら', 'り',
						'る', 'れ', 'ろ', 'わ', 'を', 'ん', 'が', 'ぎ',
						'ぐ', 'げ', 'ご', 'ざ', 'じ', 'ず', 'ぜ', 'ぞ',
						'だ', 'ぢ', 'づ', 'で', 'ど', 'ば', 'び', 'ぶ',
						'べ', 'ぼ', 'ぱ', 'ぴ', 'ぷ', 'ぺ', 'ぽ', 'ぁ',
						'ぃ', 'ぅ', 'ぇ', 'ぉ', 'ゃ', 'ゅ', 'ょ', 'っ',
						'ア', 'イ', 'ウ', 'エ', 'オ', 'カ', 'キ', 'ク',
						'ケ', 'コ', 'サ', 'シ', 'ス', 'セ', 'ソ', 'タ',
						'チ', 'ツ', 'テ', 'ト', 'ナ', 'ニ', 'ヌ', 'ネ',
						'ノ', 'ハ', 'ヒ', 'フ', 'ヘ', 'ホ', 'マ', 'ミ',
						'ム', 'メ', 'モ', 'ヤ', 'ユ', 'ヨ', 'ラ', 'リ',
						'ル', 'レ', 'ロ', 'ワ', 'ヲ', 'ン', 'ガ', 'ギ',
						'グ', 'ゲ', 'ゴ', 'ザ', 'ジ', 'ズ', 'ゼ', 'ゾ',
						'ダ', 'ヂ', 'ヅ', 'デ', 'ド', 'バ', 'ビ', 'ブ',
						'ベ', 'ボ', 'パ', 'ピ', 'プ', 'ペ', 'ポ', 'ァ',
						'ィ', 'ゥ', 'ェ', 'ォ', 'ャ', 'ュ', 'ョ', 'ッ',
						'ヴ', '.', '⇒', '「', '」', '(', ')', '『',
						'』', '“', '”', '▼', '▲', ':', '、', ',',
						'＋', '／', '*', '\'', '―', '・', '。', '％',
						'‥', '～', '《', '》', '&', '☆', '♪', ' ',
　 						'-', '"', '存', '現', '在', '状', '況', '中', 
						'断', '選', 'é', '失', '敗', '消', '去', '初',
						'期', '態', '戻', '追', '加', '思', '出', '逆',
						'転', '第', '一', '回', '法', '廷', '前', '編',
						'後', '盗', '探', '偵', '二', '忘', '果', '無',
						'手', '今', '発', '言', '証', '拠', '品', '特',
						'異', '議', '認', '弁', '護', '人', '慎', '重',
						'残', '念', '私', '見', '得', '与', '違', '示',
						'関', '係', '全', '然', '考', '裁', '判', '長',
						'心', '悪', '顔', '説', '力', '欠', '感', '却',
						'問', '題', '込', '千', '尋', '怒', '負', '被',
						'告', '立', '聞', '新', '米', '側', '提', '度',
						'目', '彼', '女', '会', '起', '殺', '年', '綾',
						'里', '月', '日', '午', '時', '分', '地', '方',
						'所', '控', '室', '星', '影', '先', '生', '落',
						'挙', '動', '不', '審', '何', '首', '部', '舞',
						'台', '＿', '宇', '宙', '介', '驚', '事', '件',
						'担', '当', '知', '入', '大', '丈', '夫', '依',
						'頼', '情', '実', '助', '士', '半', '受', '自',
						'身', '同', '成', '歩', '堂', '龍', '開', '準',
						'備', '完', '了', '検', '察', '来', '本', '氏',
						'急', '用', '隣', '亜', '内', '冒', '頭', '論',
						'子', '守', '勝', '気', '借', '明', '勇', '盟',
						'学', '有', '名', '害', '呑', '田', '菊', '三',
						'薬', '場', '撮', '写', '真', '直', '通', '報',
						'者', '死', '体', '逃', '罪', '意', '理', '記',
						'緑', '因', '変', '答', '質', '資', '料', '痛',
						'闘', '武', '器', '必', '電', '凍', '空', '詰',
						'決', '亡', '型', '使', '進', '要', '機', '押',
						'男', '正', '教', '育', '他', '切', '替', '原',
						'困', '物', '美', '柳', '恋', '白', '印', '象',
						'定', '申', '元', '可', '能', '性', '引', '行',
						'案', '信', '疑', '話', '改', '職', '業', '容',
						'友', '述', '食', '暴', '命', '解', '足', '宿',
						'最', '的', '背', '描', '耳', '好', '突', '国',
						'旗', '待', '革', '着', '泣', '強', '飲', '市',
						'販', '作', '持', '屋', '指', '紋', '静', '粛',
						'犯', '腕', '計', '止', '瞬', '間', '剖', '呼',
						'校', '舎', '裏', '庭', '別', '倒', '以', '上',
						'殴', '角', '医', '精', '製', '調', '合', '研',
						'究', '連', '錬', '金', '術', '師', '験', '化',
						'高', '圧', '流', '械', '詳', '刻', '集', '授',
						'終', '多', '少', '規', '格', '専', '線', '柱',
						'設', '走', '次', '仲', '油', '寝', '昼', '焼',
						'推', '致', '荷', '凶', '抜', '総', '由', '送',
						'株', '主', '張', '故', '苦', '図', '想', '朝',
						'若', '両', '跡', '向', '余', '点', '華', '麗',
						'散', '音', '配', '触', '小', '雨', '垂', '浮',
						'口', '帰', '血', '鋭', '似', '安', '式', '請',
						'夜', '早', '反', '撃', '離', '結', '移', '収',
						'風', '飛', '形', '確', '穴', '書', '笑', '声',
						'休', '憩', '恐', '運', '勉', '返', '恥', '貸',
						'対', '柄', '司', '預', 'ヶ', '等', '常', '閉',
						'宵', '春', '宮', '霧', '緒', '狩', '魔', '冥',
						'御', '剣', '怜', '侍', '文', '再', '様', '木',
						'槌', '親', '秒', '味', '参', '取', '任', '争',
						'川', '拝', '利', '深', '求', '相', '警', '楽',
						'近', '歌', '鳴', '神', '童', '雷', '付', '停',
						'古', '修', '工', '予', '激', '敵', '悲', '々',
						'外', '偽', '暗', '道', '光', '景', '応', '援',
						'愛', '面', '席', '官', '慕', '平', '共', '攻',
						'鎖', '個', '奥', '咲', '査', '望', '否', '代',
						'毒', '盛', '花', '傷', '火', '液', '量', '妙',
						'伸', '段', '聴', '巻', '幸', '紅', '茶', '猛',
						'処', '逮', '捕', '捜', '始', '末', '乃', '荘',
						'仕', '析', '痕', '覚', '未', '員', '噛', '妄',
						'晴', '画', '宝', '虫', '座', '善', '鬼', '択',
						'階', '退', '歯', '往', '許', '青', '永', '久',
						'試', '遊', '悟', '毛', '緊', '救', '世', '語',
						'■', '刑', '冠', '庫', '諸', '君', '隊', '照',
						'灯', '満', '律', '務', '掃', '除', '秘', '展',
						'倉', '院', '週', '催', '招', '匠', '仮', '姿',
						'霊', '媒', '怪', '山', '家', '冗', '談', '伝',
						'難', '続', '局', '表', '菱', '百', '貨', '店',
						'数', '届', '狙', '観', '葉', '植', '和', '並',
						'読', '窓', '巨', '超', '街', '買', '施', '建',
						'映', '妹', '晩', '封', '創', '供', '殿', '密',
						'儀', '割', '漢', '字', '破', '片', '勾', '玉',
						'役', '値', '打', '売', '姉', '滝', '某', '留',
						'置', '途', '種', '天', '杉', '優', '紙', '礼',
						'級', '敷', '掛', '軸', '布', '増', '活', '像',
						'絶', '呪', '詞', '六', '荒', '糸', '鋸', '圭',
						'熱', '昨', '非', '厳', '管', '乗', '博', '館',
						'嬢', '価', '協', '公', '帳', '涙', '低', '毎',
						'惜', '携', '帯', '快', '威', '岳', '哀', '牙',
						'防', '材', '鑑', '銭', '企', '扉', '看', '板',
						'界', '祭', '黄', '七', '支', '刀', '刃', '禁',
						'整', '例', '丙', '謝', '統', '広', '約', '翌',
						'我', '混', '沌', '宴', '貴', '旅', '紳', '条',
						'園', '為', '芸', '訳', '枝', '商', '曲', '乾',
						'左', '弱', '横', '視', '色', '際', '騎', '魑',
						'魅', '魍', '魎', '跳', '梁', '跋', '扈', '頻',
						'闇', '帷', '帝', '都', '包', '緞', '独', '栄',
						'遺', '骨', '肉', '派', '脳', '涯', '髪', '討',
						'服', '装', '珍', '占', '紹', '涼', '功', '飾',
						'机', '具', '水', '炭', '素', '酸', '棚', '著',
						'鏡', '標', '菌', '絵', '希', '挟', '球', '筒',
						'万', '円', '脅', '迫', '吉', '箱', '石', '奪',
						'戦', '注', '魚', '拾', '海', '燃', '費', '住',
						'騒', '習', '社', '婦', '居', '産', '父', '母',
						'房', '執', '族', '赤', '車', '監', '耐', '基',
						'宅', '省', '章', '夏', '冬', '紀', '奇', '区',
						'投', '将', '排', '危', '険', '冷', '誤', '番',
						'狂', '導', '比', '類', '孤', '才', '称', '号',
						'愚', '極', '客', '妻', '矢', '政', '志', '周',
						'迷', '細', '制', '免', '裂', '英', '獄', '藤',
						'犬', '黒', '珈', '琲', '般', '軽', '縁', '徴',
						'摘', '刺', '差', '仏', '衣', '算', '登', '滴',
						'己', '識', '老', '微', '襲', '責', '憶', '秀',
						'路', '泉', '昏', '岸', '彷', '徨', '響', '抱',
						'哲', '操', '紫', '拳', '倶', '副', '補', '佐',
						'右', '央', '挑', '札', '汚', '袋', '至', '崩',
						'染', '拘', '労', '交', '汗', '島', '兵', '衛',
						'絡', '復', '棒', '捨', '粗', '懸', '悩', '恨',
						'束', '模', '盲', '倍', '効', '洗', '振', '橋',
						'叩', '恩', '組', '抗', '肩', '牛', '乳', '厚',
						'震', '順', '済', '額', '契', '限', '泳', '網',
						'鉄', '令', '従', '銃', '採', '眼', '払', '換',
						'寺', '給', '侵', '底', '眠', '節', '叫', '床',
						'慣', '杯', '短', '治', '懲', '戒', '損', '遠',
						'脱', '即', '駆', '悔', '煮', '湯', '署', '窃',
						'喝', '采', '賊', '賭', '召', '喚', '恵', '単',
						'位', '評', '堕', '駄', '搬', '接', '宛', '筆',
						'蹴', '曜', '簿', '戯', '民', '邁', '遜', '罵',
						'詈', '雑', '獣', '蛮', '阿', '鼻', '蒙', '昧',
						'郎', '羅', '痍', '没', '薄', '賞', '罰', '露',
						'四', '五', '東', '西', '南', '北', '熟', '宣',
						'義', '則', '虚', '干', '訴', '喜', '祝', '渡',
						'惨', '扱', '賀', '兄', '弟', '浴', '忙', '土',
						'坊', '吐', '庵', '須', '皮', '災', '経', '傍',
						'版', '券', '科', '香', '薫', '誌', '洋', '営',
						'銀', '雪', '酢', '枚', '十', '嵐', '野', '俳',
						'句', '詩', '丁', '汁', '課', '錠', '敬', '岡',
						'砂', '枯', '壊', '胸', '幼', '児', '怖', '誉',
						'吹', '演', '王', '丸', '病', '層', '億', '融',
						'鹿', '羽', '権', '太', '抽', '放', '診', '息',
						'阪', '折', '福', '粉', '江', '戸', '寄', '町',
						'便', '衆', '池', '馬', '票', '盤', '芝', '九',
						'蔵', '虎', '之', '濃', '良', '尾', '踏', '辞',
						'轄', '遅', '井', '列', '射', '過', '砲', '到',
						'獲', '門', '胃', '娘', '努', '魂', '誘', '拐',
						'巡', '吾', '囚', '吊', '讐', '願', '属', '波',
						'囲', '練', '草', '辺', '岩', '飼', '揺', '脚',
						'症', '乱', '甘', '造', '距', '降', '避', '桜',
						'温', '清', '尼', '僧', '毘', '忌', '氷', '季',
						'斎', '夕', '晶', '鐘', '境', '夢', '吸', '技',
						'速', '弾', '瞑', '偉', '誠', '砕', '純', '歴',
						'坂', '零', '串', '腹', '陸', '暑', '森', '略',
						'侮', '辱', '畑', '炎', '埋', '越', '達', '壮',
						'墓', '掘', '豪', '聖', '率', '惑', '芽', '斜',
						'根', '寒', '堀', '寮', '洞', '壇', '鉢', '劇',
						'泊', '姫', '符', '承', '継', '垣', '港', '炉',
						'烈', '祈', '欲', '拒', '沈', '索', '史', '邪',
						'揮', '偶', '双', '財', '婚', '暮', '阻', '頂',
						'豊', '陣', '陽', '庁', '競', '谷', '互', '適',
						'爽', '寿', '閃', '松', '八', '輩', '粋', '枢',
						'憎', '浄', '肌', '絆', '澄', '隠', '巧', '舟',
						'辰', '河', '樹', '諏', '訪', '督', '村', '秋',
						'昌', '寛', '徳', '克', '稲', '敦', '釈', '粧',
						'絞', '添', '誕', '塚', '匡', '浜', '亘', '迎', 
				};
				}
				return _glyphTable;
			}
		}

		protected ScriptEntry() { }
		public ScriptEntry( Stream File ) {
			Type = (ScriptEntryEnum)File.ReadUInt16();

			if ( (ushort)Type < 0x80 ) {
				Data = new ushort[ArgCount[(int)Type]];
				for ( int i = 0; i < Data.Length; ++i ) {
					Data[i] = File.ReadUInt16();
				}
			} else {

			}
		}
	}
	class DummyScriptEntry : ScriptEntry {
		public string Name = "NONE SET";
		public string SpriteCharacter = "NONE SET";
		public string SpriteTalking = "NONE SET";
		public string SpriteIdle = "NONE SET";
		public bool NewTextbox = true;

		public DummyScriptEntry() { }
		public bool PrintEntryForLP( Stream File, StreamWriter Output ) {
			ushort Type = File.ReadUInt16();
			ushort tmp, tmp2, tmp3;

			if ( Type < 0x80 ) {
				switch ( (ScriptEntryEnum)Type ) {
					case ScriptEntryEnum.B:
						Output.Write( ' ' ); break;
					case ScriptEntryEnum.P:
					case ScriptEntryEnum.NextpageButton:
					case ScriptEntryEnum.NextpageNobutton:
						Output.Write( '\n' );
						NewTextbox = true;
						break;
					case ScriptEntryEnum.EndJmp:
						NewTextbox = true;
						return false;
					case ScriptEntryEnum.ReJmp:
						Output.Write( "\n[REJOIN SCRIPT AT: Section 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + "]" );
						break;
					case ScriptEntryEnum.Jmp:
						Output.Write( "\n[JUMP TO: Section 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + "]" );
						break;
					case ScriptEntryEnum.FingerChoice2argsJmp:
						Output.Write( "\n[MULTIPLE CHOICE: Sections 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + ", 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + "]" );
						break;
					case ScriptEntryEnum.FingerChoice3argsJmp:
						Output.Write( "\n[MULTIPLE CHOICE: Sections 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + ", 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + ", 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + "]" );
						break;
					case ScriptEntryEnum.Penalty:
						Output.Write( "\n[PENALTY]" );
						break;
					case ScriptEntryEnum.NewEvidence:
						tmp = File.ReadUInt16();
						Output.Write( "[NEW COURT RECORD ENTRY: 0x" + ( tmp & 0x3fff ).ToString( "X2" ) + " (" + ( ( tmp & 0x8000 ) == 0x8000 ? "PERSON" : "EVIDENCE" ) + ")" );
						if ( ( tmp & 0x4000 ) != 0x4000 ) Output.Write( " (SILENT)" );
						Output.Write( "]\n" );
						break;
					case ScriptEntryEnum.UpdateEvidence:
						tmp = File.ReadUInt16();
						tmp2 = File.ReadUInt16();
						Output.Write( "[UPDATE COURT RECORD ENTRY: 0x" + ( tmp & 0x3fff ).ToString( "X2" ) );
						Output.Write( " TO 0x" + ( tmp2 & 0x3fff ).ToString( "X2" ) );
						Output.Write( " (" + ( ( tmp & 0x8000 ) == 0x8000 ? "PERSON" : "EVIDENCE" ) + ")" );
						if ( ( tmp & 0x4000 ) != 0x4000 ) Output.Write( " (SILENT)" );
						Output.Write( "]\n" );
						break;
					case ScriptEntryEnum.TestimonyBox:
						Output.WriteLine( "[ON PRESS JUMP TO: Section 0x" + ( File.ReadUInt16() - 0x80 ).ToString( "X2" ) + " (?: " + ( File.ReadUInt16() ).ToString( "X2" ) + ")]" );
						break;
					case ScriptEntryEnum.Name:
						tmp = File.ReadUInt16();
						Name = GetName( tmp.SwapEndian() );
						break;
					case ScriptEntryEnum.Person:
						tmp = File.ReadUInt16();
						tmp2 = File.ReadUInt16();
						tmp3 = File.ReadUInt16();
						SpriteCharacter = GetName( tmp );
						SpriteTalking = tmp2.ToString( "X2" );
						SpriteIdle = tmp3.ToString( "X2" );
						break;
					default:
						for ( int i = 0; i < ArgCount[(int)Type]; ++i ) {
							File.ReadUInt16(); // just purge data, we don't really care in most cases
						}
						break;
				}
			} else {
				if ( Type >= GlyphTable.Length ) { return true; }
				if ( NewTextbox ) {
					if ( Name == SpriteCharacter ) {
						//Output.Write( "[" + SpriteCharacter + ", " + SpriteTalking + " -> " + SpriteIdle + "] " );
						Output.Write( "[" + SpriteCharacter + " " + SpriteTalking + "] " );
					} else {
						Output.Write( "[" + Name + "] " );
					}
					NewTextbox = false;
				}
				Output.Write( GlyphTable[Type] );
			}

			return true;
		}

		public string GetName( ushort NameArg ) {
			byte b1 = (byte)( NameArg & 0xFF );
			byte b2 = (byte)( NameArg >> 8 );

			switch ( b1 ) {
				case 0x00: return "NONE";
				case 0x01: return "???m";
				case 0x02: return "Phoenix";
				case 0x03: return "Police";
				case 0x04: return "Maya";
				case 0x08: return "Judge";
				case 0x09: return "Edgeworth";
				case 0x0C: return "Grossberg";
				case 0x0F: return "???f";
				case 0x13: return "TV";
				case 0x14: return "Gumshoe";
				case 0x19: return "Larry";
				case 0x1F: return "Lotta";
				case 0x21: return "Karma";
				case 0x2B: return "Chief";

				default: return b1.ToString( "X2" );
			}
		}
	}

	class Script {
		List<uint> Pointers;
		List<List<ScriptEntry>> Entries;

		public static int DumpTextForLP( List<string> args ) {
			string Filename = args[0];
			string Outfilename = args[1];
			var File = new FileStream( Filename, FileMode.Open );
			var OutfileStream = new FileStream( Outfilename, FileMode.Create );
			var OutfileWriter = new StreamWriter( OutfileStream );


			var script = new Script();
			script.LoadFile( File );
			script.PrintForLP( File, OutfileWriter );
			OutfileWriter.Close();
			OutfileStream.Close();
			return 0;
		}


		public void LoadFile( Stream File ) {
			File.Position = 0;
			uint PointerCount = File.ReadUInt32();

			Pointers = new List<uint>( (int)PointerCount );
			for ( uint i = 0; i < PointerCount; ++i ) {
				Pointers.Add( File.ReadUInt32() );
			}
		}

		public void PrintForLP( Stream File, StreamWriter Outfile ) {
			DummyScriptEntry d = new DummyScriptEntry();
			for ( int i = 0; i < Pointers.Count; ++i ) {
				Outfile.WriteLine( "[Section 0x" + i.ToString( "X2" ) + ']' );
				File.Position = Pointers[i];
				if ( File.Position >= File.Length ) { continue; }
				while ( d.PrintEntryForLP( File, Outfile ) ) {
					if ( File.Position >= File.Length ) { break; }
				}
				Outfile.WriteLine();
				Outfile.WriteLine();
			}
		}
	}
}
