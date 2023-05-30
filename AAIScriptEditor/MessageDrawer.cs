using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using AAIScriptEditor.Properties;
using System.IO;

namespace AAIScriptEditor
{
    class MessageDrawer
    {
        public const float MASCHTAB = 1.5f;
        const int initx = 120;
        const int inity = 60;
        const int endl = 48;
        const int w = 40;
        const int primallength = 371;
        const float koefcnt = 5.3f;
        int[] offsets;
        Image[] glyphs;
        int[] codes;

        public MessageDrawer()
        {
            InitGlyphsEu();
        }
        public async void DrawMessage(byte[] msg, Graphics g)
        {
            try
            {
                Clear(g);
                int x = initx, y = inity;
                for (int i = 0; i < msg.Length; i++)
                {
                    if (msg[i] == 254)
                    {
                        x = initx;
                        y += endl;
                        continue;
                    }
                    else if (GetJisEu(msg[i]) != -1)
                    {
                        int width = GetWidthEu(msg[i]);
                        /*FontInfo.CharacterData cd = FontInfo.GetInstance().GetCharacterData(jis);
                        Image glyph = CutGlyph(cd);*/
                        Image glyph = GetGlyphEu(msg[i]);
                        int rwid = (int)((float)glyph.Width / MASCHTAB);
                        int rhei = (int)((float)glyph.Height / MASCHTAB);
                        g.DrawImage(glyph, (float)x / MASCHTAB, (float)(y + GetOffsetEu(msg[i])) / MASCHTAB, rwid, rhei);
                        x += width;
                    }
                }
            } catch
            {
                return;
            }
        }

        public int GetJis(byte num)
        {
            if (num >= 1 && num <= 120)
            {
                return Global.gyakuten_conv_us[(num - 1) * 2] * 256 + Global.gyakuten_conv_us[num * 2 - 1];
            } else if (num >= 160 && num <= 225)
            {
                return num;
            }
            return -1;
        }

        public int GetJisEu(byte num)
        {
            if (num >= 1 && num <= 178)
            {
                return num + 300;
            }
            return -1;
        }

        public int GetWidth(byte num)
        {
            if (num >= 1 && num <= 120)
            {
                return (int)(Global.gyakuten_width_us[num - 1] * ((float)w / 11f));
            }
            else if (num >= 160 && num <= 225)
            {
                return Global.rus_width[num - 160];
            }
            return -1;
        }

        public int GetWidthEu(byte num)
        {
            if (num >= 1 && num <= 178)
            {
                return (int)(Global.eu_full_widthes[num - 1]) + 2;
            }
            return -1;
        }

        public int GetDSWidth(byte num)
        {
                return DSLengths[num - 1];
        }

        public int GetLength(byte[] msg)
        {
            int sum = 0;
            foreach (byte b in msg)
            {
                if (b == 254 || b == 0)
                {
                    break;
                }
                sum += GetWidth(b);
            }
            return sum;
        }
        public int GetLengthEu(byte[] msg)
        {
            int sum = 0;
            foreach (byte b in msg)
            {
                if (b == 254 || b == 0)
                {
                    break;
                }
                sum += GetWidthEu(b);
            }
            return sum;
        }

        public int GetDSLength(byte[] msg)
        {
            int sum = 0;
            foreach (byte b in msg)
            {
                if (b == 254 || b == 0)
                {
                    break;
                }
                sum += GetDSWidth(b);
            }
            return sum;
        }

        public int GetCenter(byte[] msg)
        {
            return (int)(90 - (float)(GetLength(msg) - primallength) / koefcnt);
        }

        public int GetCenterEu(byte[] msg)
        {
            return (int)(90 - (float)(GetLengthEu(msg) - primallength) / koefcnt);
        }

        public int GetDSCenter(byte[] msg)
        {
            return (256 - (GetDSLength(msg))) / 2;
        }

        public string Centering(string msg)
        {
            int x = msg.IndexOf("<bmv: 7, ");
            if (x == -1)
                return msg;
            string tmp = ScriptExtractor.RemoveBMVS(msg);
            byte[] s = ScriptExtractor.ToBytes(tmp);
            int cnt = GetCenterEu(s);
            int z = msg.IndexOf(">");
            msg = msg.Remove(x, z - x + 1);
            msg = "<bmv: 7, " + cnt + ", >" + msg;
            return msg;
        }

        public string DSCentering(string msg)
        {
            int x = msg.IndexOf("<bmv: 7, ");
            if (x == -1)
                return msg;
            string tmp = ScriptExtractor.RemoveBMVS(msg);
            byte[] s = ScriptExtractor.ToBytes(tmp);
            int cnt = GetDSCenter(s);
            int z = msg.IndexOf(">");
            msg = msg.Remove(x, z - x + 1);
            msg = "<bmv: 7, " + cnt + ", >" + msg;
            return msg;
        }

        public string[] CenterAll(string[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = Centering(text[i]);
            }
            return text;
        }

        public string[] CenterAllDS(string[] text)
        {
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = DSCentering(text[i]);
            }
            return text;
        }
        void InitGlyphs()
        {
            FontInfo.CharacterData[] cds = FontInfo.GetInstance().list;
            glyphs = new Image[187];
            offsets = new int[187];
            for (int i = 1; i <= 120; i++)
            {
                FontInfo.CharacterData cd = FontInfo.GetInstance().GetCharacterData(GetJis((byte)i));
                Image glyph = CutGlyph(cd);
                offsets[i - 1] = cd.yo;
                glyphs[i - 1] = glyph;
            }
            for (int i = 160; i <= 225; i++)
            {
                FontInfo.CharacterData cd = FontInfo.GetInstance().GetCharacterData(GetJis((byte)i));
                Image glyph = CutGlyph(cd);
                offsets[i - 40] = cd.yo;
                glyphs[i - 40] = glyph;
            }
        }

        void InitGlyphsEu()
        {
            FontInfo.CharacterData[] cds = FontInfo.GetInstance().list;
            glyphs = new Image[178];
            offsets = new int[178];
            for (int i = 1; i <= 178; i++)
            {
                FontInfo.CharacterData cd = FontInfo.GetInstance().GetCharacterData(i + 300);
                Image glyph = CutGlyphEu(cd);
                offsets[i - 1] = cd.yo;
                glyphs[i - 1] = glyph;
            }
        }

        Image GetGlyph(int x)
        {
            if (x >= 1 && x <= 120)
            {
                return glyphs[x - 1];
            } else if (x >= 160 && x <= 225)
            {
                return glyphs[x - 40];
            }
            return glyphs[88];
        }

        Image GetGlyphEu(int x)
        {
            if (x >= 1 && x <= 178)
            {
                return glyphs[x - 1];
            }
            return glyphs[88];
        }

        int GetOffset(int x)
        {
            if (x >= 1 && x <= 120)
            {
                return offsets[x - 1];
            }
            else if (x >= 160 && x <= 225)
            {
                return offsets[x - 40];
            }
            return 12;
        }

        int GetOffsetEu(int x)
        {
            if (x >= 1 && x <= 178)
            {
                return offsets[x - 1];
            }
            return 12;
        }

        public Image CutGlyph(FontInfo.CharacterData cd)
        {
            Rectangle rct = new Rectangle((int)cd.rx, 2048 - (int)cd.ry - (int)cd.rh, (int)cd.rw, (int)cd.rh + 1);
            Bitmap bmp = Resources.font_atlas as Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("No bitmap");

            // Crop the image:
            Bitmap cropBmp = bmp.Clone(rct, bmp.PixelFormat);

            return cropBmp;
        }

        public Image CutGlyphEu(FontInfo.CharacterData cd)
        {
            Rectangle rct = new Rectangle((int)cd.rx, 2048 - (int)cd.ry - (int)cd.rh, (int)cd.rw, (int)cd.rh + 1);
            Bitmap bmp = Resources.AtlasEU as Bitmap;

            // Check if it is a bitmap:
            if (bmp == null)
                throw new ArgumentException("No bitmap");

            // Crop the image:
            Bitmap cropBmp = bmp.Clone(rct, bmp.PixelFormat);

            return cropBmp;
        }

        public void Clear(Graphics g)
        {
            g.Clear(Form1.DefaultBackColor);
            g.DrawImage(Resources.MessageWindow, 0, 0, 1024 / MASCHTAB, 250 / MASCHTAB);
        }

        public byte[] DSLengths = new byte[] {
            6, 5, 6, 6, 6, 6, 6, 6, 6, 6, 9, 8, 9, 8, 7, 7,
            9, 8, 4, 6, 8, 8, 10, 8, 11, 8, 11, 8, 8, 8, 8, 8,
            14, 7, 8, 8, 7, 7, 7, 7, 7, 5, 7, 7, 2, 3, 7, 2,
            10, 7, 8, 7, 7, 5, 7, 5, 7, 8, 10, 8, 8, 7, 2, 8,
            3, 4, 4, 3, 3, 6, 8, 3, 4, 11, 10, 6, 7, 5, 5, 8,
            12, 13, 13, 12, 12, 8, 8, 3, 4, 5, 9, 14, 14, 14, 14, 14,
            14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 8, 9, 15, 11,
            12, 8, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14, 14,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 9,
            7, 7, 7, 8, 6, 6, 10, 7, 8, 8, 7, 8, 8, 7, 7, 7,
            7, 7, 8, 7, 8, 8, 8, 7, 8, 9, 7, 8, 6, 7, 8, 7,
            7, 6, 6, 5, 7, 6, 6, 8, 6, 6, 6, 6, 7, 6, 6, 6,
            6, 6, 6, 6, 6, 8, 6, 7, 6, 8, 9, 8, 8, 6, 6, 7,
            6, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0,
            0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0
        };
    }
}
