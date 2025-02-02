using System;
using System.Drawing;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Printing;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using System.Text.RegularExpressions;
using System.Timers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace NotepadSharpEditor2017New
{
    public partial class Shareditor : Form
    {
        PrintDocument prntDoc = new PrintDocument();
        public FontDialog fontDialog = new FontDialog();
        OpenFileDialog openFileDialog = new OpenFileDialog();
        SaveFileDialog savingTheFile = new SaveFileDialog();
        string[] fileNameGlobals = new string[50000];
        private System.Timers.Timer AutoTimer;
        List<Task> closeTasks = new List<Task>();
        public TaskScheduler uiScheduler;
        PrintDocument printing = new PrintDocument();
        TabPage tabpage;
        private Task DictionaryTasking;
        public FontDialog fd = new FontDialog();
        RichTextBox richTextBox3;
        PrintDocument document = new PrintDocument();
        PrintDialog dialog = new PrintDialog();
        RichTextBox richTextBox2;
        List<string> WordList = new List<string>();
        private ConcurrentBag<string> dictionaryStringBag;
   

        public Shareditor()
        {           
            InitializeComponent();
            newFile();
            myTabControlZ.TabIndex = 7;
            myTabControlZ.Select();           
            tabpage = myTabControlZ.SelectedTab;           
            getRichTextBox(tabpage).DragDrop += new DragEventHandler(richTextBox2_DragDrop);
            getRichTextBox(tabpage).VScroll += new System.EventHandler(this.richTextBox2_VScroll);
            document.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            getRichTextBox(tabpage).AllowDrop = true;
        }

        void richTextBox2_DragDrop(object sender, DragEventArgs e)
        {
            object filename = e.Data.GetData("FileDrop");
            if (filename != null)
            {
                var list = filename as string[];

                if (list != null && !string.IsNullOrWhiteSpace(list[0]))
                {
                    
                    getRichTextBox(tabpage).Clear();
                    Parallel.Invoke(() => {
                        getRichTextBox(tabpage).LoadFile(list[0], RichTextBoxStreamType.PlainText);
                        checkthespellings();
                    });
                }
             }
        } 
        
        private void panel3_Paint(object sender, PaintEventArgs e)
        {}

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {}

        private string ReadingAFile(string file)
        {
           StreamReader reader = new StreamReader(this.openFileDialog.FileName);
           string data = reader.ReadToEnd();
           reader.Close();
           return data;
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                OpeningUpFunction();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        public void OpeningUpFunction()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => OpeningUpFunction()));
            }
            else
            {
                Stream streaming = null;
                openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    if ((streaming = openFileDialog.OpenFile()) != null)
                    {
                       
                            string filename = openFileDialog.FileName;
                            string readfiletext = File.ReadAllText(filename);
                            string[] filepath = filename.Split('\\');
                            string tabTitle = filepath[filepath.Length - 1];
                            RichTextBox RichTextBo = createRichTextBox1();
                            RichTextBo.Text = readfiletext;
                            RichTextBo.SelectionStart = RichTextBo.Text.Length;
                            TabPage tabs = new TabPage(tabTitle);
                            tabs.Controls.Add(RichTextBo);
                            myTabControlZ.TabPages.Add(tabs);
                            myTabControlZ.SelectedTab = tabs;
                            tabs.Tag = filename;
                            checkthespellings();


                    }
                    streaming.Close();
                }
            }
        }

    

        public void AutoSaving(object source, ElapsedEventArgs e)
        {
            if (this.InvokeRequired)
            {
               this.Invoke(new MethodInvoker(() => AutoSaving(source, e)));
            }
            else
            {
                TabControl.TabPageCollection tabs = myTabControlZ.TabPages;
                try
                {
                    Parallel.ForEach(Partitioner.Create(0, tabs.Count), (range) =>
                    {
                        for (int i = range.Item1; i < range.Item2; i++)
                        {
                            TabPage tab = tabs[i];
                            if (tab.Tag != null)
                            {
                                getRichTextBox(tabpage).BeginInvoke((MethodInvoker)delegate { getRichTextBox(tabpage).SaveFile((string)tab.Tag, RichTextBoxStreamType.PlainText); });
                            }
                        }
                    });
                }
                catch (AggregateException ae)
                {
                    foreach (Exception innerEx in ae.InnerExceptions) { }
                }
            }
        }


        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task saveTheFile = Task.Factory.StartNew(() => { 
                saveFile(tabpage);
                Debugger.Break();
            });
        }

        

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task saveAsTheText = Task.Factory.StartNew(() => {
                SaveAS(tabpage);
            });
        }

        public void SaveAS(TabPage tp)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => SaveAS(tp)));
            }
            else
            {
                SaveFileDialog savefileDialog = new SaveFileDialog();
                savefileDialog.Filter = "*.txt|*.txt";
                savefileDialog.FileName = "*.txt";
                if (savefileDialog.ShowDialog() == DialogResult.OK)
                {
                    Parallel.Invoke(() => { 
                    getRichTextBox(tp).SaveFile(savefileDialog.FileName, RichTextBoxStreamType.PlainText);
                    getRichTextBox(tp).Focus();
                    string[] filepath = savefileDialog.FileName.Split('\\');
                    string tabTitle = filepath[filepath.Length - 1];
                    tp.Text = tabTitle;
                    tp.Tag = savefileDialog.FileName;
                    });
                }
            }
        }
        
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Undo();
     
        }

        private void closeAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            closeAll();
            Task.Factory.ContinueWhenAll(closeTasks.ToArray(), exit => Application.Exit());
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Cut();
         
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
               Clipboard.SetText(getRichTextBox(tabpage).SelectedText, TextDataFormat.Rtf);
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Redo();

       }

        private void pastToolStripMenuItem_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Paste();

        }

        private void sellectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            getRichTextBox(tabpage).SelectAll();
         }


        public void dictionaryRunsOnApp()
        {
            string dictionarybook = Properties.Resources.allwordsText;
            List<string> allwords = dictionarybook.Split(new[]
            { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).ToList();
            dictionaryStringBag = new ConcurrentBag<string>();  
            foreach (string word in allwords)
            { dictionaryStringBag.Add(word); }
        }


        public void checkthespellings()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => checkthespellings()));
            }
            else
            {
                List<string> words = getRichTextBox(tabpage).Text.Split().ToList();
                int startIndex = 0;
                foreach (string word in words)
                {
                    Parallel.Invoke(() => { 
					
                    int cursorPosition = getRichTextBox(tabpage).SelectionStart;
                    int index = getRichTextBox(tabpage).Text.IndexOf(word, startIndex);
                    getRichTextBox(tabpage).Select(index, word.Length);
                    Regex regEx = new Regex("^[a-zA-Z]*$");
                    if (dictionaryStringBag.Contains(word) || !regEx.IsMatch(word))
                    {
                        getRichTextBox(tabpage).SelectionColor = Color.Black;
                    }
                    else {
                        getRichTextBox(tabpage).SelectionColor = Color.Red;
                    }
                    getRichTextBox(tabpage).SelectionStart = cursorPosition;
                    startIndex = index + word.Length;
                    });

                }
            }
        }

        private void italicToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ItalicTheText();
        }

        public void ItalicTheText()
        {
            Font oldFont = getRichTextBox(tabpage).SelectionFont;
            FontStyle newFontStyle = new FontStyle();
            if (getRichTextBox(tabpage).SelectionFont.Italic == true)
            {
                newFontStyle = oldFont.Style ^ FontStyle.Italic;
            }
            else
            {
                newFontStyle = oldFont.Style | FontStyle.Italic;
            }
            getRichTextBox(tabpage).SelectionFont = new Font(oldFont.FontFamily, oldFont.Size, newFontStyle);
        }


        private void fontBoldToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BoldingTheText();
        }


        public void BoldingTheText()
        {
            Font oldFont = getRichTextBox(tabpage).SelectionFont;
            FontStyle newFontStyle = new FontStyle();
            if (getRichTextBox(tabpage).SelectionFont.Bold == true)
            {
                newFontStyle = oldFont.Style ^ FontStyle.Bold;
            }
            else
            {
                newFontStyle = oldFont.Style | FontStyle.Bold;
            }
            getRichTextBox(tabpage).SelectionFont = new Font(oldFont.FontFamily, oldFont.Size, newFontStyle);
        }


        private void underlineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UnderlineTheText();
        }

        public void UnderlineTheText()
        {
            Font oldFont = getRichTextBox(tabpage).SelectionFont;
            FontStyle newFontStyle = new FontStyle();
            if (getRichTextBox(tabpage).SelectionFont.Underline == true)
            {
                newFontStyle = oldFont.Style ^ FontStyle.Underline;
            }
            else
            {
                newFontStyle = oldFont.Style | FontStyle.Underline;
            }
            getRichTextBox(tabpage).SelectionFont = new Font(oldFont.FontFamily, oldFont.Size, newFontStyle);
        }

        private void fontColourToolStripMenuItem_Click(object sender, EventArgs e)
        {}

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {}

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("The Sharpeditor created by Thanuka Senadheera.ID - Number : cb005937 ,APIIT Sri Lanka.", "About author");
        }

        private void zoomINToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        public void ZoomIn()
        {
            float zoom = getRichTextBox(tabpage).ZoomFactor;
            if (zoom * 2 < 64)
                getRichTextBox(tabpage).ZoomFactor = zoom * 2;
        }

        private void richTextBox2_MouseUp(object sender, MouseEventArgs e)
        {}

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            //CountWords
            var t1 = Task.Factory.StartNew(CountWords);
            var t2 = Task.Factory.StartNew(calculateLineNumber);
            var t3 = Task.Factory.StartNew(calculatewordlength);

     

            //Debugger.Break();

            //if (getRichTextBox(tabpage).Text == "")
            //{
            //    AddLineNumbers();
           
            //}



        }

        public void calculatewordlength()
        {
            try {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => calculatewordlength()));
                }
                else {
                    lengthLabl.Text = getRichTextBox(tabpage).Text.Length.ToString();
                }
            }catch(Exception ex)
            {

            }
        }

        public void calculateLineNumber()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => calculateLineNumber()));
            }
            else
            {
                int index = getRichTextBox(tabpage).SelectionStart;
                int lineNumber = getRichTextBox(tabpage).GetLineFromCharIndex(index) + 1;
                //Retrieves the line number from the specified character position within the text of the control.
                lineno.Text = lineNumber.ToString();
            }
        }


        public void calculateLength()
        {
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();

            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => calculateLength()));
            }
            else {
                Task<string> task = Task.Run(() =>
                {      

                    return getRichTextBox(tabpage).Text.Length.ToString();
                
                }).ContinueWith(t => lengthLabl.Text = t.Result, taskScheduler);

            }

        }

        private void zoomOUTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        public void ZoomOut()
        {
            float zoom = getRichTextBox(tabpage).ZoomFactor;
            if (zoom / 2 > 0.015535)
                getRichTextBox(tabpage).ZoomFactor = zoom / 2;
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Help h1 = new Help();
            if (!h1.IsHandleCreated == true)
                {
                    h1.Show();
                }
            }

        private void preferenceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Preference p1 = new Preference();
            if (!p1.IsHandleCreated == true)
            {
                p1.Show();
            }
        }

        private void hideLengthToolStripMenuItem_Click(object sender, EventArgs e)
        {}


        private void fontSettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task fontSet = Task.Factory.StartNew(fontSettings);
        }


        public void fontSettings()
        {
            FontDialog FontChooser = new FontDialog();
            DialogResult result = FontChooser.ShowDialog();
            if (result == DialogResult.OK)
            {
                getRichTextBox(tabpage).Font = new Font(FontChooser.Font.FontFamily, 
                    FontChooser.Font.Size, FontChooser.Font.Style);
            }
            else
            {
                FontChooser.Reset();
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                search();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);   

        }


        public void search()
        {
            if (myTabControlZ.TabCount > 0)
            {
                var _myRichTextBox = getRichTextBox(tabpage);
                Find uu = new Find(_myRichTextBox);
                uu.Show();
            }
        }



        private void wrdcount_Click(object sender, EventArgs e)
        { }

        private void Shareditor_Load(object sender, EventArgs e)
        {

            //createRichTextBox2();
            //RichTextBox richBOxCreat2 = createRichTextBox2();
            //createRichTextBox2
            //myTabControlZ.Controls.Add(richBOxCreat2);

            DictionaryTasking = Task.Factory.StartNew(dictionaryRunsOnApp);
            uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();// which trying to schedule the tasks
            AutoTimer = new System.Timers.Timer(100);
            AutoTimer.Elapsed += AutoSaving;
            AutoTimer.Start();


            Task lineNumber = new Task(AddLineNumbers);
            lineNumber.Start();
            

        }


        //public void MethodCheck(object source, ElapsedEventArgs e)
        //{
        //    if (this.InvokeRequired)
        //    {
        //        this.Invoke(new MethodInvoker(() => MethodCheck(source, e)));
        //    }
        //    else
        //    {
        //        MessageBox.Show("Geting? huh?");
        //    }
        //}


        private void button1_Click(object sender, EventArgs e)
        {}

        public void CountWords()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => CountWords()));
            }
            else
            {
                var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
                string pattern = "[^\\w]";
                string input = getRichTextBox(tabpage).Text;
                string[] words = null;
                int i = 0;
                int count = 0;
                Console.WriteLine(input);
                words = Regex.Split(input, pattern, RegexOptions.IgnoreCase);
                for (i = words.GetLowerBound(0); i <= words.GetUpperBound(0); i++)
                {
                    if (words[i].ToString() == string.Empty)
                        count = count - 1;
                    count = count + 1;
                }
                //Debugger.Break();
                Task<string> task = Task.Run(() =>
                {
                    return count.ToString();
                }).ContinueWith(t => wrdcount.Text = t.Result, taskScheduler);


            }
        }
        
        private void button1_Click_1(object sender, EventArgs e)
        {}

        private void newTabToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
            newFile();
            //Debugger.Break();
        }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
      
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                Shareditor se = new Shareditor();
                se.Show();
                }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void richTextBox2_SelectionChanged(object sender, EventArgs e)
        {
            LineUp();
        }

        public void LineUp()
        {
          
                this.Invalidate();

                Point pt = getRichTextBox(tabpage).GetPositionFromCharIndex(getRichTextBox(tabpage).SelectionStart);

                if (pt.X == 1)
                {
                    Task adlo = new Task(AddLineNumbers);
                    adlo.Start();
                }
        }



        private void Shareditor_FormClosing(object sender, FormClosingEventArgs e)
        {

            e.Cancel = true;
            closeAll();
            Task.Factory.ContinueWhenAll(closeTasks.ToArray(), exit => Environment.Exit(0));
           
            
        }

        public void closeAll()
        {        
            TabControl.TabPageCollection tabs = myTabControlZ.TabPages;
            foreach (TabPage page in tabs)
            {
                Task close = Task.Factory.StartNew(() => {
                        TabPage tab = page;
                        closeTab(tab);
                    });
                closeTasks.Add(close);
                // Debugger.Break();
            }
        }

        public void closeTab(TabPage tab)
        {
            if (InvokeRequired)
            {Invoke(new MethodInvoker(() => closeTab(tab)));}
            else
            {
                if (tab.Tag == null)
                {
                    DialogResult dialogResult = MessageBox.Show(" Do you want to save this file? " 
                        + tab.Name, "Save", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                    if (dialogResult == DialogResult.Yes)
                    {   saveFile(tab);
                        removeTab(tab); }
                    else if (dialogResult == DialogResult.No)
                    {removeTab(tab);}
                }
                else
                {
                    getRichTextBox(tabpage).SaveFile((string)tab.Tag, RichTextBoxStreamType.PlainText);
                    removeTab(tab);
                }
            }
        }

        public void removeTab(TabPage tp)
        {
            if (myTabControlZ.TabPages.Count == 1)
            {
                newFile();
            }
            myTabControlZ.TabPages.Remove(tp);
        }


        public void newFile()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(() => newFile()));
            }
            else
            {
                Parallel.Invoke(() => { 
                    TabPage tabpage = new TabPage("SharpDoc");
                    RichTextBox richBOxCreat = createRichTextBox1();                  
                    //createRichTextBox2
                    tabpage.Controls.Add(richBOxCreat);
                    RichTextBox richBOxCreat2 = createRichTextBox2();
                    tabpage.Controls.Add(richBOxCreat2);
                    myTabControlZ.TabPages.Add(tabpage);
                    myTabControlZ.SelectedTab = tabpage;
                });
            }
        }




        public RichTextBox createRichTextBox1()
        {
            richTextBox2 = new RichTextBox();
            richTextBox2.BorderStyle = BorderStyle.None;
            richTextBox2.WordWrap = false;
            richTextBox2.Dock = DockStyle.Fill;
            richTextBox2.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);
            richTextBox2.SelectionChanged += new System.EventHandler(this.richTextBox2_SelectionChanged);
            richTextBox2.VScroll += new System.EventHandler(this.richTextBox2_VScroll);
                        document.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
            richTextBox2.DragDrop += new DragEventHandler(richTextBox2_DragDrop);
            richTextBox2.AllowDrop = true;
            richTextBox2.Font = new Font("Georgia", 12);
            richTextBox2.ShortcutsEnabled = true;
            richTextBox2.KeyDown += new KeyEventHandler(this.richTextBox2_KeyDown);
            richTextBox2.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedBoth;
            richTextBox2.Focus();
            return richTextBox2;
        }

       


        public RichTextBox createRichTextBox2()
        {
            richTextBox3 = new RichTextBox();
            richTextBox3.BorderStyle = BorderStyle.Fixed3D;
            richTextBox3.WordWrap = false;
            richTextBox3.Dock = DockStyle.Left;
            //richTextBox3.TextChanged += new System.EventHandler(this.richTextBox2_TextChanged);         
            richTextBox3.ReadOnly = true;
            richTextBox3.Font = new Font("Georgia", 12);
            richTextBox3.Width = getWidth();            
            richTextBox3.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
           // richTextBox3.Focus();
            return richTextBox3;
        }



        private void richTextBox2_VScroll(object sender, EventArgs e)
        {

            //richTextBox3.Text = "";
            //Task vScroll = new Task(AddLineNumbers);
            //vScroll.Start();
            //AddLineNumbers();
            //richTextBox3.Invalidate();
        }
        private void richTextBox2_FontChanged(object sender, EventArgs e)
        {

            //    richTextBox3.Font = richTextBox2.Font;
            ////Task vScroll = new Task();
            ////vScroll.Start();
            //AddLineNumbers();

        }

        private void richTextBox3_MouseDown(object sender, MouseEventArgs e)
        {
                richTextBox2.Select();
                richTextBox3.DeselectAll();  
        }

        public int getWidth()
        {
            int w = 25;
            int line = richTextBox3.Lines.Length;

            if (line <= 99)
            {
                w = 20 + (int)richTextBox3.Font.Size;
            }
            else if (line <= 999)
            {
                w = 30 + (int)richTextBox3.Font.Size;
            }
            else
            {
                w = 50 + (int)richTextBox3.Font.Size;
            }
            return w;
        }
        public void AddLineNumbers()
        {
            try {
                if (this.InvokeRequired)
                {
                    this.Invoke(new MethodInvoker(() => AddLineNumbers()));
                }
                else
                {
                   
                    //richTextBox2.Select();
                    // create & set Point pt to (0,0)
                    Point pt = new Point(0, 0);
                    // get First Index & First Line from richTextBox1
                    int First_Index = richTextBox2.GetCharIndexFromPosition(pt);
                    int First_Line = richTextBox2.GetLineFromCharIndex(First_Index);
                    pt.X = ClientRectangle.Width;
                    pt.Y = ClientRectangle.Height;
                    // get Last Index & Last Line from richTextBox1
                    int Last_Index = richTextBox2.GetCharIndexFromPosition(pt);
                    int Last_Line = richTextBox2.GetLineFromCharIndex(Last_Index);
                    // set Center alignment to created corner rich text box 3
                    richTextBox3.SelectionAlignment = HorizontalAlignment.Center;
                    // set created corner rich text box 3 text to null & width to getWidth() function value
                    richTextBox3.Text = "";
                    richTextBox3.Width = getWidth();
                    //now add each line number to rich text box 3 upto last line
                    for (int i = First_Line; i <= Last_Line + 1; i++)
                        {
                           richTextBox3.Text += i + 1 + "\n";
                        }
                }           
            }
            catch(Exception ex)
            {}
        }


        public void saveFile(TabPage tab)
        {
            if (this.InvokeRequired)
            {this.Invoke(new MethodInvoker(() => saveFile(tab)));}
            else
            {
                SaveFileDialog savefileDialog = new SaveFileDialog();
                savefileDialog.Filter = "*.NoteSharpTextRed|*.txt";
                if (tab.Tag == null)
                {
                    savefileDialog.FileName = "*.NoteSharpTextRed";
                    if (savefileDialog.ShowDialog() == DialogResult.OK)
                    {
                        getRichTextBox(tab).SaveFile(savefileDialog.FileName,
                        RichTextBoxStreamType.PlainText);
                        getRichTextBox(tab).Focus();
                        string[] filepath = savefileDialog.FileName.Split('\\');
                        string tabTitle = filepath[filepath.Length - 1];
                        tab.Text = tabTitle;
                        tab.Tag = savefileDialog.FileName;
                    }
                }
                else
                {
                    getRichTextBox(tab).SaveFile((string)tab.Tag, RichTextBoxStreamType.PlainText);
                }
            }            
        }

        public RichTextBox getRichTextBox(TabPage tp)
        {
            RichTextBox rtb = tp.Controls[0] as RichTextBox;
            return rtb;
        } 


        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {            
            var taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            if (e.KeyCode == Keys.Space)
            {
                Task spellcheck = DictionaryTasking.ContinueWith((t) => checkthespellings());
            }

            if (e.KeyCode == Keys.Enter)
            {
                Task spellcheck = DictionaryTasking.ContinueWith((t) => checkthespellings());
            }
        }

        private void Shareditor_InputLanguageChanging(object sender, InputLanguageChangingEventArgs e)
        {

        }

        private void myTabControlZ_SelectedIndexChanged(object sender, EventArgs e)
        {
                tabpage = myTabControlZ.SelectedTab;
                richTextBox2_TextChanged(sender, e);
                getRichTextBox(tabpage).Focus();    
        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                saveAllFunction();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }


        public void saveAllFunction()
        {
            TabControl.TabPageCollection tabs = myTabControlZ.TabPages;
            try
            {
                Parallel.ForEach(Partitioner.Create(0, tabs.Count), (range) =>
                {
                    for (int i = range.Item1; i < range.Item2; i++)
                    {
                        saveFile(tabs[i]);
                    }
                });
            }
            catch (AggregateException ae)
            {
                foreach (Exception innerEx in ae.InnerExceptions) { }
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                removeTab(tabpage);
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void aNSIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {

                //EnvryptionUTF32(tabpage);

                EncryptionAll();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
       
    


        
            
         
        }


        public void EnvryptionUTF32(TabPage tb)
        {           
                string plainText = getRichTextBox(tb).Text.ToString().Trim();
                var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
                string results = System.Convert.ToBase64String(plainTextBytes);
                getRichTextBox(tb).Text = results;
                encrypt.Text = "UTF64";      
        }
        

        public void EncryptionAll()
        {
            TabControl.TabPageCollection tabs = myTabControlZ.TabPages;
            try
            {
                Parallel.ForEach(Partitioner.Create(0, tabs.Count), (range) =>
                {
                    for (int i = range.Item1; i < tabs.Count + 1; i++)
                    {
                        EnvryptionUTF32(tabs[i]);
                    }
                });
            }
            catch (AggregateException ae)
            {
                foreach (Exception innerEx in ae.InnerExceptions) { }
            }
        }

		
		

        private void uTF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string utf8String = getRichTextBox(tabpage).Text;
            byte[] bytes = Encoding.Default.GetBytes(utf8String);
            String myString = Encoding.UTF8.GetString(bytes);
            getRichTextBox(tabpage).Text = myString;
            encrypt.Text = "UTF";
			
        }

        private void button1_Click_2(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                newFile();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void boldup_Click(object sender, EventArgs e)
        {
            BoldingTheText();
        }

        private void italicup_Click(object sender, EventArgs e)
        {
            ItalicTheText();
        }

        private void underlineup_Click(object sender, EventArgs e)
        {
            UnderlineTheText();
        }

        private void Aplus_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void Aminus_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void cutbut_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Cut();
        }

        private void pastebut_Click(object sender, EventArgs e)
        {
            getRichTextBox(tabpage).Paste();
        }

        private void savebut_Click(object sender, EventArgs e)
        {
            Task saveAsTheText = Task.Factory.StartNew(() => {
                SaveAS(tabpage);
            });
        }

        private void saveallbut_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                saveAllFunction();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void undobut_Click(object sender, EventArgs e)
        {

            getRichTextBox(tabpage).Undo();
        }

        private void redobut_Click(object sender, EventArgs e)
        {
            getRichTextBox(tabpage).Redo();
        }

        private void openup_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                OpeningUpFunction();
                //Debugger.Break();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        }

        private void myTabControlZ_DragDrop(object sender, DragEventArgs e)
        {
        
        }

        private void replaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                ReplaceMethod();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
    
        }


        public void ReplaceMethod()
        {
            if (myTabControlZ.TabCount > 0)
            {
                var replaceWord = getRichTextBox(tabpage);
                Replace replacForm = new Replace(replaceWord);
                replacForm.ShowDialog();
            }
        }

        private void printToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() => {
                document.Print();
                //Debugger.Break();
            }, CancellationToken.None, TaskCreationOptions.None, uiScheduler);
        
        }

        private void printPreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (myTabControlZ.TabCount > 0)
            {
                printPreviewDialog1.Document = printDocument1;
                printPreviewDialog1.ShowDialog();
            }

            //Debugger.Break();

        }


        
   

        
        private void printingAuthority(TabPage tab)
        {
            DialogResult dialogResult = MessageBox.Show(" Are you sure you want to Print this file? " + tab.Name, "Print", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (dialogResult == DialogResult.Yes)
            {
                saveFile(tab);
                removeTab(tab);
                prntDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printToolStripMenuItem_Click);
            }
            else if (dialogResult == DialogResult.No)
            { removeTab(tab); }



        }

        private void file_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {}

        private void file_Click(object sender, EventArgs e)
        {}

        private void panel1_Paint(object sender, PaintEventArgs e)
        { }

        private void uTF64ALLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Dycrpt();
        }

        public void Dycrpt()
        {
            string baseDycrypt = getRichTextBox(tabpage).Text.ToString().Trim();
            var base64EncodedBytes = System.Convert.FromBase64String(baseDycrypt);
            string dycrpted = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            getRichTextBox(tabpage).Text = dycrpted;
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawString(getRichTextBox(tabpage).Text, getRichTextBox(tabpage).Font, Brushes.Black, 20, 20);
        }
    }
    }
    

