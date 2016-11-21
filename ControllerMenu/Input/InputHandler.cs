using System;
using System.Windows.Forms;

namespace ControllerMenu.Input
{
	public delegate void InputEventHandler(IInputHandler handler, InputType input);

	public enum InputType
	{
		NextItem,
		PreviousItem,
		SelectItem,
		Back
	}

	public interface IInputHandler : IDisposable
	{
		event InputEventHandler InputDetected;

		void Listen(Control parent);
	}
}
