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
		private float selectedFontSize;
		private double sizeDiff;

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
				float fontSize;
				int locationOffset;

				if (value)
				{
					fontSize = this.selectedFontSize;
					locationOffset = Convert.ToInt32(-this.sizeDiff);
				}
				else
				{
					fontSize = this.defaultFontSize;
					locationOffset = Convert.ToInt32(this.sizeDiff);
				}
				
				this.Font = new Font(this.Font.FontFamily, fontSize);
				this.Location = new Point(this.Location.X, Convert.ToInt32(this.Location.Y + locationOffset));
				this.isSelected = value;
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			this.ForeColor = Color.White;
			this.defaultFontSize = this.Font.SizeInPoints;
			this.selectedFontSize = (float)(this.defaultFontSize * 1.33);
			this.sizeDiff = this.selectedFontSize - this.defaultFontSize;

			this.Text = this.title;
			this.Location = this.location;
			this.AutoSize = true;
		}
	}
}