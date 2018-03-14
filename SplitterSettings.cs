using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
namespace LiveSplit.Celeste {
	public partial class SplitterSettings : UserControl {
		public List<SplitInfo> Splits { get; private set; }
		public bool ILSplits;
		private bool isLoading;
		public SplitterSettings() {
			isLoading = true;
			InitializeComponent();

			Splits = new List<SplitInfo>();
			ILSplits = false;

			isLoading = false;
		}

		private void Settings_Load(object sender, EventArgs e) {
			FindForm().Text = "Celeste Autosplitter v" + Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
			LoadSettings();
		}
		public void LoadSettings() {
			isLoading = true;
			this.flowMain.SuspendLayout();

			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				flowMain.Controls.RemoveAt(i);
			}

			foreach (SplitInfo split in Splits) {
				SplitterSplitSettings setting = new SplitterSplitSettings();
				setting.cboType.DataSource = GetAvailableDescriptions<SplitType>();
				setting.cboType.Text = SplitterSplitSettings.GetEnumDescription<SplitType>(split.Type);
				setting.txtLevel.Text = split.Value;
				AddHandlers(setting);

				flowMain.Controls.Add(setting);
			}

			isLoading = false;
			this.flowMain.ResumeLayout(true);
		}
		private void AddHandlers(SplitterSplitSettings setting) {
			setting.cboType.SelectedIndexChanged += new EventHandler(ControlChanged);
			setting.txtLevel.TextChanged += new EventHandler(ControlChanged);
			setting.btnRemove.Click += new EventHandler(btnRemove_Click);
		}
		private void RemoveHandlers(SplitterSplitSettings setting) {
			setting.cboType.SelectedIndexChanged -= ControlChanged;
			setting.txtLevel.TextChanged -= ControlChanged;
			setting.btnRemove.Click -= btnRemove_Click;
		}
		public void btnRemove_Click(object sender, EventArgs e) {
			for (int i = flowMain.Controls.Count - 1; i > 0; i--) {
				if (flowMain.Controls[i].Contains((Control)sender)) {
					RemoveHandlers((SplitterSplitSettings)((Button)sender).Parent);

					flowMain.Controls.RemoveAt(i);
					break;
				}
			}
			UpdateSplits();
		}
		public void ControlChanged(object sender, EventArgs e) {
			UpdateSplits();
		}
		public void UpdateSplits() {
			if (isLoading) return;

			int chapterCount = 0;
			int heartCount = 0;
			Splits.Clear();
			foreach (Control control in flowMain.Controls) {
				if (control is SplitterSplitSettings) {
					SplitterSplitSettings setting = (SplitterSplitSettings)control;
					if (!string.IsNullOrEmpty(setting.cboType.Text)) {
						SplitType type = SplitterSplitSettings.GetEnumValue<SplitType>(setting.cboType.Text);
						Splits.Add(new SplitInfo() {
							Type = type,
							Value = setting.txtLevel.Text
						});
						if (type.ToString().Length == 8) {
							chapterCount++;
						} else if(type.ToString().IndexOf("HeartGem", StringComparison.OrdinalIgnoreCase) > 0) {
							heartCount++;
						}
					}
				}
			}
			ILSplits = chapterCount == 1 && heartCount <= 1;
		}
		public XmlNode UpdateSettings(XmlDocument document) {
			XmlElement xmlSettings = document.CreateElement("Settings");

			XmlElement xmlSplits = document.CreateElement("Splits");
			xmlSettings.AppendChild(xmlSplits);

			foreach (SplitInfo split in Splits) {
				XmlElement xmlSplit = document.CreateElement("Split");
				xmlSplit.InnerText = split.ToString();

				xmlSplits.AppendChild(xmlSplit);
			}

			return xmlSettings;
		}
		public void SetSettings(XmlNode settings) {
			int chapterCount = 0;
			Splits.Clear();
			XmlNodeList splitNodes = settings.SelectNodes(".//Splits/Split");
			foreach (XmlNode splitNode in splitNodes) {
				string splitDescription = splitNode.InnerText;
				SplitInfo split = new SplitInfo(splitDescription);
				Splits.Add(split);
				if (split.Type.ToString().Length == 8) {
					chapterCount++;
				}
			}
			ILSplits = chapterCount == 1;
		}
		private void btnAddSplit_Click(object sender, EventArgs e) {
			SplitterSplitSettings setting = new SplitterSplitSettings();
			List<string> names = GetAvailableDescriptions<SplitType>();
			setting.cboType.DataSource = names;
			setting.cboType.Text = names[0];
			AddHandlers(setting);

			flowMain.Controls.Add(setting);
			UpdateSplits();
		}
		private void btnChapterSplits_Click(object sender, EventArgs e) {
			if (Splits.Count > 0 && MessageBox.Show(this, "You already have some splits setup. This will clear anything you have and default in the Chapter splits.\r\n\r\nAre you sure you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
				return;
			}

			Splits.Clear();
			Splits.Add(new SplitInfo() { Type = SplitType.Prologue });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7 });

			LoadSettings();
		}
		private void btnChapterCheckpointSplits_Click(object sender, EventArgs e) {
			if (Splits.Count > 0 && MessageBox.Show(this, "You already have some splits setup. This will clear anything you have and default in the Chapter and Checkpoint splits.\r\n\r\nAre you sure you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
				return;
			}

			Splits.Clear();
			Splits.Add(new SplitInfo() { Type = SplitType.Prologue });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3Checkpoint3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4Checkpoint3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5Checkpoint3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5Checkpoint4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6Checkpoint3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6Checkpoint4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6Checkpoint5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7Checkpoint6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7 });

			LoadSettings();
		}
		private void btnABCSides_Click(object sender, EventArgs e) {
			if (Splits.Count > 0 && MessageBox.Show(this, "You already have some splits setup. This will clear anything you have and default in the Chapter and Checkpoint splits.\r\n\r\nAre you sure you want to continue?", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) {
				return;
			}

			Splits.Clear();
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter8 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter8 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter1 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter2 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter3 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter4 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter5 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter6 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter7 });
			Splits.Add(new SplitInfo() { Type = SplitType.Chapter8 });

			LoadSettings();
		}
		private List<string> GetAvailableDescriptions<T>() where T : struct {
			List<string> values = new List<string>();
			foreach (T value in Enum.GetValues(typeof(T))) {
				string name = value.ToString();
				MemberInfo[] infos = typeof(T).GetMember(name);
				DescriptionAttribute[] descriptions = null;
				if (infos != null && infos.Length > 0) {
					descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
				}
				if (descriptions != null && descriptions.Length > 0) {
					values.Add(descriptions[0].Description);
				} else {
					values.Add(name);
				}
			}
			return values;
		}
		private void flowMain_DragDrop(object sender, DragEventArgs e) {
			UpdateSplits();
		}
		private void flowMain_DragEnter(object sender, DragEventArgs e) {
			e.Effect = DragDropEffects.Move;
		}
		private void flowMain_DragOver(object sender, DragEventArgs e) {
			SplitterSplitSettings data = (SplitterSplitSettings)e.Data.GetData(typeof(SplitterSplitSettings));
			FlowLayoutPanel destination = (FlowLayoutPanel)sender;
			Point p = destination.PointToClient(new Point(e.X, e.Y));
			var item = destination.GetChildAtPoint(p);
			int index = destination.Controls.GetChildIndex(item, false);
			if (index == 0) {
				e.Effect = DragDropEffects.None;
			} else {
				e.Effect = DragDropEffects.Move;
				int oldIndex = destination.Controls.GetChildIndex(data);
				if (oldIndex != index) {
					destination.Controls.SetChildIndex(data, index);
					destination.Invalidate();
				}
			}
		}
	}
}