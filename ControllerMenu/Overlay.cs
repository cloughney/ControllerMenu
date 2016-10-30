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

		private readonly FontService fontService = new FontService();
		private readonly MenuContainer primaryMenuContainer;
		private readonly MenuContainer secondaryMenuContainer;
		private MenuContainer activeMenuContainer;

		public Overlay()
		{
			this.InitializeComponent();

			this.primaryMenuContainer = new MenuContainer(this.fontService);
			this.secondaryMenuContainer = new MenuContainer(this.fontService)
			{
				Visible = false
			};

			this.activeMenuContainer = this.primaryMenuContainer;
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
					this.activeMenuContainer.PreviousItem();
					break;

				case Keys.Down:
					this.activeMenuContainer.NextItem();
					break;

				case Keys.Enter:
					this.activeMenuContainer.GetSelectedItem().PerformAction();
					break;
			}

			base.OnKeyDown(e);
		}

		private void Overlay_Load(object sender, EventArgs e)
		{
			this.SetupWindow();
		}

		private async void SetupWindow()
		{
			this.TopMost = true;
			this.FormBorderStyle = FormBorderStyle.None;
			this.WindowState = FormWindowState.Maximized;
			this.BackColor = Color.FromArgb(16, 16, 16);
			this.Opacity = 0.0;

			await this.FadeIn();

			this.RenderMenu();
			this.PopulatePrimaryMenu();
		}

		private void RenderMenu()
		{
			var panelHeight = Convert.ToInt32(this.Height * 0.95);

			var primaryMenuPanel = new Panel
			{
				Size = new Size(Convert.ToInt32(this.Width * 0.25), panelHeight),
				Dock = DockStyle.Left
			};

			var secondaryMenuPanel = new Panel
			{
				Size = new Size(Convert.ToInt32(this.Width * 0.75), panelHeight),
				Dock = DockStyle.Right
			};
			
			primaryMenuPanel.Font = this.fontService.GetFontByName(Fonts.Menu, 32);
			secondaryMenuPanel.Font = this.fontService.GetFontByName(Fonts.Menu, 24);

			primaryMenuPanel.Controls.Add(this.primaryMenuContainer);
			secondaryMenuPanel.Controls.Add(this.secondaryMenuContainer);

			this.Controls.Add(primaryMenuPanel);
			this.Controls.Add(secondaryMenuPanel);
		}

		private void PopulatePrimaryMenu()
		{
			this.primaryMenuContainer.MenuItems = new List<MenuItem>
			{
				new MenuItem("Test", () =>
				{
					this.secondaryMenuContainer.MenuItems = new List<MenuItem>
					{
						new MenuItem("Close", () =>
						{
							this.secondaryMenuContainer.MenuItems.Clear();
							this.secondaryMenuContainer.Visible = false;
							this.activeMenuContainer = this.primaryMenuContainer;
							this.Refresh();
						}),
						new MenuItem("Also Close", () =>
						{
							this.secondaryMenuContainer.MenuItems.Clear();
							this.secondaryMenuContainer.Visible = false;
							this.activeMenuContainer = this.primaryMenuContainer;
							this.Refresh();
						})
					};

					this.secondaryMenuContainer.Visible = true;
					this.activeMenuContainer = this.secondaryMenuContainer;
					this.Refresh();
				}),

				new MenuItem("Exit", this.Close)
			};
		}

		private async Task FadeIn()
		{
			while (this.Opacity < this.maxOpacityValue)
			{
				await Task.Delay(10);
				this.Opacity += 0.05;
			}

			this.Opacity = this.maxOpacityValue;
		}
	}
}
