using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NotepadSharpEditor2017New
{
    public partial class Find : Form
    {
        RichTextBox richText;
        public Find()
        {
            InitializeComponent();
        }

        public Find(RichTextBox rc)
        {
            InitializeComponent();
            richText = rc;

        }


        public static void FindWord(RichTextBox rtb, String word, Color color)
        {
            if (word == "")
            {
                return;
            }
            int s_start = rtb.SelectionStart, startIndex = 0, index;
            while ((index = rtb.Text.IndexOf(word, startIndex)) != -1)
            {
                rtb.Select(index, word.Length);
                rtb.SelectionColor = color;
                startIndex = index + word.Length;
            }
            rtb.SelectionStart = s_start;
            rtb.SelectionLength = 0;
            rtb.SelectionColor = Color.Black;
        }

        private void Find_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindWord(richText, textBox1.Text, Color.Gray);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
