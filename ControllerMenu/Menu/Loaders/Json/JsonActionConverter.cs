using System;
using System.Linq;
using ControllerMenu.Actions;
using ControllerMenu.Actions.EndProcess;
using ControllerMenu.Actions.Launch;
using ControllerMenu.Actions.Navigation;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ControllerMenu.Menu.Loaders.Json
{
	public class JsonActionConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(JsonAction);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			IActionOptions options = null;
			var jsonObject = JObject.Load(reader);

			var actionType = jsonObject.Properties()
				.First(p => String.Equals("type", p.Name, StringComparison.OrdinalIgnoreCase))
				.Value.ToString()
				.ToLower();

			switch (actionType) //TODO a much better way of handling this
			{
				case "navigation":
					options = new NavigationActionOptions();
					break;
				case "launch":
					options = new LaunchActionOptions();
					break;
				case "close":
					options = new EndProcessActionOptions();
					break;
			}

			var optionsObject = jsonObject.Properties().First(p => String.Equals("options", p.Name, StringComparison.OrdinalIgnoreCase)).Value;
			serializer.Populate(optionsObject.CreateReader(), options);

			return new JsonAction
			{
				Type = actionType,
				Options = options
			};
		}
	}
}