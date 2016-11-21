using System;
using System.Windows.Forms;
using ControllerMenu.Input.Models;

namespace ControllerMenu.Input
{
    public delegate void InputEventHandler(IInputHandler handler, InputType input);

    public interface IInputHandler : IDisposable
	{
		event InputEventHandler InputDetected;

		void Listen(Control parent);
	}
}
