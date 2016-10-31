using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerMenu.Menu;
using ControllerMenu.Services;
using MenuItem = ControllerMenu.Menu.MenuItem;

namespace ControllerMenu
{
	public partial class Overlay : Form
	{
		private readonly double maxOpacityValue = 0.95;

		private readonly IFontService fontService;
		private readonly ICommandResolver commandResolver;
		private readonly IEnumerable<IInputHandler> inputHandlers;

		private readonly MenuContainer primaryMenuContainer;
		private readonly MenuContainer secondaryMenuContainer;
		private MenuContainer activeMenuContainer;

		public Overlay(
			IFontService fontService,
			ICommandResolver commandResolver,
			IEnumerable<IInputHandler> inputHandlers)
		{
			this.InitializeComponent();

			this.fontService = fontService;
			this.commandResolver = commandResolver;
			this.inputHandlers = inputHandlers;

			this.primaryMenuContainer = new MenuContainer();
			this.secondaryMenuContainer = new MenuContainer
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

			foreach (var inputHandler in this.inputHandlers)
			{
				inputHandler.Listen(this);
				inputHandler.InputDetected += this.OnInputRecieved;
			}
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

			this.primaryMenuContainer.Attach(primaryMenuPanel);
			this.secondaryMenuContainer.Attach(secondaryMenuPanel);

			this.Controls.Add(primaryMenuPanel);
			this.Controls.Add(secondaryMenuPanel);
		}

		private void PopulatePrimaryMenu()
		{
			var commands = this.commandResolver.Resolve();
			this.primaryMenuContainer.MenuItems = new List<MenuItem>
			{
				new MenuItem("Test", () =>
				{
					this.secondaryMenuContainer.MenuItems = new List<MenuItem>
					{
						new MenuItem("Close this menu", this.CloseSecondContainer),
						new MenuItem("Also close this menu", this.CloseSecondContainer)
					};

					this.secondaryMenuContainer.Visible = true;
					this.activeMenuContainer = this.secondaryMenuContainer;
					this.Refresh();
				}),

				new MenuItem("Exit", this.Close)
			};
		}

		private void OnInputRecieved(IInputHandler handler, InputType input)
		{
			switch (input)
			{
				case InputType.Back:
					if (this.activeMenuContainer == this.primaryMenuContainer)
					{
						this.Close();
					}
					else
					{
						this.CloseSecondContainer();
					}

					break;

				case InputType.PreviousItem:
					this.activeMenuContainer.PreviousItem();
					break;

				case InputType.NextItem:
					this.activeMenuContainer.NextItem();
					break;

				case InputType.SelectItem:
					this.activeMenuContainer.GetSelectedItem().PerformAction();
					break;
			}
		}

		private void CloseSecondContainer()
		{
			this.secondaryMenuContainer.MenuItems.Clear();
			this.secondaryMenuContainer.Visible = false;
			this.activeMenuContainer = this.primaryMenuContainer;
		}
	}
}
