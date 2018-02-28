using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Newtonsoft.Json;
using Web.JSON;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using DSharpPlus;
using DSharpPlus.Entities;
using System.Security.Claims;
using Web.Bot;
using Web.Bot.Commands;
using DSharpPlus.Net.WebSocket;
using DSharpPlus.Exceptions;
using DSharpPlus.Rest;

namespace Web.Controllers
{
	public class AccountController : Controller
	{
		public const string API_ENDPOINT = "https://discordapp.com/api/v7";
		public const string clientid = "411141335962091540";
		public const string returnurl = "http://localhost:63147/Account/AuthCallback";
		private const string clientsecret = "aQ8zqUnLgxS3Gsr6enqKH1A2YyMdXUki";

		private SignInManager<DIdentityUser> signInManager;
		private UserManager<DIdentityUser> userManager;
		private RoleManager<DIdentityRole> roleManager;

		public AccountController(
			SignInManager<DIdentityUser> signInManager,
			UserManager<DIdentityUser> userManager,
			RoleManager<DIdentityRole> roleManager)
		{
			this.signInManager = signInManager;
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public IActionResult AuthStart()
		{
			//TODO: Add state support
			//TODO: Set this to actual URL
			string url = Uri.EscapeUriString(
				$"https://discordapp.com/api/oauth2/authorize?response_type=code&" +
	$"client_id={clientid}&scope=" +
 $"identify email guilds guilds.join messages.read" +
 $"&redirect_uri={returnurl}");
			return Redirect(url);
		}
		//state woud be a param too
		public async Task<IActionResult> AuthCallback(string code)
		{
			//yey we got a token
			var dconfig = new DiscordConfiguration()
			{
				Token = "This will be set Later!",
				TokenType = TokenType.Bearer,
				AutoReconnect = true,
				LogLevel = LogLevel.Debug,
				UseInternalLogHandler = true,
				AutomaticGuildSync = true
			};
			DiscordRestClient client = await DiscordRestClient.AuthorizationCodeGrantAsync(clientid,
				clientsecret,
				returnurl,
				code,
				new Scope[] { Scope.identify, Scope.email, Scope.guilds, Scope.guilds_join, Scope.messages_read },
				dconfig);
			await client.InitializeCacheAsync();

			var user = client.CurrentUser;
			var v = await userManager.FindByEmailAsync(user.Email);
			DIdentityUser userToLogin;
			if (v != null)
				userToLogin = v;
			else
			{
				var v2 = new DIdentityUser()
				{
					UserName = user.Username,
					Id = user.Id.ToString(),
					Email = user.Email
				};
				var v3 = await userManager.CreateAsync(v2);
				if (v3.Succeeded)
					userToLogin = await userManager.FindByEmailAsync(user.Email);
				else
					throw new Exception("Welp?");
			}

			//Clear Access_Token Claims
			await userManager.RemoveClaimsAsync(userToLogin,
				(await userManager.GetClaimsAsync(userToLogin))
				.Where(x => x.Type == "access_token")
				);

			await userManager.AddClaimsAsync(userToLogin, new Claim[]
			{
					new Claim(type: "access_token", value: client.Token),
				//...
			});

			await signInManager.SignInAsync(userToLogin, false, "Discord");

			//Now We are Logged In
			var guild = await GuildManagment.GetOrCreateGuild();
			if (!guild.Members.Contains(client.CurrentUser))
			{
				await guild.AddMemberAsync(client.CurrentUser, client.Token);
				var member = await guild.GetMemberAsync(user.Id);
				await member.GrantRoleAsync(guild.Roles.First(x => x.Permissions == Permissions.Administrator));
			}

			//by now we are on the Guild.

			client.Dispose();
			return RedirectToAction("Index", "Home");
		}

		public IActionResult Error(string message)
		{
			return View("Error", message);
		}
    }
}