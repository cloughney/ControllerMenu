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
