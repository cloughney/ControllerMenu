namespace ControllerMenu.Services
{
	public interface IActiveWindowService
	{
	    int? ProcessId { set; }
	    ActiveWindowConfiguration CurrentConfiguration { get;  }
	}
}