using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AAIScriptEditor.Properties;
using System.IO;
using System.Threading;

namespace AAIScriptEditor
{
    public partial class Form1 : Form
    {
        string[] filecode;
        string[] textfile;
        bool loaded = false;
        int sel = 0;
        MessageDrawer md;
        string[][] changes = new string[30][];
        int savedcount = 0;
        int selbacks = 0;
        int numoffound = 0;
        bool infinding = false;
        int[] fs = null;
        string scpath;
        Thread myThread = null;
        bool closed = false;
        Graphics gr;
        private Stack<string[]> _editingHistory = new Stack<string[]>();
        private Stack<string[]> _undoHistory = new Stack<string[]>();
        bool t = false;
        public Form1()
        {
            InitializeComponent();
            saveToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            editToolStripMenuItem.Enabled = false;
            centerAllToolStripMenuItem.Enabled = false;
            this.FormClosing += OnClosed;
            groupBox1.Enabled = false;
            центрированиеDSToolStripMenuItem.Enabled = false;
        }

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            numoffound = 0;
            selbacks = 0;
            savedcount = 0;
            changes = new string[30][];
            sel = 0;
            loaded = false;
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Открыть файл сценария";
                openFileDialog.Filter = "Текстовый файл (*.txt, *.cs)|*.txt;*.cs|Любой файл (*.*)|*.*";
                openFileDialog.FilterIndex = 1;
                DialogResult dr = openFileDialog.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    //Get the path of specified file
                    scpath = openFileDialog.FileName;
                } else if (dr == DialogResult.Cancel)
                {
                    //Get the path of specified file
                    return;
                }
            }
            filecode = File.ReadAllLines(scpath);
            textfile = ScriptExtractor.GotNOTrans(scpath);
            pictureBox1.CreateGraphics().DrawImage(Resources.MessageWindow, 0, 0, 1024 / MessageDrawer.MASCHTAB, 250 / MessageDrawer.MASCHTAB);
            gr = pictureBox1.CreateGraphics();
            fs = null;
            textBox1.Lines = textfile;
            textBox1.ScrollBars = ScrollBars.Vertical;
            if (myThread != null)
            {
                myThread.Abort();
            }
            if (!loaded)
            {
                myThread = new Thread(new ThreadStart(Func));
                myThread.Start(); // запускаем поток
            }
            loaded = true;
            changes[changes.Length - 1] = textfile;
            savedcount++;
            saveToolStripMenuItem.Enabled = true;
            saveAsToolStripMenuItem.Enabled = true;
            editToolStripMenuItem.Enabled = true;
            undoToolStripMenuItem.Enabled = false;
            redoToolStripMenuItem.Enabled = false;
            centerAllToolStripMenuItem.Enabled = true;
            groupBox1.Enabled = true;
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            button5.Enabled = false;
            textBox1.Click += On_Click;
            md = new MessageDrawer();
            _editingHistory.Push(textBox1.Lines);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!loaded)
            {
                return;
            }
            if (infinding)
            {
                ResetFinding();
            }
            if (selbacks != 0)
            {
                Refresh();
            }
            string res = "";
            //new Thread(new ThreadStart(UpdateChanges)).Start();
            new Thread(new ThreadStart(ShowMessage)).Start();
            /*for (int i = 0; i < bytes.Length; i++)
            {
                res += "" + bytes[i] + "\n";
            }
            MessageBox.Show(res);*/
        }

        private void On_Click(object sender, EventArgs e)
        {
            if (textBox1.SelectionStart != sel)
            {
                sel = textBox1.SelectionStart;
                if (!loaded)
                {
                    return;
                }
                ShowMessage();
            }
        }

        public async void AutoSave()
        {
            while (true)
            {
                Thread.Sleep(600000);
                if (!closed)
                    File.WriteAllLines(scpath.Insert(scpath.Length - 4, "auto"), ScriptExtractor.ToCodeAllText(textBox1.Lines));
            }
        }

        public async void Func()
        {
            Thread thr = new Thread(AutoSave);
            while (true)
            {
                if (closed)
                {
                    thr.Abort();
                    break;
                } else if (!t)
                {
                    t = true;
                    thr.Start();
                }
            }
        }

        public void Refresh() {
            for (int i = changes.Length - 1; i >= selbacks; i--)
            {
                changes[i] = changes[i - selbacks];
            }
            for (int i = 0; i < selbacks; i++)
            {
                changes[i] = null;
            }
            savedcount -= selbacks;
            redoToolStripMenuItem.Enabled = false;
            selbacks = 0;
        }

        async void ShowMessage()
        {
            string[] dial = ScriptExtractor.GetDialogue(textBox1.Lines, textBox1.SelectionStart);
            //string[] dial = ScriptExtractor.GetMessage(textBox1.Lines, textBox1.GetLineFromCharIndex(textBox1.SelectionStart));
            if (dial == null)
                return;
            byte[] bytes = ScriptExtractor.ToBytes(ScriptExtractor.RemoveBMVS(dial));
            if (bytes == null)
                return;
            md.DrawMessage(bytes, gr);
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _undoHistory.Push(_editingHistory.Pop());
            redoToolStripMenuItem.Enabled = true;
            textBox1.Lines = _editingHistory.Peek();
            undoToolStripMenuItem.Enabled = _editingHistory.Count > 1;
            ShowMessage();
        }

        async void UpdateChanges()
        {
            loaded = false;
            _editingHistory.Push(textBox1.Lines);
            undoToolStripMenuItem.Enabled = true;
            _undoHistory.Clear();
            redoToolStripMenuItem.Enabled = false;
            loaded = true;
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _editingHistory.Push(_undoHistory.Pop());
            redoToolStripMenuItem.Enabled = _undoHistory.Count > 0;
            textBox1.Lines = _editingHistory.Peek();
            undoToolStripMenuItem.Enabled = true;
            ShowMessage();
        }

        public int[] Find(string seek)
        {
            infinding = true;
            List<int> sels = new List<int>();
            string[] text = textBox1.Lines;
            int len = 0;
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i].Contains(seek))
                {
                    sels.Add(len + text[i].IndexOf(seek));
                }
                len += text[i].Length + 2;
            }
            return sels.ToArray();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int[] sels = Find(textBox2.Text);
            if (sels == null || sels.Length == 0)
            {
                MessageBox.Show("Совпадения не найдены");
                return;
            }
            for (int i = 0; i < sels.Length; i++)
            {
                if (sels[i] >= textBox1.SelectionStart)
                {
                    GotoFound(sels, i);
                    break;
                }
            }
            fs = sels;
            button4.Enabled = true;
        }

        void GotoFound(int[] sels, int ind)
        {
            textBox1.SelectionStart = sels[ind];
            numoffound = ind;
            if (numoffound >= sels.Length - 1 && numoffound != 0)
            {
                button3.Enabled = true;
                button2.Enabled = false;
            }
            else if (numoffound >= sels.Length - 1 && numoffound == 0)
            {
                button3.Enabled = false;
                button2.Enabled = false;
            }
            else if (numoffound == 0)
            {
                button2.Enabled = true;
                button3.Enabled = false;
            } else
            {
                button2.Enabled = true;
                button3.Enabled = true;
            }
            textBox1.Focus();
            textBox1.ScrollToCaret();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numoffound++;
            GotoFound(fs, numoffound);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            numoffound--;
            GotoFound(fs, numoffound);
        }

        void ResetFinding()
        {
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            numoffound = 0;
            fs = null;
            infinding = false;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            int cur = -1;
            if (!fs.Contains(textBox1.SelectionStart))
            {
                button1_Click(sender, e);
            } else
            {
                string[] text = textBox1.Lines;
                for (int i = 0; i < text.Length; i++)
                {
                    if (text[i].Contains(textBox2.Text))
                    {
                        cur++;
                        if (cur == numoffound)
                        {
                            text[i] = text[i].Replace(textBox2.Text, textBox3.Text);
                            loaded = false;
                            textBox1.Lines = text;
                            loaded = true;
                            textBox1.SelectionStart = fs[numoffound];
                            button4.Enabled = false;
                            button1_Click(sender, e);
                            break;
                        }
                    }
                }
            }
            UpdateChanges();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int selc = textBox1.SelectionStart;
            string[] text = textBox1.Lines;
            for (int i = 0; i < text.Length; i++)
            {
                text[i] = text[i].Replace(textBox2.Text, textBox3.Text);
            }
            loaded = false;
            textBox1.Lines = text;
            loaded = true;
            textBox1.SelectionStart = selc;
            textBox1.Focus();
            textBox1.ScrollToCaret();
            UpdateChanges();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            button1.Enabled = textBox2.Text != null || textBox2.Text != "";
            button5.Enabled = textBox2.Text != null || textBox2.Text != "";
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllLines(scpath, ScriptExtractor.ToCodeAllText(textBox1.Lines));
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.RestoreDirectory = true;
                sfd.Title = "Сохранить файл сценария как";
                sfd.Filter = "Текстовый файл (*.txt, *.cs)|*.txt;*.cs|Без расширения (*.*)|*.*";
                sfd.FilterIndex = 1;
                DialogResult dr = sfd.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    //Get the path of specified file
                    scpath = sfd.FileName;
                }
                else if (dr == DialogResult.Cancel)
                {
                    //Get the path of specified file
                    return;
                }
            }
            File.WriteAllLines(scpath, ScriptExtractor.ToCodeAllText(textBox1.Lines));
        }

        private void centerAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Lines = md.CenterAll(textBox1.Lines);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void OnClosed(object sender, FormClosingEventArgs e)
        {
            closed = true;
            string caption = "Сохраниться";
            string message = "Хотите ли вы сохраниться?";
            DialogResult result = MessageBox.Show(message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                File.WriteAllLines(scpath, ScriptExtractor.ToCodeAllText(textBox1.Lines));
            }
        }

        private void центрированиеDSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            textBox1.Lines = md.CenterAllDS(textBox1.Lines);
        }
    }
}
