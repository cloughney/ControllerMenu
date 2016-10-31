using System;

namespace ControllerMenu.Commands
{
	public class Command
	{
		public Command(string title, Action action)
		{
			this.Title = title;
			this.Action = action;
		}

		public string Title { get; private set; }

		public Action Action { get; set; }
	}
}