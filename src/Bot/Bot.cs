using Web.Bot.Commands;
using Web.Bot.Entities;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Net.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;

namespace Web.Bot
{
    //The Main Class From An Example.
    public class DiscordBot : IDisposable
    {
        public const string n = "\r\n";

        public static DiscordClient _client;
        public static CommandsNextExtension _cnext;
        private readonly CancellationTokenSource _cts;
        private readonly StartTimes _starttimes;
        private Config _config;
        public static InteractivityExtension _interactivity;

        public DiscordBot()
        {
			//TODO: Move Console.Write XY to Admin Board
            var path = Main.DataFolder + "config.json";
            if (!File.Exists(path))
            {
                if (!Directory.Exists(Path.GetDirectoryName(path)))
                    Directory.CreateDirectory(Path.GetDirectoryName(path) ?? throw new DirectoryNotFoundException());
                new Config().SaveToFile(Main.DataFolder + "config.json");

                #region !! Report to user that config has not been set yet !! (aesthetics)

                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;
                WriteCenter("▒▒▒▒▒▒▒▒▒▄▄▄▄▒▒▒▒▒▒▒", 2);
                WriteCenter("▒▒▒▒▒▒▄▀▀▓▓▓▀█▒▒▒▒▒▒");
                WriteCenter("▒▒▒▒▄▀▓▓▄██████▄▒▒▒▒");
                WriteCenter("▒▒▒▄█▄█▀░░▄░▄░█▀▒▒▒▒");
                WriteCenter("▒▒▄▀░██▄░░▀░▀░▀▄▒▒▒▒");
                WriteCenter("▒▒▀▄░░▀░▄█▄▄░░▄█▄▒▒▒");
                WriteCenter("▒▒▒▒▀█▄▄░░▀▀▀█▀▒▒▒▒▒");
                WriteCenter("▒▒▒▄▀▓▓▓▀██▀▀█▄▀▀▄▒▒");
                WriteCenter("▒▒█▓▓▄▀▀▀▄█▄▓▓▀█░█▒▒");
                WriteCenter("▒▒▀▄█░░░░░█▀▀▄▄▀█▒▒▒");
                WriteCenter("▒▒▒▄▀▀▄▄▄██▄▄█▀▓▓█▒▒");
                WriteCenter("▒▒█▀▓█████████▓▓▓█▒▒");
                WriteCenter("▒▒█▓▓██▀▀▀▒▒▒▀▄▄█▀▒▒");
                WriteCenter("▒▒▒▀▀▒▒▒▒▒▒▒▒▒▒▒▒▒▒▒");
                Console.BackgroundColor = ConsoleColor.Yellow;
                WriteCenter("WARNING", 3);
                Console.ResetColor();
                WriteCenter("Thank you Mario!", 1);
                WriteCenter("But our config.json is in another castle!");
                WriteCenter("(Please fill in the config.json that was generated.)", 2);
                WriteCenter($"({path})");
                WriteCenter("Press any key to exit..", 1);
                //Console.SetCursorPosition(0, 0); Doesnt Work here
                Console.ReadKey();

                #endregion

                Environment.Exit(0);
            }
            _config = Config.LoadFromFile(path);
			_client = new DiscordClient(new DiscordConfiguration
			{
				AutoReconnect = true,
				LogLevel = LogLevel.Debug,
				Token = _config.Token,
				TokenType = TokenType.Bot,
				UseInternalLogHandler = true,
				AutomaticGuildSync = true,
				WebSocketClientFactory = WebSocket4NetCoreClient.CreateNew // new
            });

			//OLD _client.SetWebSocketClient<WebSocket4NetCoreClient>();

			_interactivity = _client.UseInteractivity(new InteractivityConfiguration
            {
                PaginationBehavior = TimeoutBehaviour.DeleteMessage,
                PaginationTimeout = TimeSpan.FromSeconds(30),
                Timeout = TimeSpan.FromSeconds(30)
            });

            _starttimes = new StartTimes
            {
                BotStart = DateTime.UtcNow,
                SocketStart = DateTime.MinValue
            };

            _cts = new CancellationTokenSource();


            var dep = new ServiceCollection()
                .AddSingleton(new Dependencies
                {
                    Interactivity = _interactivity,
                    StartTimes = _starttimes,
                    Cts = _cts
                }).BuildServiceProvider();

            _cnext = _client.UseCommandsNext(new CommandsNextConfiguration
            {
                CaseSensitive = false,
                EnableDefaultHelp = true,
                EnableDms = true,
                EnableMentionPrefix = true,
                StringPrefixes = new List<string>()
				{
					_config.Prefix
				},
                IgnoreExtraArguments = true,
                Services = dep
            });

            RegisterCustomConverters(ref _cnext);

            _cnext.RegisterCommands<Main>();
            // _cnext.RegisterCommands<DSPlus.Examples.ExampleInteractiveCommands>();

            _client.Ready += OnReadyAsync;
            _client.SocketClosed += _client_SocketClosed;
            _cnext.CommandErrored += _cnext_CommandErrored;
            _client.DebugLogger.LogMessageReceived += DebugLogger_LogMessageReceived;
            _cnext.CommandExecuted += _cnext_CommandExecuted;
			//_client.UpdateStatusAsync(new DiscordActivity("for " + _config.Prefix + "help", ActivityType.Watching), UserStatus.Online);
			//Console.Write(Default3);
        }

        private void RegisterCustomConverters(ref CommandsNextExtension cnext)
        {
        }

        public void Dispose()
        {
            _client.Dispose();
            _interactivity = null;
            _cnext = null;
            _config = null;
        }

		private static async Task _cnext_CommandExecuted(CommandExecutionEventArgs e)
		{
            await e.Context.Message.DeleteAsync();
        }

        private static void DebugLogger_LogMessageReceived(object sender, DebugLogMessageEventArgs e)
        {
			if (e.Application != "REST")
			{
				//GameLog.Log($"[{e.Application}] [{e.Level}] {e.Message}", "Logger");
			}
        }

        private static async Task _cnext_CommandErrored(CommandErrorEventArgs e)
        {
            //await e.Context.Channel.SendMessageAsync(
            //    $"{e.Context.Message.Author.Mention}'s Command ```{Environment.NewLine}{e.Context.Message.Content}{Environment.NewLine}``` ERRORED SEE LOG");
            //await e.Context.Message.DeleteAsync();
            Console.WriteLine($"ERROR: {e.Command}, {e.Exception.Message}");
			//GameLog.Log($"[ERROR] {e.Command}, {e.Exception.Message}", "Commands");
		}

        private static Task _client_SocketClosed(SocketCloseEventArgs e)
        {
            return Task.Delay(0);
        }

        public async Task RunAsync()
        {
            await _client.ConnectAsync();
            await _client.UpdateCurrentUserAsync("2.0");
            await WaitForCancellationAsync();
        }

        private async Task WaitForCancellationAsync()
        {
            while (!_cts.IsCancellationRequested)
                await Task.Delay(500);
        }

        private async Task OnReadyAsync(ReadyEventArgs e)
        {
            await Task.Yield();
            _starttimes.SocketStart = DateTime.UtcNow;
        }

        private static void WriteCenter(string value, int skipline = 0)
        {

            for (var i = 0; i < skipline; i++)
                Console.WriteLine();

			//TODO: Fix this!
			//For Some Reason SetCursorPosition Errors?
			//Console.SetCursorPosition((Console.WindowWidth - value.Length) / 2, Console.CursorTop);
			Console.WriteLine(value);
        }
    }
}