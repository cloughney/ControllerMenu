using System;

namespace ControllerMenu.Actions.Navigation
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

            Action navigationActions = null;

	        foreach (var operation in navOptions.Operations)
	        {
	            Action operationAction;
		        switch (operation.ToLower())
		        {
					case "exit":
						operationAction = () => context.Overlay.ToggleOverlay();
				        break;
					default:
						throw new Exception("Invalid operation configured for navigation action"); //TODO proper error handling
				}

	            if (navigationActions == null)
	            {
	                navigationActions = operationAction;
	            }
	            else
	            {
	                navigationActions += operationAction;
	            }
	        }

	        return navigationActions;
        }
    }
}