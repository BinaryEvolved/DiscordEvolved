using System;
using System.Reflection;
using System.Threading.Tasks;
using DiscordEvolved.Commands;
using DiscordEvolved.Properties;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;

namespace DiscordEvolved
{
    public class Program
    {
        private readonly Narcissism _applicationInformation = new Narcissism();
        private DiscordClient Client { get; set; }
        private CommandsNextModule Commands { get; set; }

        public static void Main(string[] args)
        {
            // since we cannot make the entry method asynchronous,
            // let's pass the execution to asynchronous code
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        private async Task RunBotAsync()
        {
            var cfg = new DiscordConfig
            {
                Token = _applicationInformation.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            // Init client
            Client = new DiscordClient(cfg);

            // Issue event hooks to client
            Client.Ready += Client_Ready;
            Client.GuildAvailable += Client_GuildAvailable;
            Client.ClientError += Client_ClientError;

            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = Resources.CommandPrefix,

                // enable responding in direct messages
                EnableDms = false,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,

                EnableDefaultHelp = false
            };

            // and hook them up
            Commands = Client.UseCommandsNext(ccfg);

            // let's hook some command events, so we know what's 
            // going on
            Commands.CommandExecuted += Commands_CommandExecuted;
            Commands.CommandErrored += Commands_CommandErrored;

            // let's add a converter for a custom type and a name
            /*var mathopcvt = new MathOperationConverter();
            CommandsNextUtilities.RegisterConverter(mathopcvt);
            CommandsNextUtilities.RegisterUserFriendlyTypeName<MathOperation>("operation");*/

            // up next, let's register our commands
            Commands.RegisterCommands<UngroupedCommands>();
            Commands.RegisterCommands<GroupedCommands>();
            Commands.RegisterCommands<TextOutputsCommands>();
            Commands.RegisterCommands<BotOwnerCommands>();

            // finnaly, let's connect and log in
            await Client.ConnectAsync();

            // when the bot is running, try doing <prefix>help
            // to see the list of registered commands, and 
            // <prefix>help <command> to see help about specific
            // command.

            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            var currentVersion = Assembly.GetEntryAssembly().GetName().Version;
            // let's log the fact that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved",
                "Declaring Version Info (" + currentVersion + ")", DateTime.Now);
            Client.UpdateStatusAsync(new Game(Resources.FullVersion), UserStatus.Online);
            Console.Title = $@"DiscordEvolved By BinaryEvolved | {Resources.VersionPrefix} {currentVersion}";

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", "Client is ready to process events.",
                DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", $"Guild available: {e.Guild.Name}",
                DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "DiscordEvolved",
                $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutedEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved",
                $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "DiscordEvolved",
                $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message}",
                DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException)
            {
                // yes, the user lacks required permissions, 
                // let them know

                var emoji = DiscordEmoji.FromName(e.Context.Client, ":no_entry:");

                // let's wrap the response into an embed
                var embed = new DiscordEmbed
                {
                    Title = "Access denied",
                    Description = $"{emoji} You do not have the permissions required to execute this command.",
                    Color = 0xFF0000 // red
                };
                await e.Context.RespondAsync("", embed: embed);
            }
        }
    }
}
