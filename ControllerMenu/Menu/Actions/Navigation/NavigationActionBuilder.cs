using System;
using System.Collections.Generic;

namespace ControllerMenu.Menu.Actions.Navigation
{
    public class NavigationActionBuilder : IActionBuilder
    {
        public string Type => "Navigation";

        public Action Build(IApplicationContext context, IActionOptions options)
        {
	        var navOptions = options as NavigationActionOptions;
	        if (navOptions == null)
	        {
		        throw new Exception("Invalid options for navigation action"); //TODO proper error handling
	        }

			var actionQueue = new Queue<Action>();

	        foreach (var operation in navOptions.Operations)
	        {
		        switch (operation.ToLower())
		        {
					case "exit":
						actionQueue.Enqueue(() => context.Overlay.Close());
				        break;
					default:
						throw new Exception("Invalid operation configured for navigation action"); //TODO proper error handling
				}
	        }

	        return () => { foreach (var action in actionQueue) action.Invoke(); };
        }
    }
}