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
    class ScriptExtractor
    {
        public static string[] nums = new string[10] { "z", "o", "tw", "th", "fo", "fi", "si", "se", "e", "n" };

        public static char[] engwords = {'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
            '!', '?', '.', '(', ')', ':', ',', '/', '*', "'".ToCharArray()[0],
            '+', '%', '&', '-', '"', '[', ']', '$', '#', '>', '<', '=','■', 'é', '●', ';', ' ', 'ï', '♥' };

        public static char[] specialWords = { 'ç', '♪', '☆', '●', '_', 'â' };

        public static char[] FrenchWords = {'ç', '♪', '☆', '●', '_', 'â', '×', '★', '~', '@', '¥', 'ᴱ', 'ᴿ', 'À', 'Á', 'Â', 'Ä', 'Æ', 'Ç', 'È', 'É', 'Ê', 'Ë', 'Ì', 'Í', 'Î', 'Ï', 'Ñ', 'Ò', 'Ó', 'Ô', 'Ö',
            'Ù', 'Ú', 'Û', 'Ü', 'ß', 'à', 'á', 'ä', 'æ', 'è', 'ê', 'ë', 'ì', 'í', 'î', 'ñ', 'ò', 'ó', 'ô', 'ö', 'ù', 'ú', 'û', 'ü', 'Œ', 'œ', '¡', '«', '°', '»', '¿', '€', 'Ã', 'Õ', 'ã', 'õ'};

        public static char[] rusWords = { 'А','Б','В','Г','Д','Е','Ё','Ж','З','И','Й','К','Л','М','Н','О','П','Р','С','Т','У','Ф','Х','Ц','Ч','Ш','Щ','Ъ','Ы','Ь','Э','Ю','Я','а','б','в','г','д','е','ё','ж','з','и','й','к','л','м','н',
                'о','п','р','с','т','у','ф','х','ц','ч','ш','щ','ъ','ы','ь','э','ю','я' };
        public static string[] common_nums = new string[]
        {
            "89, ", "63, ", "64, ", "65, ", "66, ", "67, ", "68, ", "69, ", "70, ", "71, ", "72, ",
            "73, ", "74, ", "75, ", "76, ", "77, ", "80, "
        };

        public static string[] common_symbs = new string[]
        {
            " ", "!", "?", ".", "(", ")", ":", ",", "/", "*", "'",
            "+", "%", "&", "-", "" + '"', "$"
        };
        public static string SpaceUNKs(string st)
        {
            while (true)
            {
                int x = st.IndexOf("[unk: ") + 6;
                if (x == 5)
                {
                    break;
                }
                int z = st.IndexOf("]");
                string sod = st.Substring(x, z - x);
                st = st.Remove(x - 6, sod.Length + 7);
                string tmp = ("[unk: " + sod + "{nk}").Replace(" ", "{spct}").Replace(",", "{zapx}");
                for (int i = 0; i < 10; i++)
                {
                    tmp = tmp.Replace("" + i, "{num" + nums[i] + "}");
                }
                st = st.Insert(x - 6, tmp);
            }
            st = st.Replace("{nk}", "]");
            return st;
        }

        public static string SpaceBMVS(string st)
        {
            while (true)
            {
                int x = st.IndexOf("<bmv: ") + 6;
                if (x == 5)
                {
                    break;
                }
                int z = st.IndexOf(">");
                string sod = st.Substring(x, z - x);
                st = st.Remove(x - 6, sod.Length + 7);
                string tmp = ("<bmv: " + sod + "{st}").Replace(" ", "{spct}").Replace(",", "{zapx}");
                for (int i = 0; i < 10; i++)
                {
                    tmp = tmp.Replace("" + i, "{num" + nums[i] + "}");
                }
                st = st.Insert(x - 6, tmp);
            }
            st = st.Replace("{st}", ">");
            return st;
        }
        public static string ChangeBMVS(string st)
        {
            while (true)
            {
                int x = st.IndexOf("<bmv: ") + 6;
                if (x == 5)
                {
                    break;
                }
                int z = st.IndexOf(">");
                string sod = st.Substring(x, z - x);
                st = st.Remove(x - 6, sod.Length + 7);
                st = st.Insert(x - 6, "255, " + sod);
            }
            return st;
        }

        public static string FindComps(string st)
        {
            char[] sym = st.ToCharArray();
            string res = "";
            bool ins = false;
            for (int i = 0; i < st.Length; i++)
            {
                
                if (sym[i] == '<')
                {
                    if (st.Length - i < 5)
                    {
                        res += "{less}";
                        continue;
                    }
                    if (sym[i + 1] == 'b' && sym[i + 2] == 'm' && sym[i + 3] == 'v' && sym[i + 4] == ':' || (sym[i + 1] == 'e' && sym[i + 2] == 'n' && sym[i + 3] == 'd' && (sym[i + 4] == 'l' || sym[i + 4] == '>')))
                        ins = true;
                    else
                    {
                        res += "{less}";
                        continue;
                    }
                } else if (sym[i] == '>')
                {
                    if (ins)
                        ins = false;
                    else
                    {
                        res += "{more}";
                        continue;
                    }
                }
                res += sym[i];
            }
            return res;
        }

        public static string ConvertNumbers(string st)
        {
            for (int i = 9; i > -1; i--)
            {
                st = st.Replace("" + i, "" + (i + 1) + ", ");
                st = st.Replace("{num" + nums[i] + "}", "" + i);
            }
            st = st.Replace("2, 1, , ", "10, ");
            return st;
        }

        public static string ChangeToCode(string st)
        {
            st = FindComps(st);
            st = st.Replace("<endl>", "{endl}");
            st = st.Replace("<end>", "{end}");
            st = SpaceUNKs(st);
            st = SpaceBMVS(st);
            st = st.Replace(",", "{zap}");
            st = st.Replace(" ", "{spc}");
            st = ConvertNumbers(st);
            st = st.Replace("{spc}", "89, ");
            st = st.Replace("{spct}", " ");
            st = st.Replace("{zapx}", ",");
            st = st.Replace("[unk: ", "");
            st = st.Replace("]", "");
            st = ChangeBMVS(st);
            st = st.Replace("{less}", "<");
            st = st.Replace("{more}", ">");
            st = st.Replace("{zap}", "69, ");
            st = st.Replace("{endl}", "254, ");
            st = st.Replace("{end}", "0 ");
            st = Translate(st, engwords, 11);
            st = Translate(st, specialWords, 109);
            st = Translate(st, rusWords, 160);
            return st;
        }

        public static string ChangeToCodeEu(string st)
        {
            st = FindComps(st);
            st = st.Replace("<endl>", "{endl}");
            st = st.Replace("<end>", "{end}");
            st = SpaceUNKs(st);
            st = SpaceBMVS(st);
            st = st.Replace(",", "{zap}");
            st = st.Replace(" ", "{spc}");
            st = ConvertNumbers(st);
            st = st.Replace("{spc}", "89, ");
            st = st.Replace("{spct}", " ");
            st = st.Replace("{zapx}", ",");
            st = st.Replace("[unk: ", "");
            st = st.Replace("]", "");
            st = ChangeBMVS(st);
            st = st.Replace("{less}", "<");
            st = st.Replace("{more}", ">");
            st = st.Replace("{zap}", "69, ");
            st = st.Replace("{endl}", "254, ");
            st = st.Replace("{end}", "0 ");
            st = Translate(st, engwords, 11);
            st = Translate(st, FrenchWords, 109);
            return st;
        }

        public static string CodeToSymb(string st)
        {
            if (st == "0 ")
            {
                st = "0";
            }
            else
            {
                st = st.Remove(st.Length - 2, 2);
            }
            int t = 0;
            t = Int32.Parse(st);
            if (t == 0)
            {
                st = "<end>";
            }
            else if (t < 11)
            {
                st = "" + (t - 1);
            }
            else if (t < 11 + engwords.Length)
            {
                st = "" + engwords[(t - 11)];
            }
            else if (t < 109 + specialWords.Length && t >= 109)
            {
                st = "" + specialWords[(t - 109)];
            }
            else if (t < 160 + rusWords.Length && t >= 160)
            {
                st = "" + rusWords[(t - 160)];
            }
            else
            {
                st = "[unk: " + st + ", ]";
            }
            return st;
        }

        public static string CodeToSymbEu(string st)
        {
            if (st == "0 ")
            {
                st = "0";
            }
            else
            {
                st = st.Remove(st.Length - 2, 2);
            }
            int t = 0;
            t = Int32.Parse(st);
            if (t == 0)
            {
                st = "<end>";
            }
            else if (t < 11)
            {
                st = "" + (t - 1);
            }
            else if (t < 11 + engwords.Length)
            {
                st = "" + engwords[(t - 11)];
            }
            else if (t < 109 + FrenchWords.Length && t >= 109)
            {
                st = "" + FrenchWords[(t - 109)];
            }
            else
            {
                st = "[unk: " + st + ", ]";
            }
            return st;
        }

        public static string Translate(string st, char[] arr, int plus)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i] == ',' || arr[i] == ' ')
                    continue;
                st = st.Replace("" + arr[i], "" + (i + plus) + ", ");
            }
            return st;
        }
        static string[] MakeToStart(string[] origstrings)
        {
            char otst = '	';
            for (Int64 i = 0; i < origstrings.Length; i++)
            {
                origstrings[i] = origstrings[i].Replace("" + otst, "");
            }
            return origstrings;
        }

        public static string[] Debug;
        static string[] TranslateFile(string[] lines)
        {
            string[] vs = new string[10];
            for (int i = 0; i < 10; i++)
            {
                vs[i] = "" + i + ", ";
            }
            for (Int64 i = 0; i < lines.Length; i++)
            {
                lines[i] = lines[i].Replace("byte.MaxValue,", "255,");
            }
            bool inSc = false;
            string[] tmp = new string[3] { "", "", "" };
            int ind = 0;
            char[] stnums = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (Int64 i = 0; i < lines.Length; i++)
            {
                if (i != 0)
                {
                    if (lines[i - 1].ToUpper().Contains("MSG") && lines[i - 1].Contains(" = new") && lines[i - 1].Contains("byte[]") && lines[i] == "{" && !inSc)
                    {
                        if (Array.IndexOf(stnums, lines[i + 1].ToCharArray()[0]) != -1)
                        {
                            inSc = true;
                            ind = 0;
                        }
                        continue;
                    }
                }
                if ((lines[i] == "};" || lines[i] == "}" || lines[i] == "},") && inSc)
                {
                    for (int j = 0; j <= ind; j++)
                    {
                        lines[i - ind - 1 + j] = tmp[j];
                    }
                    try
                    {
                        tmp[0] = "" + tmp[0];
                    }
                    catch
                    {
                        MessageBox.Show(lines[i -1] + "\nTranslate");
                    }
                    Debug = tmp;
                    tmp = new string[4] { "", "", "", ""};
                    inSc = false;
                    continue;
                }
                if (inSc)
                {
                    if (lines[i] == "254,")
                    {
                        lines[i] = "";
                        tmp[ind] = tmp[ind] + "<endl>";
                        ind++;
                        continue;
                    }
                    if (lines[i] == "255,")
                    {
                        byte[] bmvs = new byte[] {
                            0, 0, 1, 1, 1, 1, 1, 1, 2, 2, 1, 1, 0, 0, 4, 9, 3, 14, 0, 0, 1, 1,
                            1, 4, 3, 1, 0, 0, 0, 1, 0, 1, 1, 2, 2, 0, 0, 3, 1, 1
                        };
                        char[] t = lines[i + 1].ToCharArray();
                        string s = "";
                        for (int j = 0; j < t.Length - 1; j++)
                        {
                            s = s + t[j];
                        }
                        int nf = Int32.Parse(s);
                        int am = 0;
                        try
                        {
                            am = bmvs[nf];
                        }
                        catch
                        {
                            Console.WriteLine("The new funcnum was found: " + lines[i + 1]);
                            return null;
                        }
                        if (ind == 3)
                            MessageBox.Show(tmp[0] + "\n" + tmp[1] + "\n" + tmp[2]);
                        tmp[ind] += "<bmv: ";
                        for (int j = 0; j <= am; j++)
                        {
                            tmp[ind] += lines[i + 1 + j] + " ";
                            lines[i + 1 + j] = "";
                        }
                        tmp[ind] += ">";
                        lines[i] = "";
                        continue;
                    }
                    if (lines[i] == "") continue;
                    else
                    {
                        lines[i] = lines[i] + " ";
                        if (Array.IndexOf(common_nums, lines[i]) != -1)
                        {
                            tmp[ind] = tmp[ind] + common_symbs[Array.IndexOf(common_nums, lines[i])];
                        }
                        else if (Array.IndexOf(vs, lines[i]) != -1)
                        {
                            tmp[ind] = tmp[ind] + (Array.IndexOf(vs, lines[i]) - 1);
                        }
                        else
                        {
                            try
                            {
                                tmp[ind] = tmp[ind] + CodeToSymbEu(lines[i]);
                            } catch
                            {
                                MessageBox.Show(lines[i - 1] + " " + i);
                            }
                        }
                    }
                    lines[i] = "";
                }
            }
            string[] res = new string[lines.Length];
            ind = 0;
            for (Int64 i = 0; i < lines.Length; i++)
            {
                if (i != 0)
                {
                    if (lines[i - 1].ToUpper().Contains("MSG") && lines[i - 1].Contains(" = new") && lines[i - 1].Contains("byte[]") && lines[i] == "{" && !inSc)
                    {
                        if (lines[i + 1] == "")
                        {
                            inSc = true;
                        }
                    }
                    else if ((lines[i] == "};" || lines[i] == "}" || lines[i] == "},") && inSc)
                    {
                        inSc = false;
                    }
                    else if (inSc && lines[i] == "")
                    {
                        continue;
                    }
                }
                res[ind] = lines[i];
                ind++;
            }
            string[] ans = new string[ind];
            for (Int64 i = 0; i < ind; i++)
            {
                ans[i] = res[i];
            }
            return ans;
        }

        public static string[] ToCodeAllText(string[] text)
        {
            List<string> ls = new List<string>();
            for (int i = 0; i < text.Length; i++)
            {
                ls.Add(text[i]);
                if (i == 0)
                {
                    continue;
                }
                if (text[i - 1].ToUpper().Contains("MSG") && text[i - 1].Contains("byte[]") && text[i - 1].Contains(" = new") && text[i] == "{")
                {
                    int z = 0;
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if ((text[j] == "};" || text[j] == "}" || text[j] == "},"))
                        {
                            break;
                        }
                        z++;
                    }
                    string[] tt = new string[z];
                    for (int j = 0; j < z; j++)
                    {
                        tt[j] = text[i + j + 1];
                    }
                    if (CheckIftheText(tt, text[i - 1]))
                    {
                        for (int j = 0; j < z; j++)
                        {
                            //ChangeToCode(tt[j]);MakeLessernums(tt[j]);
                            byte[] bs = ToBytes(text[i + 1 + j]);
                            for (int k = 0; k < bs.Length; k++)
                            {
                                if (j == z - 1 && k == bs.Length - 1)
                                {
                                    ls.Add("" + bs[k]);
                                    continue;
                                }
                                ls.Add("" + bs[k] + ",");
                            }
                        }
                        i += z;
                    }
                }
            }
            return ls.ToArray();
        }

        public static string MakeLessernums(string st)
        {
            char[] symbs = st.ToCharArray();
            string rt = "";
            bool ins = false;
            char[] numss = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            for (int i = 0; i < symbs.Length; i++)
            {
                if (symbs[i] == '<')
                {
                    ins = true;
                }
                else if (symbs[i] == '>')
                {
                    ins = false;
                }
                else if (Array.IndexOf(numss, symbs[i]) != -1 && !ins)
                {
                    if (symbs[i] == '1' && symbs[i + 1] == '0')
                    {
                        symbs[i] = '9';
                        i++;
                    }
                    else if (symbs[i] == '0')
                    {
                        Console.WriteLine(st);
                    }
                    else
                    {
                        symbs[i] = numss[Array.IndexOf(numss, symbs[i]) - 1];
                    }
                }
                rt += symbs[i];
            }
            st = rt;
            return st;
        }

        static bool CheckIftheText(string[] tex, string lastpart)
        {
            for (int i = 0; i < tex.Length; i++)
            {
                if (Array.IndexOf(tex[i].ToCharArray(), '{') != -1)
                {
                    return false;
                }
            }
            if (tex == null)
            {
                return false;
            }
            char[] re = tex[tex.Length - 1].ToCharArray();
            if (re.Length < 5)
            {
                return false;
            }
            if ((re[re.Length - 1] == '>' && re[re.Length - 2] == ' ' && re[re.Length - 3] == '0' && re[re.Length - 4] == ' ' && re[re.Length - 5] == ',') || lastpart.ToUpper().Contains("MSG"))
                return true;
            return false;
        }

        public static string[] GotTrans(string path)
        {
            string[] file = File.ReadAllLines(path);
            file = ToCodeAllText(file);
            return file;
        }

        public static string[] GotNOTrans(string path)
        {
            string[] file = MakeToStart(File.ReadAllLines(path));
            file = TranslateFile(file);
            return file;
        }

        public static byte[] ToBytes(string st)
        {
            string tmp = st;
            string[] nums = ChangeToCodeEu(st).Replace(", ", " ").Split(' ');
            byte[] del = new byte[nums.Length - 1];
            for (int j = 0; j < del.Length; j++)
            {
                try
                {
                    del[j] = byte.Parse(nums[j]);
                }
                catch
                {
                    MessageBox.Show(tmp + "\nToBytes");//File.WriteAllText("C:\\AAI Script Editor\\AAIScriptEditor - RUS\\Новый текстовый документ (2).txt", nums[j] + st);
                }
            }
            return del;
        }

        public static byte[] ToBytes(string[] sts)
        {
            try
            {
                int fullen = 0;
                byte[][] ar = new byte[sts.Length][];
                for (int i = 0; i < ar.Length; i++)
                {
                    ar[i] = ToBytes(sts[i]);
                    fullen += ar[i].Length;
                }
                byte[] res = new byte[fullen];
                int ind = 0;
                for (int i = 0; i < ar.Length; i++)
                {
                    for (int j = 0; j < ar[i].Length; j++)
                    {
                        res[ind++] = ar[i][j];
                    }
                }
                return res;
            } catch
            {
                return null;
            }
        }

        public static string RemoveBMVS(string st)
        {
            st = FindComps(st);
            while (true)
            {
                int x = st.IndexOf("<bmv: ");
                if (x == -1)
                {
                    break;
                }
                int z = st.IndexOf(">");
                st = st.Remove(x, z - x + 1);
            }
            st = st.Replace("{less}", "<");
            st = st.Replace("{more}", ">");
            return st;
        }

        public static string[] RemoveBMVS(string[] st)
        {
            for (int i = 0; i < st.Length; i++)
            {
                st[i] = RemoveBMVS(st[i]);
            }
            return st;
        }

        public static string[] GetDialogue(string[] text, int ind)
        {
            int len = 0;
            string[] lastdialogue = null;
            int fin = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0)
                    continue;
                if (text[i - 1].ToUpper().Contains("MSG") && text[i - 1].Contains("byte[]") && text[i - 1].Contains(" = new") && text[i] == "{")
                {
                    int z = 0;
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (text[j] == "};" || text[j] == "}" || text[j] == "},")
                        {
                            break;
                        }
                        z++;
                    }
                    string[] tt = new string[z];
                    for (int j = 0; j < z; j++)
                    {
                        tt[j] = text[i + j + 1];
                    }
                    if (CheckIftheText(tt, text[i - 1]))
                    {
                        lastdialogue = tt;
                        fin = z;
                    }
                }
                len += text[i].Length + 2;
                if (len >= ind)
                    return lastdialogue;
            }
            return null;
        }

        public static string[] GetMessage(string[] text, int ind)
        {
            int st = -1;
            for (int i = ind; i > 0; i--)
            {
                if (text[i - 1].ToUpper().Contains("MSG") && text[i - 1].Contains("byte[]") && text[i - 1].Contains(" = new") && text[i] == "{")
                {
                    st = i;
                    break;
                }
            }
            if (st == -1)
                return null;
            int z = 0;
            for (int j = st + 1; j < text.Length; j++)
            {
                if (text[j] == "};" || text[j] == "}" || text[j] == "},")
                {
                    break;
                }
                z++;
            }
            if (z == 0 || z > 10)
            {
                return null;
            }
            string[] tt = new string[z];
            for (int j = 0; j < z; j++)
            {
                tt[j] = text[st + j + 1];
            }
            if (CheckIftheText(tt, text[st - 1]))
            {
                return tt;
            }
            return null;
        }

        public static bool IsRight(string[] text, int ind)
        {
            int len = 0;
            string[] lastdialogue = null;
            int fin = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (i == 0)
                    continue;
                if (text[i - 1].ToUpper().Contains("MSG") && text[i - 1].Contains(" = new") && text[i - 1].Contains("byte[]") && text[i] == "{")
                {
                    int z = 0;
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        if (text[j] == "};" || text[j] == "}" || text[j] == "},")
                        {
                            break;
                        }
                        z++;
                    }
                    string[] tt = new string[z];
                    for (int j = 0; j < z; j++)
                    {
                        tt[j] = text[i + j + 1];
                    }
                    if (CheckIftheText(tt, text[i - 1]))
                    {
                        lastdialogue = tt;
                        fin = i + z;
                    }
                }
                len += text[i].Length + 2;
                if (len >= ind && i <= fin)
                    return true;
                else if (len >= ind && i > fin)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
