using System.Windows.Forms;
using ControllerMenu.Controls;
using ControllerMenu.Menu.Models;
using ControllerMenu.Menu.ViewModels;

namespace ControllerMenu.View.Menu
{
	public class MenuItem : Component<MenuItemControl>
	{
		private readonly MenuAction action;

		private bool isSelected;

		public MenuItem(string title, MenuAction action)
		{
			this.action = action;
			this.isSelected = false;

			this.Control = new MenuItemControl(title);
		}

		public bool IsSelected
		{
			get
			{
				return this.isSelected;
			}
			set
			{
				if (this.Control.Created)
				{
					this.Control.ChangeActiveState(value);
				}
				
				this.isSelected = value;
			}
		}

		public void PerformAction()
		{
			this.action.Perform();
		}

		public override void Attach(Control parent)
		{
			base.Attach(parent);

			if (this.Control.Created && this.isSelected)
			{
				this.Control.ChangeActiveState(true);
			}
		}
	}
}