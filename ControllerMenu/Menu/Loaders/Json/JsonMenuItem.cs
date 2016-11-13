using System.Collections.Generic;

namespace ControllerMenu.Menu.Loaders.Json
{
	public class JsonMenuItem
	{
		public string Title { get; set; }

	    public IList<JsonAction> Actions { get; set; }
	}
}