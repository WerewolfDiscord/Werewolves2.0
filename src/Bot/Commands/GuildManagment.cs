using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Web.Bot.Entities;

namespace Web.Bot.Commands
{
	public class GuildManagment : BaseCommandModule
	{
		private Dependencies dep;

		private static DiscordGuild mainGuild;
		public GuildManagment(Dependencies dep)
		{
			this.dep = dep;
		}

		public static async Task<DiscordGuild> GetOrCreateGuild()
		{
#if DEBUG
			if (mainGuild == null)
				foreach (var v in DiscordBot._client.Guilds.ToArray())
					if (v.Value.IsOwner)
						try { await v.Value.DeleteAsync(); } catch { }
					else
						try { await v.Value.LeaveAsync(); } catch { }
#endif
			if (mainGuild == null)
			{
				mainGuild = await DiscordBot._client.CreateGuildAsync("Marketplace");
				var admin = await mainGuild.CreateRoleAsync("Admin", DSharpPlus.Permissions.Administrator, DiscordColor.SpringGreen);
			}
			return mainGuild;
		}
	}
}
