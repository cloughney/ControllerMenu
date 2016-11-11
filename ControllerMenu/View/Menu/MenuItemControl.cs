using System;
using System.Drawing;
using System.Windows.Forms;

namespace ControllerMenu.Menu.ViewModels
{
	public class MenuItemControl : Label
	{
		private readonly string title;

		private float defaultFontSize;
		private float selectedFontSize;
		private double sizeDiff;

		public MenuItemControl(string title)
		{
			this.title = title;
		}

		public event EventHandler ControlCreated;

		public void ChangeActiveState(bool updatedIsSelected)
		{
			float fontSize;
			int locationOffset;

			if (updatedIsSelected)
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
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();

			this.defaultFontSize = this.Font.SizeInPoints;
			this.selectedFontSize = (float)(this.defaultFontSize * 1.33);
			this.sizeDiff = this.selectedFontSize - this.defaultFontSize;

			this.ForeColor = Color.White;
			this.Text = this.title;
			this.Dock = DockStyle.Bottom;
			this.AutoSize = true;
			
			this.ControlCreated?.Invoke(this, EventArgs.Empty);
		}
	}
}