using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
namespace LiveSplit.Celeste {
	public partial class SplitterSplitSettings : UserControl {
		private int mX = 0;
		private int mY = 0;
		private bool isDragging = false;
		public SplitterSplitSettings() {
			InitializeComponent();
			cboType.SelectedItem = SplitType.Manual;
		}
		private void cboType_Validating(object sender, CancelEventArgs e) {
			string item = GetItemInList(cboType);
			if (string.IsNullOrEmpty(item)) {
				cboType.SelectedItem = SplitType.Manual;
			} else {
				cboType.SelectedItem = GetEnumValue<SplitType>(item);
			}
		}
		private void cboType_SelectedIndexChanged(object sender, EventArgs e) {
			string splitDescription = cboType.SelectedValue.ToString();
			SplitType split = GetEnumValue<SplitType>(splitDescription);
			if (split != SplitType.LevelExit && split != SplitType.LevelEnter) {
				txtLevel.Visible = false;
				txtLevel.Text = string.Empty;
				cboType.Size = new System.Drawing.Size(313, 21);
			} else {
				cboType.Size = new System.Drawing.Size(207, 21);
				txtLevel.Visible = true;
			}
			txtLevel.Tag = txtLevel.Visible;

			MemberInfo[] infos = typeof(SplitType).GetMember(split.ToString());
			DescriptionAttribute[] descriptions = null;
			if (infos.Length > 0) {
				descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			}
			if (descriptions != null && descriptions.Length > 0) {
				ToolTips.SetToolTip(cboType, descriptions[0].Description);
			} else {
				ToolTips.SetToolTip(cboType, string.Empty);
			}
		}
		public static T GetEnumValue<T>(string text) {
			foreach (T item in Enum.GetValues(typeof(T))) {
				string name = item.ToString();

				MemberInfo[] infos = typeof(T).GetMember(name);
				DescriptionAttribute[] descriptions = null;
				if (infos.Length > 0) {
					descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
				}
				DescriptionAttribute description = descriptions != null && descriptions.Length > 0 ? descriptions[0] : null;

				if (name.Equals(text, StringComparison.OrdinalIgnoreCase) || (description != null && description.Description.Equals(text, StringComparison.OrdinalIgnoreCase))) {
					return item;
				}
			}
			return default(T);
		}
		public static string GetEnumDescription<T>(T item) {
			string name = item.ToString();

			MemberInfo[] infos = typeof(T).GetMember(name);
			DescriptionAttribute[] descriptions = null;
			if (infos.Length > 0) {
				descriptions = (DescriptionAttribute[])infos[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
			}
			DescriptionAttribute description = descriptions != null && descriptions.Length > 0 ? descriptions[0] : null;

			return description == null ? name : description.Description;
		}
		private string GetItemInList(ComboBox cbo) {
			string item = cbo.Text;
			for (int i = cbo.Items.Count - 1; i >= 0; i += -1) {
				object ob = cbo.Items[i];
				if (ob.ToString().Equals(item, StringComparison.OrdinalIgnoreCase)) {
					return ob.ToString();
				}
			}
			return null;
		}
		private void picHandle_MouseMove(object sender, MouseEventArgs e) {
			if (!isDragging) {
				if (e.Button == MouseButtons.Left) {
					int num1 = mX - e.X;
					int num2 = mY - e.Y;
					if (((num1 * num1) + (num2 * num2)) > 20) {
						DoDragDrop(this, DragDropEffects.All);
						isDragging = true;
						return;
					}
				}
			}
		}
		private void picHandle_MouseDown(object sender, MouseEventArgs e) {
			mX = e.X;
			mY = e.Y;
			isDragging = false;
		}
	}
	public class ToolTipAttribute : Attribute {
		public string ToolTip { get; set; }
		public ToolTipAttribute(string text) {
			ToolTip = text;
		}
	}
}