using System.Collections.Generic;
using ControllerMenu.Menu.Actions.Launch;

namespace ControllerMenu.Menu.Actions.Navigation
{
	public class NavigationActionOptions : ActionOptions
	{
		public IList<string> Operations { get; set; }
	}
}