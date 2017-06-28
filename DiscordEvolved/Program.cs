using System;
using System.Threading.Tasks;
using DiscordEvolved.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Exceptions;

namespace DiscordEvolved
{
    public class Program
    {
        public DiscordClient Client { get; set; }
        public CommandsNextModule Commands { get; set; }
        public readonly Narcissism ApplicationInformation = new Narcissism();

        public static void Main(string[] args)
        {
            // since we cannot make the entry method asynchronous,
            // let's pass the execution to asynchronous code
            var prog = new Program();
            prog.RunBotAsync().GetAwaiter().GetResult();
        }

        public async Task RunBotAsync()
        {
            // Load config file
            ApplicationInformation.LoadData();
            var cfg = new DiscordConfig
            {
                Token = ApplicationInformation.DiscordToken,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                LogLevel = LogLevel.Info,
                UseInternalLogHandler = true
            };

            // Init client
            this.Client = new DiscordClient(cfg);

            // Issue event hooks to client
            this.Client.Ready += this.Client_Ready;
            this.Client.GuildAvailable += this.Client_GuildAvailable;
            this.Client.ClientError += this.Client_ClientError;

            // up next, let's set up our commands
            var ccfg = new CommandsNextConfiguration
            {
                // let's use the string prefix defined in config.json
                StringPrefix = Properties.Resources.CommandPrefix,

                // enable responding in direct messages
                EnableDms = false,

                // enable mentioning the bot as a command prefix
                EnableMentionPrefix = true,

                EnableDefaultHelp = false
            };

            // and hook them up
            this.Commands = this.Client.UseCommandsNext(ccfg);

            // let's hook some command events, so we know what's 
            // going on
            this.Commands.CommandExecuted += this.Commands_CommandExecuted;
            this.Commands.CommandErrored += this.Commands_CommandErrored;

            // let's add a converter for a custom type and a name
            /*var mathopcvt = new MathOperationConverter();
            CommandsNextUtilities.RegisterConverter(mathopcvt);
            CommandsNextUtilities.RegisterUserFriendlyTypeName<MathOperation>("operation");*/

            // up next, let's register our commands
            this.Commands.RegisterCommands<UngroupedCommands>();
            this.Commands.RegisterCommands<GroupedCommands>();
            this.Commands.RegisterCommands<ExecCommands>();
            this.Commands.RegisterCommands<TextOutputsCommands>();
            this.Commands.RegisterCommands<BotOwnerCommands>();

            // finnaly, let's connect and log in
            await this.Client.ConnectAsync();

            // when the bot is running, try doing <prefix>help
            // to see the list of registered commands, and 
            // <prefix>help <command> to see help about specific
            // command.

            // and this is to prevent premature quitting
            await Task.Delay(-1);
        }

        private Task Client_Ready(ReadyEventArgs e)
        {
            var CurrentVersion = System.Reflection.Assembly.GetEntryAssembly().GetName().Version;
            // let's log the fact that this event occured
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", "Declaring Version Info (" + CurrentVersion + ")", DateTime.Now);
            Client.UpdateStatusAsync(new Game(Properties.Resources.FullVersion), UserStatus.Online);
            Console.Title = $"DiscordEvolved By BinaryEvolved | {Properties.Resources.VersionPrefix} {CurrentVersion}";

            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", "Client is ready to process events.", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(GuildCreateEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", $"Guild available: {e.Guild.Name}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Client_ClientError(ClientErrorEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Client.DebugLogger.LogMessage(LogLevel.Error, "DiscordEvolved", $"Exception occured: {e.Exception.GetType()}: {e.Exception.Message}", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private Task Commands_CommandExecuted(CommandExecutedEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Info, "DiscordEvolved", $"{e.Context.User.Username} successfully executed '{e.Command.QualifiedName}'", DateTime.Now);

            // since this method is not async, let's return
            // a completed task, so that no additional work
            // is done
            return Task.CompletedTask;
        }

        private async Task Commands_CommandErrored(CommandErrorEventArgs e)
        {
            // let's log the name of the guild that was just
            // sent to our client
            e.Context.Client.DebugLogger.LogMessage(LogLevel.Error, "DiscordEvolved", $"{e.Context.User.Username} tried executing '{e.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {e.Exception.GetType()}: {e.Exception.Message ?? "<no message>"}", DateTime.Now);

            // let's check if the error is a result of lack
            // of required permissions
            if (e.Exception is ChecksFailedException ex)
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