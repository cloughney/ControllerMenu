using System.Collections.Generic;

namespace ControllerMenu.Menu.Models
{
    public interface IMenuContainer
    {
        IMenuContainer Parent { get; }

        IList<MenuItem> MenuItems { get;  }
    }
}