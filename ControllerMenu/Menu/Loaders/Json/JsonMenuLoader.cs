using System;
using System.Collections.Generic;
using System.IO;
using ControllerMenu.Actions;
using ControllerMenu.Actions.Navigation;
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

	        //todo there must be a better way
	        this.CloseAction = this.actionResolver.Resolve("Navigation",
	            new NavigationActionOptions { Operations = new List<string> { "exit" }});
	    }

        private Action CloseAction { get; set; }

		public IMenuContainer Load(string menuName)
        {
            var menuContainer = new MenuContainer();

            var configRaw = File.ReadAllText($"Menus\\{menuName}.json");
		    var config = JsonConvert.DeserializeObject<JsonMenuConfiguration>(configRaw);

		    foreach (var menuItemEntry in config.MenuItems)
		    {
		        var title = menuItemEntry.Title;
		        Action menuAction = null;

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

			        if (actionOptions.CloseAfter)
			        {
			            action += this.CloseAction;
			        }

			        if (menuAction == null)
			        {
			            menuAction = action;
			        }
			        else
			        {
			            menuAction += action;
			        }
				}

		        menuContainer.MenuItems.Add(new MenuItem
		        {
		            Title = title,
		            Action = new MenuAction(menuAction)
		        });
		    }

            return menuContainer;
        }
	}
}
