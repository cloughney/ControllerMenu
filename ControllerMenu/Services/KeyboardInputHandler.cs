using System.Windows.Forms;

namespace ControllerMenu.Services
{
	public class KeyboardInputHandler : IInputHandler
	{
		public event InputEventHandler InputDetected;

		public void Listen(Control parent)
		{
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
	}
}