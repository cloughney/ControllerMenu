using System.Collections.Generic;
using System.Windows.Forms;

namespace ControllerMenu.Menu
{
	public class MenuContainerControl : Panel
	{
		public void InitializeMenu(List<MenuItem> menuItems)
		{
			if (!this.Created)
			{
				return;
			}

			this.Controls.Clear();

			foreach (var menuItem in menuItems)
			{
				menuItem.Attach(this);
			}
		}

		protected override void OnCreateControl()
		{
			base.OnCreateControl();
			
			this.Dock = DockStyle.Fill;
			this.Padding = new Padding(100);
		}
	}
}