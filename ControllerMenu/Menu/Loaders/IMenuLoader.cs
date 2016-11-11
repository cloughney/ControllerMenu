using ControllerMenu.Menu.Models;

namespace ControllerMenu.Menu.Loaders
{
    public interface IMenuLoader
    {
        IMenuContainer Load(string menuName);
    }
}