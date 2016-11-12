using System;
using System.Collections.Generic;
using System.Linq;

namespace ControllerMenu.Menu.Actions
{
    public class DefaultActionResolver : IActionResolver
    {
        private readonly IApplicationContext context;
        private readonly Dictionary<string, IActionBuilder> registeredActionBuilders;

        public DefaultActionResolver(
            IApplicationContext context,
            IEnumerable<IActionBuilder> actionBuilders)
        {
            this.context = context;
            this.registeredActionBuilders = actionBuilders.ToDictionary(k => k.Type);
        }

        public Action Resolve(string actionType, object options)
        {
            IActionBuilder builder;
            return this.registeredActionBuilders.TryGetValue(actionType, out builder)
                ? builder.Build(this.context, options)
                : null;
        }
    }
}