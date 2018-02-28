using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Bot.Commands
{
	public class Main : BaseCommandModule
	{
		public static string DataFolder = Path.GetFullPath("./BotData/");

		[Command("info")]
		public async Task GetInfo(CommandContext ctx)
		{
			await ctx.RespondAsync("Web-Based Rework of Werewolves Bot");
		}
	}
}
