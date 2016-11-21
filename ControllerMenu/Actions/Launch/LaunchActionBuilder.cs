using System;
using System.Diagnostics;

namespace ControllerMenu.Actions.Launch
{
	public class LaunchActionBuilder : IActionBuilder
	{
		public string Type => "Launch";

		public Action Build(IApplicationContext context, IActionOptions options)
		{
			var launchOptions = options as LaunchActionOptions;
			if (launchOptions == null)
			{
				throw new Exception("Invalid options for launch action"); //TODO proper error handling
			}

			return () =>
			{
				Process.Start(launchOptions.Path, launchOptions.Arguments);
				//TODO waitForExit
			};
		}
	}
}