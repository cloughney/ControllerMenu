using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Autofac;
using ControllerMenu.Services;

namespace ControllerMenu
{
	public static class Program
	{
		[STAThread]
		public static void Main(string[] args)
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var container = RegisterServices();
			var overlayForm = container.Resolve<Overlay>();

			var processIdRaw = args.Length > 0 ? args[0] : null;
			int processId;
			if (Int32.TryParse(processIdRaw, out processId))
			{
				
			}
			
			Application.Run(overlayForm);
		}

		private static IContainer RegisterServices()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<FontService>().As<IFontService>().SingleInstance();
			builder.RegisterType<JsonCommandResolver>().As<ICommandResolver>();

			builder.RegisterType<KeyboardInputHandler>().As<IInputHandler>();

			builder.RegisterType<ActiveWindowService>().As<IActiveWindowService>();

			builder.RegisterType<Overlay>();

			return builder.Build();
		}
	}
}
