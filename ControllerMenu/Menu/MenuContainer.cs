using System.Collections.Generic;
using System.Windows.Forms;
using ControllerMenu.Controls;
using ControllerMenu.Services;

namespace ControllerMenu.Menu
{
	public class MenuContainer : Component<MenuContainerControl>
	{
		private readonly List<MenuItem> menuItems;
		private int selectedIndex;

		public MenuContainer()
		{
			this.menuItems = new List<MenuItem>();

			this.Control = new MenuContainerControl();
			this.Control.VisibleChanged += (sender, args) =>
			{
				var control = sender as Control;
				if (control != null && control.Visible)
				{
					this.InitializeMenu();
				}
			};
		}

		public bool Visible
		{
			get
			{
				return this.Control.Visible;
			}
			set
			{
				this.Control.Visible = value;
			}
		}

		public List<MenuItem> MenuItems
		{
			get
			{
				return this.menuItems;
			}
			set
			{
				this.menuItems.Clear();
				this.menuItems.AddRange(value);

				this.InitializeMenu();
			}
		}

		public override void Attach(Control parent)
		{
			base.Attach(parent);

			this.InitializeMenu();
		}

		public void NextItem()
		{
			this.SelectItem(this.selectedIndex + 1);
		}

		public void PreviousItem()
		{
			this.SelectItem(this.selectedIndex - 1);
		}

		public MenuItem GetSelectedItem()
		{
			return this.menuItems[this.selectedIndex];
		}

		private void InitializeMenu()
		{
			if (this.menuItems.Count > 0)
			{
				this.SelectItem(0);
			}
			else
			{
				this.selectedIndex = -1;
			}

			this.Control.InitializeMenu(this.menuItems);
		}

		private void SelectItem(int index)
		{
			if (this.menuItems.Count == 0)
			{
				return;
			}

			if (index < 0)
			{
				index = this.menuItems.Count - 1;
			}
			else if (index >= this.menuItems.Count)
			{
				index = 0;
			}

			if (this.selectedIndex >= 0 && this.selectedIndex < this.menuItems.Count)
			{
				this.menuItems[this.selectedIndex].IsSelected = false;
			}
			
			this.menuItems[index].IsSelected = true;
			this.selectedIndex = index;
		}
	}
}