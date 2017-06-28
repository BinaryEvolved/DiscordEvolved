using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordEvolved.Commands
{
    class TextOutputsCommands
    {
        private readonly Narcissism config = new Narcissism();

        [Command("help")]
        [Description("Information about the DiscordEvolved Bot")]
        [Aliases("wtf?")]
        public async Task Help(CommandContext ctx, [Description("Additional infomration for command")] params string[] command)
        {

            //Shows Typing Indicator
            await ctx.TriggerTypingAsync();

            

            switch (command.Length)
            {
                case 1:
                    //Help for a specific command
                    //TODO: Add ability to look up description and syntax for specific commands

                    //If listing all commands, send even if you don't have permissions for the section
                    if (command[0] == "listall")
                        await SendHelp(ctx, true);

                    break;


                default:
                    await SendHelp(ctx);

                    break;
            }
        }

        private async Task SendHelp(CommandContext ctx, bool listAll = false)
        {
            /*
             * This whole section is more of an example for how I would like the help section to work.
             * Ideally commands could be disable dynamically in a database or server-to-server by server owners
             * Permissions would be checked dynamically and displayed for the user who requested help
             * Any group section that the user doesn't have permissions for would be hidden
             */

            //Declare status emojis
            var check = DiscordEmoji.FromName(ctx.Client, ":white_check_mark:");
            var x = DiscordEmoji.FromName(ctx.Client, ":x:");
            var soon = DiscordEmoji.FromName(ctx.Client, ":clock8:");
            var noPerm = DiscordEmoji.FromName(ctx.Client, ":no_entry:");

            //List Commands
            var embed = new DiscordEmbed()
            {
                Title = "Help",
                Description = "The following commands available in the bot",
                Fields = new List<DiscordEmbedField>(),
                Color = 2162503
            };

            var statusInformation = new DiscordEmbedField
            {
                Inline = false,
                Name = "Status Information:",
                Value = $"`{check}`: Command Online and Ready | `{x}`: Command Unavailable\n" +
                        $"`{soon}`: Coming Soon | `{noPerm}`: Invalid Permissions (for user {ctx.Message.Author.Username})"
            };

            var publicCommands = new DiscordEmbedField
            {
                Inline = false,
                Name = "Public Commands",
                Value = $"`{check}help` `{check}about` `{check}uptime` `{check}ping` `{soon}whois` `{soon}serverwhois`"
            };

            //Checks Server Owner Permissions (sOp)
            var sOp = noPerm;
            if (ctx.Message.Channel.Guild.Owner.Id == ctx.Message.Author.Id)
                sOp = check;
            var guildOwner = new DiscordEmbedField
            {
                Inline = false,
                Name = "Server Owner Commands",
                Value = $"`{sOp}leaveserver`"
            };


            //Checks Bot Owner Command Permissions (oCp)
            var oCp = noPerm;
            if (ctx.Message.Author.Id == ctx.Client.CurrentApplication.Owner.Id)
                oCp = check;
            var ownerCommands = new DiscordEmbedField
            {
                Inline = false,
                Name = "Bot Owner Only Commands",
                Value = $"`{oCp}shutdown` `{oCp}disable` `{oCp}enable`"
            };



            embed.Fields.Add(statusInformation);
            embed.Fields.Add(publicCommands);
            if (listAll || sOp == check)
                embed.Fields.Add(guildOwner);
            if (listAll || oCp == check)
                embed.Fields.Add(ownerCommands);

            await ctx.RespondAsync("", false, embed);
        }

        [Command("about")]
        [Description("Information about the DiscordEvolved Bot")]
        [Aliases("version", "bot", "owner")]
        public async Task About(CommandContext ctx)
        {
            //Shows Typing Indicator
            await ctx.TriggerTypingAsync();

            var embed = new DiscordEmbed
            {
                Title = "DiscordEvolved",
                Description =
                    "The DiscordEvolved Bot is created by BinaryEvolved with the goal to create a best friend to use when managing your discord servers.\n" +
                    "This bot uses the DiscordSharpPlus Library: https://github.com/NaamloosDT/DSharpPlus (Licensed under MIT)",
                Color = 2162503
            };

            var field1 = new DiscordEmbedField
            {
                Name = "Version",
                Value = Properties.Resources.FullVersion,
                Inline = true
            };

            var field2 = new DiscordEmbedField
            {
                Name = "GitHub",
                Value = "https://github.com/BinaryEvolved/DiscordEvolved",
                Inline = true
            };

            var thumbnail = new DiscordEmbedThumbnail
            {
                Url = "https://discordapp.com/assets/eadcfed66c2178f6fbcadd4f0d849396.svg"
            };

            embed.Thumbnail = thumbnail;
            embed.Fields.Add(field1);
            embed.Fields.Add(field2);
            await ctx.RespondAsync("", false, embed);
        }

        [Command("uptime")]
        [Description("Displays uptime for DiscordEvolved bot")]
        public async Task Uptime(CommandContext ctx)
        {
            //Shows Typing Indicator
            await ctx.TriggerTypingAsync();

            var response = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            await ctx.RespondAsync($"The bot as been running for {response.Days} Days, {response.Hours} Hours, {response.Minutes} Minutes, {response.Seconds} Seconds, and {response.Milliseconds} Milliseconds");
        }
    }
}
