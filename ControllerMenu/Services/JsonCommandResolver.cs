using System.Collections.Generic;
using System.IO;
using ControllerMenu.Commands;
using Newtonsoft.Json;

namespace ControllerMenu.Services
{
	public interface ICommandResolver
	{
		List<Command> Resolve();
	}

	public class JsonCommandResolver : ICommandResolver
	{
		public List<Command> Resolve()
		{
			var configRaw = File.ReadAllText("commands.json");
			var config = JsonConvert.DeserializeObject<CommandConfiguration>(configRaw);

			return null;
		}
	}
}
