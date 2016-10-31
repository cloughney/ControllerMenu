using System.Windows.Forms;

namespace ControllerMenu.Services
{
	public class JoystickInputHandler : IInputHandler
	{
		public event InputEventHandler InputDetected;
		public void Listen(Control parent)
		{
			//var dInput = new DirectInput();
		}
	}
}