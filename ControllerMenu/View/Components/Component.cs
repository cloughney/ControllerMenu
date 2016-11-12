using System.Windows.Forms;

namespace ControllerMenu.View.Components
{
	public abstract class Component<TControl> 
		where TControl : Control
	{
		protected TControl Control { get; set; }

		public virtual void Attach(Control parent)
		{
			parent.Controls.Add(this.Control);
		}
	}
}