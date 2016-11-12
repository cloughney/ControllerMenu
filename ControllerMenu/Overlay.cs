using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerMenu.Menu.Loaders;
using ControllerMenu.Menu.Models;
using ControllerMenu.Services;
using ControllerMenu.View.Menu;

namespace ControllerMenu
{
	public partial class Overlay : Form
	{
	    private const double MaxOpacityValue = 0.95;

	    private readonly IActiveWindowService activeWindowService;
		private readonly IFontService fontService;
		private readonly IMenuLoader menuLoader;
		private readonly IEnumerable<IInputHandler> inputHandlers;

		private readonly MenuPanel primaryMenuContainer;
		private readonly MenuPanel secondaryMenuContainer;
		private MenuPanel activeMenuContainer;

		public Overlay(
		    IApplicationContext context,
			IActiveWindowService activeWindowService,
			IFontService fontService,
			IMenuLoader menuLoader,
			IEnumerable<IInputHandler> inputHandlers)
		{
			this.InitializeComponent();

		    context.Overlay = this; //TODO better solution for this?

			this.activeWindowService = activeWindowService;
			this.fontService = fontService;
			this.menuLoader = menuLoader;
			this.inputHandlers = inputHandlers;

			this.primaryMenuContainer = new MenuPanel();
			this.secondaryMenuContainer = new MenuPanel
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
			this.PopulateMenu("mainmenu"); //TODO string class enum thingy for menu names?

		    foreach (var inputHandler in this.inputHandlers)
			{
				inputHandler.Listen(this);
				inputHandler.InputDetected += this.OnInputRecieved;
			}
		}

		private async Task FadeIn()
		{
			while (this.Opacity < MaxOpacityValue)
			{
				await Task.Delay(10);
				this.Opacity += 0.05;
			}

			this.Opacity = MaxOpacityValue;
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

		private void PopulateMenu(string menuName)
		{
			var mainMenu = this.menuLoader.Load(menuName);
		    var menuItems = mainMenu.MenuItems
		        .Select(menuItem => new View.Menu.MenuItem(menuItem.Title, menuItem.Action))
		        .ToList();

		    var exitItem = new View.Menu.MenuItem("Exit", new MenuAction(this.Close));
		    menuItems.Add(exitItem);

		    this.primaryMenuContainer.MenuItems = menuItems;
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
