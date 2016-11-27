using System;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ControllerMenu.Actions.EndProcess
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
					var runningProcesses = Process.GetProcessesByName(endOptions.ProcessName);
					foreach (var p in runningProcesses)
					{
						p.Kill();
					}
				};
			}
			
			//if (!String.IsNullOrWhiteSpace(endOptions.WindowTitle))
			//	{
			//		return () =>
			//		{
			//			var runningProcesses = Process.GetProcesses();
			//			foreach (var process in runningProcesses)
			//			{
			//				if (process.MainWindowHandle == IntPtr.Zero || String.IsNullOrWhiteSpace(process.MainWindowTitle))
			//				{
			//					continue;
			//				}

			//				if (!Regex.IsMatch(process.MainWindowTitle, endOptions.WindowTitle))
			//				{
			//					continue;
			//				}

			//				process.CloseMainWindow();
			//			}
			//		};
			//	}

			throw new Exception("Invalid options for close action"); //TODO proper error handling
		}
	}
}