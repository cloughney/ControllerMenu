using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ControllerMenu.Services
{
	public class ActiveWindowService : IActiveWindowService
	{
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern IntPtr OpenProcess(uint processAccessFlags, bool inheritHandle, uint processId);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint processId);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

		[DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
		private static extern IntPtr GetParent(IntPtr hWnd);

		public string GetProcessName()
		{
			try
			{
				var targetHwnd = GetWindow(Process.GetCurrentProcess().MainWindowHandle, 2);

				while (true)
				{
					var wHnd = GetParent(targetHwnd);
					if (wHnd.Equals(IntPtr.Zero))
					{
						break;
					}

					targetHwnd = wHnd;
				}

				uint processId;
				GetWindowThreadProcessId(targetHwnd, out processId);

				var process = Process.GetProcessById((int)processId);
				return process.MainModule.FileName;
			}
			catch (Exception ex)
			{
				File.WriteAllText("output.txt", ex.Message);
			}

			return null;
		}
	}
}
