namespace LiveSplit.Celeste {
	partial class SplitterSplitSettings {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplitterSplitSettings));
			this.btnRemove = new System.Windows.Forms.Button();
			this.ToolTips = new System.Windows.Forms.ToolTip(this.components);
			this.picHandle = new System.Windows.Forms.PictureBox();
			this.cboType = new System.Windows.Forms.ComboBox();
			this.txtLevel = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.picHandle)).BeginInit();
			this.SuspendLayout();
			// 
			// btnRemove
			// 
			this.btnRemove.Image = ((System.Drawing.Image)(resources.GetObject("btnRemove.Image")));
			this.btnRemove.Location = new System.Drawing.Point(344, 1);
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.Size = new System.Drawing.Size(26, 23);
			this.btnRemove.TabIndex = 4;
			this.ToolTips.SetToolTip(this.btnRemove, "Remove this setting");
			this.btnRemove.UseVisualStyleBackColor = true;
			// 
			// picHandle
			// 
			this.picHandle.Cursor = System.Windows.Forms.Cursors.SizeAll;
			this.picHandle.Image = ((System.Drawing.Image)(resources.GetObject("picHandle.Image")));
			this.picHandle.Location = new System.Drawing.Point(3, 4);
			this.picHandle.Name = "picHandle";
			this.picHandle.Size = new System.Drawing.Size(20, 20);
			this.picHandle.TabIndex = 5;
			this.picHandle.TabStop = false;
			this.picHandle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.picHandle_MouseDown);
			this.picHandle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picHandle_MouseMove);
			// 
			// cboType
			// 
			this.cboType.FormattingEnabled = true;
			this.cboType.Location = new System.Drawing.Point(25, 3);
			this.cboType.Name = "cboType";
			this.cboType.Size = new System.Drawing.Size(313, 21);
			this.cboType.TabIndex = 0;
			this.cboType.SelectedIndexChanged += new System.EventHandler(this.cboType_SelectedIndexChanged);
			this.cboType.Validating += new System.ComponentModel.CancelEventHandler(this.cboType_Validating);
			// 
			// txtLevel
			// 
			this.txtLevel.Location = new System.Drawing.Point(238, 3);
			this.txtLevel.Name = "txtLevel";
			this.txtLevel.Size = new System.Drawing.Size(100, 20);
			this.txtLevel.TabIndex = 6;
			this.txtLevel.Visible = false;
			// 
			// SplitterSplitSettings
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.txtLevel);
			this.Controls.Add(this.cboType);
			this.Controls.Add(this.btnRemove);
			this.Controls.Add(this.picHandle);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "SplitterSplitSettings";
			this.Size = new System.Drawing.Size(373, 28);
			((System.ComponentModel.ISupportInitialize)(this.picHandle)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		public System.Windows.Forms.Button btnRemove;
		private System.Windows.Forms.ToolTip ToolTips;
		private System.Windows.Forms.PictureBox picHandle;
		public System.Windows.Forms.ComboBox cboType;
		public System.Windows.Forms.TextBox txtLevel;
	}
}
