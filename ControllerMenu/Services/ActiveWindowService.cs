using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using ControllerMenu.Models;

namespace ControllerMenu.Services
{
	public class ActiveWindowService : IActiveWindowService
	{
		[DllImport("user32.dll")]
		private static extern IntPtr GetTopWindow(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll")]
		private static extern bool IsWindowVisible(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern int GetWindowTextLength(IntPtr hWnd);

		[DllImport("user32.dll")]
		private static extern IntPtr GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		private readonly IDictionary<string, ActiveWindowConfiguration> configuredWindows;

	    public ActiveWindowService()
	    {
	        //TODO populate from service
	        this.configuredWindows = new Dictionary<string, ActiveWindowConfiguration>
	        {
				//{ "kodi.exe", new ActiveWindowConfiguration { MenuName = "kodi" } }
	        };
	    }

	    public ActiveWindowConfiguration CurrentConfiguration
	    {
	        get
	        {
				var activeProcess = GetActiveProcess();
		        if (activeProcess == null)
		        {
			        return null;
		        }

		        var processName = new FileInfo(activeProcess.MainModule.FileName).Name.ToLower();

				ActiveWindowConfiguration config;
	            return this.configuredWindows.TryGetValue(processName, out config) ? config : null;
	        }
	    }

		private static Process GetActiveProcess()
		{
			const uint GW_HWNDNEXT = 2;

			var topWindow = GetTopWindow(IntPtr.Zero);
			if (topWindow == IntPtr.Zero)
			{
				return null;
			}

			IntPtr nextWindow;
			var prevWindow = topWindow;

			while (true)
			{
				nextWindow = GetWindow(prevWindow, GW_HWNDNEXT);
				if (nextWindow == IntPtr.Zero)
				{
					return null;
				}

				if (IsWindowVisible(nextWindow) && GetWindowTextLength(nextWindow) > 0)
				{
					break;
				}

				prevWindow = nextWindow;
			}

			uint processId;
			GetWindowThreadProcessId(nextWindow, out processId);

			return Process.GetProcessById((int) processId);
		}
	}
}
