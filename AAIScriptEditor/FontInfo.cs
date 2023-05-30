using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AAIScriptEditor.Properties;
using System.IO;

namespace AAIScriptEditor
{
    class FontInfo
    {
        public float TextureSizeX;
        public float TextureSizeY;
        public int CharacterNum;
        public int CharacterSize;
        public int length;
        public CharacterData[] list;
        public int[] codes;
        private static FontInfo fi = null;

        public class CharacterData
        {
            public int code, rm, lm, xo, yo;
            public float rx, ry, rw, rh;
            public bool isSpace;

            public string toString()
            {
                string st = "[Code]" + code + "[RectTransform:[x]" + rx +
                    "[y]" + ry + "[w]" + rw + "[h]" + rh + "][righttMargin]" + rm +
                    "[leftMargin]" + lm + "[xOffset]" + xo + "[yOffset]" + yo +
                    "[isSpace]" + (isSpace ? "true" : "false");
                return st;
            }

            public void ReadData(BinaryReader br)
            {
                this.code = br.ReadInt32();
                this.rx = br.ReadSingle();
                this.ry = br.ReadSingle();
                this.rw = br.ReadSingle();
                this.rh = br.ReadSingle();
                this.rm = br.ReadInt32();
                this.lm = br.ReadInt32();
                this.xo = br.ReadInt32();
                this.yo = br.ReadInt32();
                if (br.ReadInt32() == 1)
                {
                    this.isSpace = true;
                }
                else
                    this.isSpace = false;
            }

            public void WriteData(BinaryWriter br)
            {
                br.Write(this.code);
                br.Write(this.rx);
                br.Write(this.ry);
                br.Write(this.rw);
                br.Write(this.rh);
                br.Write(this.rm);
                br.Write(this.lm);
                br.Write(this.xo);
                br.Write(this.yo);
                int x = 0;
                if (this.isSpace)
                {
                    x = 1;
                }
                br.Write(x);
            }
        }

        public void ReadData(BinaryReader br)
        {
            this.TextureSizeX = br.ReadSingle();
            this.TextureSizeY = br.ReadSingle();
            this.CharacterNum = br.ReadInt32();
            this.CharacterSize = br.ReadInt32();
            this.length = br.ReadInt32();
            list = new CharacterData[length];
            for (int i = 0; i < length; i++)
            {
                CharacterData cd = new CharacterData();
                cd.ReadData(br);
                list[i] = cd;
            }
        }

        static FontInfo ReadData(byte[] data)
        {
            byte[] header = new byte[44];
            for (int i = 0; i < 44; i++)
            {
                header[i] = data[i];
            }
            if (header[40] != 111 || header[39] != 102 || header[38] != 110 || header[37] != 105 || header[36] != 95 || header[35] != 116 || header[34] != 110 || header[33] != 111 || header[32] != 102)
            {
                throw new Exception();
            }
            byte[] nd = new byte[data.Length - 44];
            for (int i = 44; i < data.Length; i++)
            {
                nd[i - 44] = data[i];
            }
            FontInfo foi = new FontInfo();
            using (BinaryReader br = new BinaryReader(new MemoryStream(nd)))
                foi.ReadData(br);
            return foi;
        }
        
        public static FontInfo GetInstance()
        {
            if (fi == null)
            {
                fi = ReadData(Resources.FontEU);
                fi.GetCodes();
            }
            return fi;
        }

        public void GetCodes()
        {
            int max = list[0].code;
            for (int i = 1; i < list.Length; i++)
            {
                if (list[i].code > max) max = list[i].code;
            }
            codes = new int[max + 1];
            for (int i = 0; i < list.Length; i++)
            {
                codes[list[i].code] = i;
            }
        }

        public FontInfo.CharacterData GetCharacterData(int code)
        {
            if (codes[code] == 0)
            {
                return list[codes[33180]];
            }
            return list[codes[code]];
        }
    }
}
