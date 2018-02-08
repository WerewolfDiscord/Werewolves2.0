using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Lib;
using Microsoft.AspNetCore.Hosting;
using Web.Models;

#region CrawlInfo
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    using Crawler;
//
//    var data = UpdateInfo.FromJson(jsonString);

namespace Lib
{
	using System;
	using System.Net;
	using System.Collections.Generic;

	using Newtonsoft.Json;
	using J = Newtonsoft.Json.JsonPropertyAttribute;

	public partial class CrawlInfo
	{
		[J("Guilds")] public List<Guild> Guilds { get; set; }

		public CrawlInfo()
		{
			Guilds = new List<Guild>();
		}
	}

	public partial class Guild
	{
		[J("id")] public ulong Id { get; set; }
		[J("Name")] public string Name { get; set; }
		[J("IconUrl")] public string IconURL { get; set; }
		[J("Channels")] public List<Channel> Channels { get; set; }

		public Guild()
		{
			Channels = new List<Channel>();
		}
	}

	public partial class Channel
	{
		[J("Name")] public string Name { get; set; }
		[J("Season")] public int Season { get; set; }
		[J("id")] public ulong Id { get; set; }
		[J("Messages")] public List<Message> Messages { get; set; }
		public Channel()
		{
			Messages = new List<Message>();
		}
	}

	public partial class Message
	{
		[J("Author")] public Author Author { get; set; }
		[J("Timestamp")] public long Timestamp { get; set; }
		[J("Content")] public string Content { get; set; }
		[J("id")] public ulong Id { get; set; }

		public Message()
		{
			Author = new Author();
		}
	}

	public partial class Author
	{
		[J("Name")] public string Name { get; set; }
		[J("IconURL")] public string IconURL { get; set; }
		[J("id")] public ulong Id { get; set; }
	}

	public partial class CrawlInfo
	{
		public static CrawlInfo FromJson(string json) => JsonConvert.DeserializeObject<CrawlInfo>(json, Converter.Settings);
	}

	public static class Serialize
	{
		public static string ToJson(this CrawlInfo self) => JsonConvert.SerializeObject(self, Converter.Settings);
	}

	public class Converter
	{
		public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
		{
			MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
			DateParseHandling = DateParseHandling.None,
			Formatting = Formatting.Indented
		};
	}

	public class MessageComparer : IComparer<Message>
	{
		public int Compare(Message x, Message y)
		{
			return (int)(x.Timestamp - y.Timestamp);
		}
	}
}
#endregion

namespace Web.Controllers
{

	public class ArchiveController : Controller
    {
		Dictionary<string, string> cache = new Dictionary<string, string>();

		private readonly string path;

		public ArchiveController(IHostingEnvironment env)
		{
			path = env.ContentRootPath
			   + Path.DirectorySeparatorChar.ToString()
			   + "wwwroot"
			   + Path.DirectorySeparatorChar.ToString();
		}

		CrawlInfo GetInfo(string guild, bool forceupdate)
		{
			var s = "";
			if (forceupdate || !(cache.TryGetValue(guild, out s)))
			{
				var s2 = System.IO.File.ReadAllText(path + guild + ".json");
				cache.Add(guild, s2);
				s = s2;
			}

			return CrawlInfo.FromJson(s);
		}

		public IActionResult Index(int season, string channel, string guild = "Werewolves", bool forceupdate = false)
		{
			var ci = GetInfo(guild, forceupdate);

			var v = ci.Guilds.First(x => x.Name == guild);
			var v2 = v.Channels.Where(x => x.Season == season);
			var v3 = v2.First(x => x.Name.ToLower() == channel.ToLower());

			return View(new ChannelModel()
			{
				c = v3,
				g = v
			});
        }

		public IActionResult ListAll(string guild = "Werewolves", bool forceupdate = false)
		{
			var ci = GetInfo(guild, forceupdate);
			var v = ci.Guilds.First(x => x.Name == guild);

			List<ChannelModel> l = new List<ChannelModel>();
			foreach (var v2 in v.Channels)
			{
				l.Add(new ChannelModel()
				{
					c = v2,
					g = v
				});
			}

			return View("List", l);
		}
		public IActionResult List(int season, string guild = "Werewolves", bool forceupdate = false)
		{
			var ci = GetInfo(guild, forceupdate);
			var v = ci.Guilds.First(x => x.Name == guild);

			List<ChannelModel> l = new List<ChannelModel>();
			foreach (var v2 in v.Channels.Where(x => x.Season == season))
			{
				l.Add(new ChannelModel()
				{
					c = v2,
					g = v
				});
			}

			return View(l);
		}
	}
}