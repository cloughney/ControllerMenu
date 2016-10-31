using System;
using System.Windows.Forms;
using ControllerMenu.Controls;

namespace ControllerMenu.Menu
{
	public class MenuItem : Component<MenuItemControl>
	{
		private readonly Action action;

		private bool isSelected;

		public MenuItem(string title, Action action)
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
			this.action();
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