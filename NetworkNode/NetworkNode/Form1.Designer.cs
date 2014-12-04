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
            this.configButton = new System.Windows.Forms.Button();
            this.connectCloudButton = new System.Windows.Forms.Button();
            this.disconnectCloudButton = new System.Windows.Forms.Button();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.logsListView = new System.Windows.Forms.ListView();
            this.col1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.connectManagerButton = new System.Windows.Forms.Button();
            this.disconnectManagerButton = new System.Windows.Forms.Button();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // configButton
            // 
            this.configButton.Location = new System.Drawing.Point(12, 270);
            this.configButton.Name = "configButton";
            this.configButton.Size = new System.Drawing.Size(117, 23);
            this.configButton.TabIndex = 2;
            this.configButton.Text = "Load Configuration";
            this.configButton.UseVisualStyleBackColor = true;
            this.configButton.Click += new System.EventHandler(this.configButton_Click);
            // 
            // connectCloudButton
            // 
            this.connectCloudButton.Enabled = false;
            this.connectCloudButton.Location = new System.Drawing.Point(135, 270);
            this.connectCloudButton.Name = "connectCloudButton";
            this.connectCloudButton.Size = new System.Drawing.Size(150, 23);
            this.connectCloudButton.TabIndex = 3;
            this.connectCloudButton.Text = "Connect to cloud";
            this.connectCloudButton.UseVisualStyleBackColor = true;
            this.connectCloudButton.Click += new System.EventHandler(this.cloudConnect_Click);
            // 
            // disconnectCloudButton
            // 
            this.disconnectCloudButton.Enabled = false;
            this.disconnectCloudButton.Location = new System.Drawing.Point(290, 270);
            this.disconnectCloudButton.Name = "disconnectCloudButton";
            this.disconnectCloudButton.Size = new System.Drawing.Size(150, 23);
            this.disconnectCloudButton.TabIndex = 4;
            this.disconnectCloudButton.Text = "Disconnect from cloud";
            this.disconnectCloudButton.UseVisualStyleBackColor = true;
            this.disconnectCloudButton.Click += new System.EventHandler(this.disconnectCloud_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 326);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(449, 22);
            this.statusStrip.TabIndex = 5;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(48, 17);
            this.statusLabel.Text = "Inactive";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "Config.xml";
            this.openFileDialog.Filter = "Config (*.xml)|*.xml|All files (*.*)|*.*";
            this.openFileDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog_FileOk);
            // 
            // logsListView
            // 
            this.logsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.col1});
            this.logsListView.Location = new System.Drawing.Point(12, 12);
            this.logsListView.Name = "logsListView";
            this.logsListView.Size = new System.Drawing.Size(428, 252);
            this.logsListView.TabIndex = 6;
            this.logsListView.UseCompatibleStateImageBehavior = false;
            this.logsListView.View = System.Windows.Forms.View.Details;
            // 
            // col1
            // 
            this.col1.Text = "Logs";
            this.col1.Width = 425;
            // 
            // connectManagerButton
            // 
            this.connectManagerButton.Enabled = false;
            this.connectManagerButton.Location = new System.Drawing.Point(135, 299);
            this.connectManagerButton.Name = "connectManagerButton";
            this.connectManagerButton.Size = new System.Drawing.Size(150, 23);
            this.connectManagerButton.TabIndex = 7;
            this.connectManagerButton.Text = "Connect to manager";
            this.connectManagerButton.UseVisualStyleBackColor = true;
            this.connectManagerButton.Click += new System.EventHandler(this.connectManagerButton_Click);
            // 
            // disconnectManagerButton
            // 
            this.disconnectManagerButton.Enabled = false;
            this.disconnectManagerButton.Location = new System.Drawing.Point(290, 299);
            this.disconnectManagerButton.Name = "disconnectManagerButton";
            this.disconnectManagerButton.Size = new System.Drawing.Size(150, 23);
            this.disconnectManagerButton.TabIndex = 8;
            this.disconnectManagerButton.Text = "Disconnect from manager";
            this.disconnectManagerButton.UseVisualStyleBackColor = true;
            this.disconnectManagerButton.Click += new System.EventHandler(this.disconnectManagerButton_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 348);
            this.Controls.Add(this.disconnectManagerButton);
            this.Controls.Add(this.connectManagerButton);
            this.Controls.Add(this.logsListView);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.disconnectCloudButton);
            this.Controls.Add(this.connectCloudButton);
            this.Controls.Add(this.configButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "NetworkNode - Unknown";
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button configButton;
        private System.Windows.Forms.Button connectCloudButton;
        private System.Windows.Forms.Button disconnectCloudButton;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ListView logsListView;
        private System.Windows.Forms.ColumnHeader col1;
        private System.Windows.Forms.Button connectManagerButton;
        private System.Windows.Forms.Button disconnectManagerButton;
    }
}

