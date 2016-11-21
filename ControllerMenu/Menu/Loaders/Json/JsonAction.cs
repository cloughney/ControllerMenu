using ControllerMenu.Actions;
using Newtonsoft.Json;

namespace ControllerMenu.Menu.Loaders.Json
{
	[JsonConverter(typeof(JsonActionConverter))]
	public class JsonAction
    {
        public string Type { get; set; }

        public IActionOptions Options { get; set; }
    }
}