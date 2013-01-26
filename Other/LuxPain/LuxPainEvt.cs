using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HyoutaTools.Other.LuxPain
{
    public class LuxPainEvtHeader
    {
        public UInt32 Magic;
        public UInt32 Unknown1;
        public UInt32 Unknown2;
        public UInt32 Unknown3;
        public UInt32 TextOffsetsLocation;
        public UInt32 TextLocation;
    }

    public class LuxPainEvtText
    {
        public UInt32 OffsetLocation;
        public String Text;

        internal void FormatForEditing()
        {
            for (int i = 0; i < 26; ++i)
            {
                Text = Text.Replace((char)('Ａ' + i), (char)('A' + i));
                Text = Text.Replace((char)('ａ' + i), (char)('a' + i));
            }
            for (int i = 0; i < 10; ++i)
            {
                Text = Text.Replace((char)('０' + i), (char)('0' + i));
            }

            Text = Text.Replace('　', ' ');
            Text = Text.Replace('！', '!');
            Text = Text.Replace('．', '.');
            Text = Text.Replace('？', '?');
            Text = Text.Replace('’', '\'');
            Text = Text.Replace('，', ',');
            Text = Text.Replace('”', '"');
            Text = Text.Replace('‐', '-');
            Text = Text.Replace('：', ':');
            Text = Text.Replace('；', ';');
            Text = Text.Replace('（', '(');
            Text = Text.Replace('）', ')');
            Text = Text.Replace("<ff00>", "\n");
            
            return;
        }

        internal void FormatForGameInserting()
        {
            for (int i = 0; i < 26; ++i)
            {
                Text = Text.Replace((char)('A' + i), (char)('Ａ' + i));
                Text = Text.Replace((char)('a' + i), (char)('ａ' + i));
            }
            for (int i = 0; i < 10; ++i)
            {
                Text = Text.Replace((char)('0' + i), (char)('０' + i));
            }

            Text = Text.Replace(' ', '　');
            Text = Text.Replace('!', '！');
            Text = Text.Replace('.', '．');
            Text = Text.Replace('?', '？');
            Text = Text.Replace('\'', '’');
            Text = Text.Replace(',', '，');
            Text = Text.Replace('"', '”');
            Text = Text.Replace('-', '‐');
            Text = Text.Replace(':', '：');
            Text = Text.Replace(';', '；');
            Text = Text.Replace('(', '（');
            Text = Text.Replace(')', '）');
            Text = Text.Replace("\n", "<ff00>");

            return;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    public class LuxPainEvt
    {
        public LuxPainEvtHeader Header;
        public List<LuxPainEvtText> TextEntries;

        public LuxPainEvt(String filename)
        {
            if (!LoadFile(System.IO.File.ReadAllBytes(filename)))
            {
                throw new Exception("LuxPainEvt: Load Failed!");
            }
        }

        public LuxPainEvt(byte[] Bytes)
        {
            if (!LoadFile(Bytes))
            {
                throw new Exception("LuxPainEvt: Load Failed!");
            }
        }

        private bool LoadFile(byte[] Bytes)
        {
            Header = new LuxPainEvtHeader();
            Header.Magic = BitConverter.ToUInt32(Bytes, 0x00);
            if (Header.Magic != 0x31304353) return false;
            Header.Unknown1 = BitConverter.ToUInt32(Bytes, 0x04);
            Header.Unknown2 = BitConverter.ToUInt32(Bytes, 0x08);
            Header.Unknown3 = BitConverter.ToUInt32(Bytes, 0x0C);
            Header.TextOffsetsLocation = BitConverter.ToUInt32(Bytes, 0x10);
            Header.TextLocation = BitConverter.ToUInt32(Bytes, 0x14);

            UInt32 PredictedTextAmount = (Header.TextLocation - Header.TextOffsetsLocation) / 2;
            TextEntries = new List<LuxPainEvtText>((int)PredictedTextAmount);

            for (UInt32 loc = Header.TextOffsetsLocation; loc < Header.TextLocation; loc += 2)
            {
                LuxPainEvtText txt = new LuxPainEvtText();
                txt.OffsetLocation = loc;
                UInt16 Offset = BitConverter.ToUInt16(Bytes, (int)loc);

                txt.Text = Util.GetTextLuxPain((int)(Header.TextLocation + (Offset * 2)), Bytes);

                TextEntries.Add(txt);
            }

            //foreach (LuxPainEvtText t in TextEntries)
            //{
            //    Console.WriteLine(t.Text);
            //}

            return true;
        }

        public void FormatTextForEditing()
        {
            foreach (LuxPainEvtText t in TextEntries)
            {
                t.FormatForEditing();
            }
        }

        public void FormatTextForGameInserting()
        {
            foreach (LuxPainEvtText t in TextEntries)
            {
                t.FormatForGameInserting();
            }
        }
    }
}
