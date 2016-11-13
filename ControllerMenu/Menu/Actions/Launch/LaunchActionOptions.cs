namespace ControllerMenu.Menu.Actions.Launch
{
	public class LaunchActionOptions : ActionOptions
	{
		public string Path { get; set; }

		public string Arguments { get; set; }

		public bool WaitUntilExit { get; set; }
	}
}