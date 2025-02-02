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
    public partial class Replace : Form
    {
        RichTextBox Sup;
        public Replace()
        {
            InitializeComponent();
        }

        public Replace(RichTextBox t)
        {
            InitializeComponent();
            Sup = t;

        }


        private void Replace_Load(object sender, EventArgs e)
        {}

        private void searchReplaceButton_Click(object sender, EventArgs e)
        {
            Sup.Text = Sup.Text.Replace(findword.Text, replaceword.Text);
        }

        private void cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void findword_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
