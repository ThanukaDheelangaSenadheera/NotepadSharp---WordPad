namespace NotepadSharpEditor2017New
{
    partial class Replace
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.findword = new System.Windows.Forms.TextBox();
            this.replaceword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.searchReplaceButton = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // findword
            // 
            this.findword.Location = new System.Drawing.Point(129, 13);
            this.findword.Name = "findword";
            this.findword.Size = new System.Drawing.Size(129, 20);
            this.findword.TabIndex = 0;
            this.findword.TextChanged += new System.EventHandler(this.findword_TextChanged);
            // 
            // replaceword
            // 
            this.replaceword.Location = new System.Drawing.Point(129, 73);
            this.replaceword.Name = "replaceword";
            this.replaceword.Size = new System.Drawing.Size(129, 20);
            this.replaceword.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Find Word :";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 73);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Replace Word :";
            // 
            // searchReplaceButton
            // 
            this.searchReplaceButton.Location = new System.Drawing.Point(22, 121);
            this.searchReplaceButton.Name = "searchReplaceButton";
            this.searchReplaceButton.Size = new System.Drawing.Size(115, 23);
            this.searchReplaceButton.TabIndex = 4;
            this.searchReplaceButton.Text = "Search and Replace";
            this.searchReplaceButton.UseVisualStyleBackColor = true;
            this.searchReplaceButton.Click += new System.EventHandler(this.searchReplaceButton_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(183, 121);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 5;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // Replace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(285, 170);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.searchReplaceButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.replaceword);
            this.Controls.Add(this.findword);
            this.Name = "Replace";
            this.Text = "Replace";
            this.Load += new System.EventHandler(this.Replace_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox findword;
        private System.Windows.Forms.TextBox replaceword;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button searchReplaceButton;
        private System.Windows.Forms.Button cancel;
    }
}