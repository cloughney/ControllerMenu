using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ControllerMenu.Input.Models;

namespace ControllerMenu.Input
{
    public delegate void InputEventHandler(IInputHandler handler, InputType input);

    public interface IInputHandler : IDisposable
	{
		event InputEventHandler InputDetected;

	    IList<InputType> ActiveInputs { set; }

	    void Listen(Control parent, IList<InputType> initialInputTypes);
	}
}
