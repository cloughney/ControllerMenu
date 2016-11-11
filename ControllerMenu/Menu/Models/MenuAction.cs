using System;

namespace ControllerMenu.Menu.Models
{
    public class MenuAction
    {
        private readonly Action action;

        public MenuAction(Action action)
        {
            //TODO inject context object?

            this.action = action;
        }

        public void Perform()
        {
            this.action.Invoke();
        }
    }
}