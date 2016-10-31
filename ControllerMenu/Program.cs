using System;
using System.Windows.Forms;
using Autofac;
using ControllerMenu.Services;

namespace ControllerMenu
{
	public static class Program
	{
		[STAThread]
		public static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);

			var container = RegisterServices();
			var overlayForm = container.Resolve<Overlay>();

			Application.Run(overlayForm);
		}

		private static IContainer RegisterServices()
		{
			var builder = new ContainerBuilder();

			builder.RegisterType<FontService>().As<IFontService>().SingleInstance();
			builder.RegisterType<JsonCommandResolver>().As<ICommandResolver>();

			builder.RegisterType<KeyboardInputHandler>().As<IInputHandler>();

			builder.RegisterType<Overlay>();

			return builder.Build();
		}
	}
}
