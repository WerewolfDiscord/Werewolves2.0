using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class Trade
    {
		[JsonProperty("Name")]
		public string Name { get; set; }
		[JsonProperty("ToSell")]
		public Item ToSell;
		[JsonProperty("Price")]
		public Item Price;
		[JsonProperty("seller")]
		public DIdentityUser seller;

		public string ToJson()
		{
			return JsonConvert.SerializeObject(this);
		}

		public static Trade FromJson(string json)
		{
			return JsonConvert.DeserializeObject<Trade>(json);
		}
    }
}
