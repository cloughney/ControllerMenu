using System;
using System.Windows.Forms;

namespace ControllerMenu
{
	public class MenuItem
	{
		private readonly Action action;
		private readonly MenuItemControl control;

		private bool isSelected;

		public MenuItem(string title, Action action)
		{
			this.action = action;

			this.control = new MenuItemControl(title);
			this.control.ControlCreated += (sender, args) =>
			{
				if (this.isSelected)
				{
					this.control.ChangeActiveState(true);
				}
			};

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
				if (this.control.IsCreated)
				{
					this.control.ChangeActiveState(value);
				}
				
				this.isSelected = value;
			}
		}

		public void Attach(Control parent)
		{
			parent.Controls.Add(this.control);
		}

		public void PerformAction()
		{
			this.action();
		}
	}
}