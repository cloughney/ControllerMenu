using System;

namespace ControllerMenu.Actions
{
    public interface IActionResolver
    {
        Action Resolve(string actionType, IActionOptions options);
    }
}