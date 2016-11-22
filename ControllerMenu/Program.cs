using System;
using System.Windows.Forms;
using Autofac;
using ControllerMenu.Actions;
using ControllerMenu.Actions.EndProcess;
using ControllerMenu.Actions.Launch;
using ControllerMenu.Actions.Navigation;
using ControllerMenu.Input;
using ControllerMenu.Input.Controller;
using ControllerMenu.Input.Keyboard;
using ControllerMenu.Input.Models;
using ControllerMenu.Menu.Loaders;
using ControllerMenu.Menu.Loaders.Json;
using ControllerMenu.Services;
using ControllerMenu.View;

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

			var processIdRaw = args.Length > 0 ? args[0] : null;
			int processId;
			if (Int32.TryParse(processIdRaw, out processId))
			{
			    var activeWindowService = container.Resolve<IActiveWindowService>();
			    activeWindowService.ProcessId = processId;
			}

		    var overlayForm = container.Resolve<Overlay>();

		    Application.Run(overlayForm);
		}

		private static IContainer RegisterServices()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<FontService>().As<IFontService>().SingleInstance();
			builder.RegisterType<JsonMenuLoader>().As<IMenuLoader>().SingleInstance();

			builder.RegisterType<KeyboardInputHandler>().As<IInputHandler>().InstancePerDependency();
		    builder.RegisterType<ControllerInputHandler>().As<IInputHandler>().InstancePerDependency();

		    builder.RegisterType<ActiveWindowService>().As<IActiveWindowService>().SingleInstance();

		    builder.RegisterType<JsonMenuLoader>().As<IMenuLoader>().SingleInstance();

		    builder.RegisterType<DefaultActionResolver>().As<IActionResolver>().SingleInstance();
		    builder.RegisterType<NavigationActionBuilder>().As<IActionBuilder>();
			builder.RegisterType<LaunchActionBuilder>().As<IActionBuilder>();
			builder.RegisterType<EndProcessActionBuilder>().As<IActionBuilder>();

			builder.RegisterType<ApplicationContext>().As<IApplicationContext>().SingleInstance();

		    builder.RegisterType<Overlay>();

			return builder.Build();
		}
	}
}
