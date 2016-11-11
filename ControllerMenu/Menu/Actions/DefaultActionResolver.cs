using System;
using System.Collections.Generic;
using System.Linq;

namespace ControllerMenu.Menu.Actions
{
    public class DefaultActionResolver : IActionResolver
    {
        private readonly Dictionary<string, Func<object, Action>> registeredActionBuilders;

        public DefaultActionResolver(IEnumerable<IActionBuilder> actionBuilders)
        {

        }

        public Action Resolve(string actionType, object options)
        {
            throw new NotImplementedException();
        }
    }

    public interface IActionBuilder
    {
        string Type { get; }

        Action Build(object options);
    }

    public class NavigationActionBuilder : IActionBuilder
    {
        public string Type => "navigation";

        public Action Build(object options)
        {
            throw new NotImplementedException();
        }
    }
}