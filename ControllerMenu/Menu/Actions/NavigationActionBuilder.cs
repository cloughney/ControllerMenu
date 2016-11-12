using System;

namespace ControllerMenu.Menu.Actions
{
    public class NavigationActionBuilder : IActionBuilder
    {
        public string Type => "navigation";

        public Action Build(IApplicationContext context, object options)
        {
            //todo switch on navigation action
            return () => context.Overlay.Close();
        }
    }
}