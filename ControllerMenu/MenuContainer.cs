using System.Collections.Generic;
using System.Windows.Forms;
using ControllerMenu.Services;

namespace ControllerMenu
{
	public class MenuContainer : Panel
	{
		private readonly FontService fontService;
		private readonly List<MenuItem> menuItems;
		private int selectedIndex;

		public MenuContainer(FontService fontService)
		{
			this.fontService = fontService;
			this.menuItems = new List<MenuItem>();
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

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			
			//this.BackColor = Color.Gray;
			this.Dock = DockStyle.Fill;
			this.Padding = new Padding(100);

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
			this.Controls.Clear();

			foreach (var menuItem in this.menuItems)
			{
				menuItem.Attach(this);
			}

			if (this.menuItems.Count > 0)
			{
				this.SelectItem(0);
			}
			else
			{
				this.selectedIndex = -1;
			}
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