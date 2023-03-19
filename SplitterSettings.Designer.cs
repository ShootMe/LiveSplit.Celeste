namespace LiveSplit.Celeste {
    partial class SplitterSettings {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.btnAddSplit = new System.Windows.Forms.Button();
            this.flowMain = new System.Windows.Forms.FlowLayoutPanel();
            this.flowOptions = new System.Windows.Forms.FlowLayoutPanel();
            this.chkAutoReset = new System.Windows.Forms.CheckBox();
            this.chkHighPriority = new System.Windows.Forms.CheckBox();
            this.btnChapterSplits = new System.Windows.Forms.Button();
            this.btnChapterCheckpointSplits = new System.Windows.Forms.Button();
            this.btnABCSides = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip();
            this.flowMain.SuspendLayout();
            this.flowOptions.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnAddSplit
            // 
            this.btnAddSplit.AutoSize = true;
            this.btnAddSplit.Location = new System.Drawing.Point(6, 6);
            this.btnAddSplit.Margin = new System.Windows.Forms.Padding(6);
            this.btnAddSplit.Name = "btnAddSplit";
            this.btnAddSplit.Size = new System.Drawing.Size(59, 23);
            this.btnAddSplit.TabIndex = 0;
            this.btnAddSplit.Text = "Add Split";
            this.btnAddSplit.UseVisualStyleBackColor = true;
            this.btnAddSplit.Click += new System.EventHandler(this.btnAddSplit_Click);
            // 
            // flowMain
            // 
            this.flowMain.AllowDrop = true;
            this.flowMain.AutoSize = true;
            this.flowMain.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowMain.Controls.Add(this.flowOptions);
            this.flowMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowMain.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowMain.Location = new System.Drawing.Point(0, 0);
            this.flowMain.Margin = new System.Windows.Forms.Padding(0);
            this.flowMain.Name = "flowMain";
            this.flowMain.Size = new System.Drawing.Size(874, 56);
            this.flowMain.TabIndex = 0;
            this.flowMain.WrapContents = false;
            this.flowMain.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowMain_DragDrop);
            this.flowMain.DragEnter += new System.Windows.Forms.DragEventHandler(this.flowMain_DragEnter);
            this.flowMain.DragOver += new System.Windows.Forms.DragEventHandler(this.flowMain_DragOver);
            // 
            // flowOptions
            // 
            this.flowOptions.AutoSize = true;
            this.flowOptions.Controls.Add(this.btnAddSplit);
            this.flowOptions.Controls.Add(this.chkAutoReset);
            this.flowOptions.Controls.Add(this.chkHighPriority);
            this.flowOptions.Controls.Add(this.btnChapterSplits);
            this.flowOptions.Controls.Add(this.btnChapterCheckpointSplits);
            this.flowOptions.Controls.Add(this.btnABCSides);
            this.flowOptions.Location = new System.Drawing.Point(0, 0);
            this.flowOptions.Margin = new System.Windows.Forms.Padding(0);
            this.flowOptions.Name = "flowOptions";
            this.flowOptions.Size = new System.Drawing.Size(874, 56);
            this.flowOptions.TabIndex = 0;
            // 
            // chkAutoReset
            // 
            this.chkAutoReset.Location = new System.Drawing.Point(126, 6);
            this.chkAutoReset.Margin = new System.Windows.Forms.Padding(6);
            this.chkAutoReset.Name = "chkAutoReset";
            this.chkAutoReset.Size = new System.Drawing.Size(192, 44);
            this.chkAutoReset.TabIndex = 5;
            this.chkAutoReset.TabStop = false;
            this.chkAutoReset.Text = "Auto Reset ILs";
            this.toolTip1.SetToolTip(this.chkAutoReset, "Reset IL splits if restarting or exiting the chapter (incl. save and quit)");
            this.chkAutoReset.UseVisualStyleBackColor = true;
            this.chkAutoReset.CheckedChanged += new System.EventHandler(this.ControlChanged);
            // 
            // chkHighPriority
            // 
            this.chkHighPriority.Location = new System.Drawing.Point(330, 6);
            this.chkHighPriority.Margin = new System.Windows.Forms.Padding(6);
            this.chkHighPriority.Name = "chkHighPriority";
            this.chkHighPriority.Size = new System.Drawing.Size(166, 44);
            this.chkHighPriority.TabIndex = 6;
            this.chkHighPriority.TabStop = false;
            this.chkHighPriority.Text = "High Priority";
            this.toolTip1.SetToolTip(this.chkHighPriority, "Set Celeste.exe\'s task priority to High");
            this.chkHighPriority.UseVisualStyleBackColor = true;
            this.chkHighPriority.CheckedChanged += new System.EventHandler(this.ControlChanged);
            // 
            // btnChapterSplits
            // 
            this.btnChapterSplits.Location = new System.Drawing.Point(508, 6);
            this.btnChapterSplits.Margin = new System.Windows.Forms.Padding(6);
            this.btnChapterSplits.Name = "btnChapterSplits";
            this.btnChapterSplits.Size = new System.Drawing.Size(108, 44);
            this.btnChapterSplits.TabIndex = 1;
            this.btnChapterSplits.TabStop = false;
            this.btnChapterSplits.Text = "Chapter";
            this.btnChapterSplits.UseVisualStyleBackColor = true;
            this.btnChapterSplits.Click += new System.EventHandler(this.btnChapterSplits_Click);
            // 
            // btnChapterCheckpointSplits
            // 
            this.btnChapterCheckpointSplits.Location = new System.Drawing.Point(628, 6);
            this.btnChapterCheckpointSplits.Margin = new System.Windows.Forms.Padding(6);
            this.btnChapterCheckpointSplits.Name = "btnChapterCheckpointSplits";
            this.btnChapterCheckpointSplits.Size = new System.Drawing.Size(140, 44);
            this.btnChapterCheckpointSplits.TabIndex = 2;
            this.btnChapterCheckpointSplits.TabStop = false;
            this.btnChapterCheckpointSplits.Text = "Checkpoint";
            this.btnChapterCheckpointSplits.UseVisualStyleBackColor = true;
            this.btnChapterCheckpointSplits.Click += new System.EventHandler(this.btnChapterCheckpointSplits_Click);
            // 
            // btnABCSides
            // 
            this.btnABCSides.Location = new System.Drawing.Point(780, 6);
            this.btnABCSides.Margin = new System.Windows.Forms.Padding(6);
            this.btnABCSides.Name = "btnABCSides";
            this.btnABCSides.Size = new System.Drawing.Size(88, 44);
            this.btnABCSides.TabIndex = 4;
            this.btnABCSides.TabStop = false;
            this.btnABCSides.Text = "A B C";
            this.btnABCSides.UseVisualStyleBackColor = true;
            this.btnABCSides.Click += new System.EventHandler(this.btnABCSides_Click);
            // 
            // toolTip1
            // 
            this.toolTip1.AutomaticDelay = 250;
            // 
            // SplitterSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.flowMain);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "SplitterSettings";
            this.Size = new System.Drawing.Size(874, 56);
            this.Load += new System.EventHandler(this.Settings_Load);
            this.flowMain.ResumeLayout(false);
            this.flowMain.PerformLayout();
            this.flowOptions.ResumeLayout(false);
            this.flowOptions.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnAddSplit;
        private System.Windows.Forms.FlowLayoutPanel flowMain;
        private System.Windows.Forms.FlowLayoutPanel flowOptions;
        private System.Windows.Forms.Button btnChapterSplits;
        private System.Windows.Forms.Button btnChapterCheckpointSplits;
        private System.Windows.Forms.Button btnABCSides;
        private System.Windows.Forms.CheckBox chkAutoReset;
        private System.Windows.Forms.CheckBox chkHighPriority;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}
