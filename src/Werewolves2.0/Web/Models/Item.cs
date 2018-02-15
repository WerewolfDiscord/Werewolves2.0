using Newtonsoft.Json;

namespace Web.Models
{
	public class Item
	{
		[JsonProperty("Name")]
		public string Name;
		[JsonProperty("Count")]
		public int Count;
		[JsonProperty("Id")]
		public long Id;
	}
}