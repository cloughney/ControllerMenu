using System.Windows.Forms;

namespace ControllerMenu.Services
{
	public delegate void InputEventHandler(IInputHandler handler, InputType input);

	public enum InputType
	{
		NextItem,
		PreviousItem,
		SelectItem,
		Back
	}

	public interface IInputHandler
	{
		event InputEventHandler InputDetected;

		void Listen(Control parent);
	}
}
