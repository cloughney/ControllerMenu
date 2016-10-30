using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControllerMenu
{
	public class MenuItem : Label
	{
		private readonly string title;
		private readonly Point location;

		private float defaultFontSize = 36;
		private bool isSelected;

		public MenuItem(string title, Point location)
		{
			this.title = title;
			this.location = location;

			this.isSelected = false;
		}

		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				var fontSize = (float) (value ? this.defaultFontSize * 1.33 : this.defaultFontSize);
				this.Font = new Font(this.Font.FontFamily, fontSize);
				this.isSelected = value;
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.ForeColor = Color.White;
			this.defaultFontSize = this.Font.SizeInPoints;

			this.Text = this.title;
			this.Location = this.location;
			this.AutoSize = true;
		}
	}
}