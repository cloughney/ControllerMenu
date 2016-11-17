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
            this.registeredActionBuilders = actionBuilders.ToDictionary(k => k.Type, StringComparer.OrdinalIgnoreCase);
        }

        public Action Resolve(string actionType, IActionOptions options)
        {
            IActionBuilder builder;
	        if (!this.registeredActionBuilders.TryGetValue(actionType, out builder))
	        {
		        return null;
			}

	        var resolvedAction = builder.Build(this.context, options);
			
			if (options.CloseAfter)
			{
				return () =>
				{
					resolvedAction.Invoke();
					this.context.Overlay.Close();
				};
			}

			return resolvedAction;
        }
    }
}