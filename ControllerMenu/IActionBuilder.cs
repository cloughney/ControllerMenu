using System;
using ControllerMenu.Menu.Actions;

namespace ControllerMenu
{
    public interface IActionBuilder
    {
        string Type { get; }
        Action Build(IApplicationContext context, IActionOptions options);
    }
}