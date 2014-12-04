namespace NetworkCloud
{
    partial class NetworkCloud
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkCloud));
            this.label1 = new System.Windows.Forms.Label();
            this.configButton = new System.Windows.Forms.Button();
            this.startButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.linksListView = new System.Windows.Forms.ListView();
            this.id = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.node_in = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.node_out = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.port_in = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.port_out = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.logsListView = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Links";
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(12, 368);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(261, 23);
            this.configButton.TabIndex = 2;
            this.configButton.Text = "Load Configuration";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // startButton
            // 
            this.startButton.Enabled = false;
            this.startButton.Location = new System.Drawing.Point(279, 368);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(258, 23);
            this.startButton.TabIndex = 3;
            this.startButton.Text = "Start";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 397);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(549, 22);
            this.statusStrip.TabIndex = 6;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(48, 17);
            this.statusLabel.Text = "Inactive";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "Config (*.xml)|*.xml|All Files (*.*)|*.*";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // linksListView
            // 
            this.linksListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.id,
            this.node_in,
            this.node_out,
            this.port_in,
            this.port_out});
            this.linksListView.Enabled = false;
            this.linksListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.linksListView.Location = new System.Drawing.Point(12, 25);
            this.linksListView.Name = "linksListView";
            this.linksListView.Size = new System.Drawing.Size(525, 189);
            this.linksListView.TabIndex = 8;
            this.linksListView.UseCompatibleStateImageBehavior = false;
            this.linksListView.View = System.Windows.Forms.View.Details;
            // 
            // id
            // 
            this.id.Text = "ID";
            this.id.Width = 40;
            // 
            // node_in
            // 
            this.node_in.Text = "Node IN";
            this.node_in.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.node_in.Width = 140;
            // 
            // node_out
            // 
            this.node_out.Text = "Node OUT";
            this.node_out.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.node_out.Width = 140;
            // 
            // port_in
            // 
            this.port_in.Text = "Port IN";
            this.port_in.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.port_in.Width = 100;
            // 
            // port_out
            // 
            this.port_out.Text = "Port OUT";
            this.port_out.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.port_out.Width = 100;
            // 
            // logsListView
            // 
            this.logsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.logsListView.Enabled = false;
            this.logsListView.Location = new System.Drawing.Point(12, 220);
            this.logsListView.Name = "logsListView";
            this.logsListView.Size = new System.Drawing.Size(525, 142);
            this.logsListView.TabIndex = 9;
            this.logsListView.UseCompatibleStateImageBehavior = false;
            this.logsListView.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Logs";
            this.columnHeader1.Width = 525;
            // 
            // NetworkCloud
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(549, 419);
            this.Controls.Add(this.logsListView);
            this.Controls.Add(this.linksListView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.startButton);
            this.Controls.Add(this.configButton);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "NetworkCloud";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NetworkCloud";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.NetworkCloud_FormClosing);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListView linksListView;
        private System.Windows.Forms.ColumnHeader id;
        private System.Windows.Forms.ColumnHeader node_in;
        private System.Windows.Forms.ColumnHeader node_out;
        private System.Windows.Forms.ColumnHeader port_in;
        private System.Windows.Forms.ColumnHeader port_out;
        private System.Windows.Forms.ListView logsListView;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}

