using System.Collections.Generic;
using System.Windows.Forms;
using ControllerMenu.Input.Models;

namespace ControllerMenu.Input.Keyboard
{
	public class KeyboardInputHandler : IInputHandler
	{
	    private Control parentControl;
	    private readonly List<InputType> activeInputTypes;

	    public KeyboardInputHandler()
	    {
	        this.activeInputTypes = new List<InputType>();
	    }

	    public event InputEventHandler InputDetected;

	    public IList<InputType> ActiveInputs
	    {
	        set
	        {
	            this.activeInputTypes.Clear();
	            this.activeInputTypes.AddRange(value);
	        }
	    }

	    public void Listen(Control parent, IList<InputType> initialInputTypes)
		{
		    this.parentControl = parent;
			parent.KeyDown += this.OnKeyDown;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (this.InputDetected == null)
			{
				return;
			}

		    //TODO listen to active inputs?

			switch (e.KeyCode)
			{
				case Keys.Escape:
					this.InputDetected(this, InputType.Back);
					break;

				case Keys.Up:
					this.InputDetected(this, InputType.PreviousItem);
					break;

				case Keys.Down:
					this.InputDetected(this, InputType.NextItem);
					break;

				case Keys.Enter:
					this.InputDetected(this, InputType.SelectItem);
					break;
			}
		}

	    public void Dispose()
	    {
	        if (this.parentControl != null)
	        {
	            this.parentControl.KeyDown -= this.OnKeyDown;
	        }
	    }
	}
}