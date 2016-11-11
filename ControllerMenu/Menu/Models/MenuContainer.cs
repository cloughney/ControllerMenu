using System.Collections.Generic;

namespace ControllerMenu.Menu.Models
{
    public class MenuContainer : IMenuContainer
    {
        public MenuContainer()
        {
            this.Parent = null;
            this.MenuItems = new List<MenuItem>();
        }

        public IMenuContainer Parent { get; set; }
        public IList<MenuItem> MenuItems { get; set; }
    }
}