using System.IO;
using ControllerMenu.Menu.Actions;
using ControllerMenu.Menu.Models;
using Newtonsoft.Json;

namespace ControllerMenu.Menu.Loaders.Json
{
    public class JsonMenuLoader : IMenuLoader
    {
        private readonly IActionResolver actionResolver;

	    public JsonMenuLoader(IActionResolver actionResolver)
	    {
	        this.actionResolver = actionResolver;
	    }

		public IMenuContainer Load(string menuName)
        {
            var menuContainer = new MenuContainer();

			var configRaw = File.ReadAllText($"{menuName}.json");
		    var config = JsonConvert.DeserializeObject<JsonMenuConfiguration>(configRaw);

		    foreach (var menuItemEntry in config.MenuItems)
		    {
		        var title = menuItemEntry.Title;

			    foreach (var actionEntry in menuItemEntry.Actions)
			    {
					var actionType = actionEntry.Type;
					var actionOptions = actionEntry.Options;
					var action = this.actionResolver.Resolve(actionType, actionOptions);

					if (action == null)
					{
						//TODO error handling
						continue;
					}

					menuContainer.MenuItems.Add(new MenuItem
					{
						Title = title,
						Action = new MenuAction(action)
					});
				}
		    }

            return menuContainer;
        }
	}
}
