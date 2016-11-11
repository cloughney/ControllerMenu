using System;
using System.Collections.Generic;

namespace ControllerMenu.Menu.Actions
{
    public interface IActionResolver
    {
        Action Resolve(string actionType, object options);
    }
}