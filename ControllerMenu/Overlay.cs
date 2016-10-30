using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerMenu.Services;

namespace ControllerMenu
{
	public partial class Overlay : Form
	{
		private readonly double maxOpacityValue = 1.0;

		private readonly MenuContainer menuContainer = new MenuContainer(new FontService());

		public Overlay()
		{
			this.InitializeComponent();
		}

		protected override bool IsInputKey(Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Right:
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
					return true;
				case Keys.Shift | Keys.Right:
				case Keys.Shift | Keys.Left:
				case Keys.Shift | Keys.Up:
				case Keys.Shift | Keys.Down:
					return true;
			}
			return base.IsInputKey(keyData);
		}

		protected override void OnKeyDown(KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Escape:
					this.Close();
					break;

				case Keys.Up:
					this.menuContainer.PreviousItem();
					break;

				case Keys.Down:
					this.menuContainer.NextItem();
					break;
			}

			base.OnKeyDown(e);
		}

		private void Overlay_Load(object sender, EventArgs e)
		{
			this.SetupWindow();
			this.FadeIn();
		}

		private void SetupWindow()
		{
			this.TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			this.BackColor = Color.FromArgb(16, 16, 16);
			this.Opacity = 0.0;
		}

		private void RenderMenu()
		{
			this.Controls.Add(this.menuContainer);
		}

		private async void FadeIn()
		{
			while (this.Opacity < this.maxOpacityValue)
			{
				await Task.Delay(10);
				this.Opacity += 0.05;
			}
			this.Opacity = this.maxOpacityValue;
			this.RenderMenu();
		}
	}
}
