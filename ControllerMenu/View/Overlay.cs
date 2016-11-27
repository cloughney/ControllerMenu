using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ControllerMenu.Input;
using ControllerMenu.Input.Models;
using ControllerMenu.Menu.Loaders;
using ControllerMenu.Menu.Models;
using ControllerMenu.Services;
using ControllerMenu.View.Menu;

namespace ControllerMenu.View
{
	public partial class Overlay : Form
	{
	    private const double MaxOpacityValue = 0.95;
		private const string DefaultMenuName = "mainmenu"; //TODO string class enum thingy for menu names?

		private readonly IActiveWindowService activeWindowService;
		private readonly IFontService fontService;
		private readonly IMenuLoader menuLoader;
		private readonly IEnumerable<IInputHandler> inputHandlers;

		private readonly MenuPanel primaryMenuContainer;
		private readonly MenuPanel secondaryMenuContainer;
		private MenuPanel activeMenuContainer;

		private delegate void OnInputDetected(IInputHandler handler, InputType inputType);

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

	    public void ToggleOverlay()
	    {
	        FormWindowState windowState;
	        List<InputType> activeInputTypes;

	        if (this.WindowState == FormWindowState.Maximized)
	        {
	            windowState = FormWindowState.Minimized;
	            activeInputTypes = new List<InputType> { InputType.Menu };
	        }
	        else
	        {
	            windowState = FormWindowState.Maximized;
	            activeInputTypes = new List<InputType>
	            {
	                InputType.Menu,
	                InputType.PreviousItem,
	                InputType.NextItem,
	                InputType.SelectItem,
	                InputType.Back
	            };

		        var activeWindowConfig = this.activeWindowService.CurrentConfiguration;
		        var activeMenuName = activeWindowConfig?.MenuName ?? DefaultMenuName;
				this.PopulateMenu(activeMenuName);
	        }

	        foreach (var inputHandler in this.inputHandlers)
	        {
	            inputHandler.ActiveInputs = activeInputTypes;
	        }

	        this.WindowState = windowState;
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

	    protected override void OnClosing(CancelEventArgs e)
	    {
	        foreach (var inputHandler in this.inputHandlers)
	        {
	            inputHandler.Dispose();
	        }

	        base.OnClosing(e);
	    }

	    private void SetupWindow()
	    {
		    this.Text = String.Empty;
			this.TopMost = true;
		    this.ShowInTaskbar = false;
		    this.WindowState = FormWindowState.Maximized;

			this.FormBorderStyle = FormBorderStyle.None;
			this.BackColor = Color.FromArgb(16, 16, 16);
		    this.Opacity = MaxOpacityValue;

		    this.RenderMenu();
			this.PopulateMenu(DefaultMenuName);

		    foreach (var inputHandler in this.inputHandlers)
			{
				inputHandler.Listen(this, new List<InputType>
				{
				    InputType.Menu,
				    InputType.PreviousItem,
				    InputType.NextItem,
				    InputType.SelectItem,
				    InputType.Back
				});

				inputHandler.InputDetected += (handler, input) =>
				{
					if (this.InvokeRequired)
					{
						this.Invoke(new OnInputDetected(this.HandleInput), handler, input);
					}
					else
					{
						this.HandleInput(handler, input);
					}
				};
			}
		}

//		private async Task FadeIn()
//		{
//			while (this.Opacity < MaxOpacityValue)
//			{
//				await Task.Delay(10);
//				this.Opacity += 0.05;
//			}
//
//		    this.Opacity = MaxOpacityValue;
//		}

		private void RenderMenu()
		{
			var panelHeight = Convert.ToInt32(this.Height * 0.95);

			var primaryMenuPanel = new Panel
			{
				Size = new Size(Convert.ToInt32(this.Width * 0.34), panelHeight),
				Dock = DockStyle.Left
			};

			var secondaryMenuPanel = new Panel
			{
				Size = new Size(Convert.ToInt32(this.Width * 0.66), panelHeight),
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
		        .Select(menuItem => new Menu.MenuItem(menuItem.Title, menuItem.Action))
		        .ToList();

			if (String.Equals(menuName, DefaultMenuName, StringComparison.OrdinalIgnoreCase))
			{
				var exitItem = new Menu.MenuItem("Exit", new MenuAction(this.ToggleOverlay));
				menuItems.Add(exitItem);
			}
			else
			{
				var mainMenuItem = new Menu.MenuItem("Main Menu", new MenuAction(() => this.PopulateMenu(DefaultMenuName))); //TODO invoke necessary?
				menuItems.Add(mainMenuItem);
			}

		    this.primaryMenuContainer.MenuItems = menuItems;
		}

		private void CloseSecondContainer()
		{
			this.secondaryMenuContainer.MenuItems.Clear();
			this.secondaryMenuContainer.Visible = false;
			this.activeMenuContainer = this.primaryMenuContainer;
		}

		private void HandleInput(IInputHandler handler, InputType input)
		{
			switch (input)
			{
                case InputType.Menu:
			        this.ToggleOverlay();
			        break;

				case InputType.Back:
					if (this.activeMenuContainer != this.primaryMenuContainer)
					{
					    this.CloseSecondContainer();
					}
					else
					{
						this.ToggleOverlay();
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
	}
}
