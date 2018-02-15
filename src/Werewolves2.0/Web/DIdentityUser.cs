using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web
{
    public class DIdentityUser : IdentityUser<string>
    {
		[JsonProperty("DiscordId")]
		public long DiscordId;
		[JsonProperty("AccessToken")]
		public string AccessToken;
    }
}
