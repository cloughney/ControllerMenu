using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using ControllerMenu.Services;

namespace ControllerMenu
{
	public class MenuContainer : Panel
	{
		private readonly FontService fontService;
		private readonly List<MenuItem> menuItems = new List<MenuItem>();
		private int selectedIndex;

		public MenuContainer(FontService fontService)
		{
			this.fontService = fontService;
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			
			var width = Convert.ToInt32(Math.Ceiling(this.Parent.Width * 0.95));
			var height = Convert.ToInt32(Math.Ceiling(this.Parent.Height * 0.95));
			this.Size = new Size(width, height);

			var posX = (this.Parent.Width - width) / 2;
			var posY = (this.Parent.Height - height) / 2;
			this.Location = new Point(posX, posY);

			this.Font = this.fontService.GetFontByName(Fonts.Menu, 16);

			this.PopulateMenu();
		}

		public void NextItem()
		{
			this.SelectItem(this.selectedIndex + 1);
		}

		public void PreviousItem()
		{
			this.SelectItem(this.selectedIndex - 1);
		}

		private void PopulateMenu()
		{
			for (var i = 0; i < 5; i++)
			{
				var menuItem = new MenuItem(String.Concat("Menu Item ", i), new Point(0, i * 50));
				this.menuItems.Add(menuItem);
				this.Controls.Add(menuItem);
			}

			this.SelectItem(0);
		}

		private void SelectItem(int index)
		{
			if (index < 0)
			{
				index = this.menuItems.Count - 1;
			}
			else if (index >= this.menuItems.Count)
			{
				index = 0;
			}

			this.menuItems[this.selectedIndex].IsSelected = false;
			this.menuItems[index].IsSelected = true;
			this.selectedIndex = index;
		}
	}
}