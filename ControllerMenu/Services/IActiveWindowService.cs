using ControllerMenu.Models;

namespace ControllerMenu.Services
{
	public interface IActiveWindowService
	{
	    ActiveWindowConfiguration CurrentConfiguration { get;  }
	}
}