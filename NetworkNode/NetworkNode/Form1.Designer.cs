namespace NetworkNode
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.chatListBox = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // chatListBox
            // 
            this.chatListBox.FormattingEnabled = true;
            this.chatListBox.Location = new System.Drawing.Point(12, 12);
            this.chatListBox.Name = "chatListBox";
            this.chatListBox.Size = new System.Drawing.Size(425, 238);
            this.chatListBox.TabIndex = 0;
            this.chatListBox.SelectedIndexChanged += new System.EventHandler(this.chatListBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 262);
            this.Controls.Add(this.chatListBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Form1";
            this.Text = "NetworkNode";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox chatListBox;
    }
}

