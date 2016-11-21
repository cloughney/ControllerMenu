using System.Windows.Forms;
using ControllerMenu.Input.Models;

namespace ControllerMenu.Input.Keyboard
{
	public class KeyboardInputHandler : IInputHandler
	{
	    private Control parentControl;

		public event InputEventHandler InputDetected;

		public void Listen(Control parent)
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