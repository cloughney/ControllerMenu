using System.Collections.Generic;
using System.Diagnostics;

namespace ControllerMenu.Services
{
    public class ActiveWindowConfiguration
    {
        public List<object> MenuItems { get; set; }
    }

	public class ActiveWindowService : IActiveWindowService
	{
	    private readonly IDictionary<string, ActiveWindowConfiguration> configuredWindows;

	    public ActiveWindowService()
	    {
	        //TODO populate from service
	        this.configuredWindows = new Dictionary<string, ActiveWindowConfiguration>();
	    }

	    private string ProcessName
	    {
	        get { return this.ProcessId.HasValue ? Process.GetProcessById(this.ProcessId.Value).MainModule.FileName : null; }
	    }

	    public int? ProcessId { private get; set; }

	    public ActiveWindowConfiguration CurrentConfiguration
	    {
	        get
	        {
	            ActiveWindowConfiguration config;
	            return this.configuredWindows.TryGetValue(this.ProcessName, out config) ? config : null;
	        }
	    }
	}
}
