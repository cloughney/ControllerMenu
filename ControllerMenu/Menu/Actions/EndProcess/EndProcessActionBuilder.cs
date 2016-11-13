using System;
using System.Diagnostics;

namespace ControllerMenu.Menu.Actions.EndProcess
{
	public class EndProcessActionBuilder : IActionBuilder
	{
		public string Type => "Close";

		public Action Build(IApplicationContext context, IActionOptions options)
		{
			var endOptions = options as EndProcessActionOptions;
			if (endOptions == null)
			{
				throw new Exception("Invalid options for close action"); //TODO proper error handling
			}

			if (!String.IsNullOrWhiteSpace(endOptions.ProcessName))
			{
				return () =>
				{
					foreach (var p in Process.GetProcessesByName(endOptions.ProcessName))
					{
						p.Kill();
					}
				};
			}

			//if (!String.IsNullOrWhiteSpace(endOptions.WindowTitle))
			//{
			//	return () =>
			//	{
					
			//	};
			//}

			throw new Exception("Invalid options for close action"); //TODO proper error handling
		}
	}
}