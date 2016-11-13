using System;

namespace ControllerMenu.Menu.Actions
{
    public interface IActionResolver
    {
        Action Resolve(string actionType, IActionOptions options);
    }
}