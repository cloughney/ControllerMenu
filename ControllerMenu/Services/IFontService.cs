using System.Drawing;

namespace ControllerMenu.Services
{
    public interface IFontService
    {
        Font GetFontByName(string name, float size);
    }
}