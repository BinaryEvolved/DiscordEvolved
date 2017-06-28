using System.Collections.Generic;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
// ReSharper disable UnusedMember.Global

namespace DiscordEvolved.Commands
{
    internal class UngroupedCommands
    {
        [Command("ping")] // let's define this method as a command
        [Description("Example ping command")] // this will be displayed to tell users what this command does when they invoke help
        [Aliases("pong")] // alternative names for the command
        public async Task Ping(CommandContext ctx) // this command takes no arguments
        {
            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":ping_pong:");

            // respond with ping
            await ctx.RespondAsync($"{emoji} Pong! Ping: {ctx.Client.Ping}ms");
        }

        [Command("greet"), Description("Says hi to specified user."), Aliases("sayhi", "say_hi")]
        public async Task Greet(CommandContext ctx, [Description("The user to say hi to.")] DiscordMember member) // this command takes a member as an argument; you can pass one by username, nickname, id, or mention
        {
            // note the [Description] attribute on the argument.
            // this will appear when people invoke help for the
            // command.

            // let's trigger a typing indicator to let
            // users know we're working
            await ctx.TriggerTypingAsync();

            // let's make the message a bit more colourful
            var emoji = DiscordEmoji.FromName(ctx.Client, ":wave:");

            // and finally, let's respond and greet the user.
            await ctx.RespondAsync($"{emoji} Hello, {member.Mention}!");
        }

        [Command("whois")]
        [Aliases("who")]
        public async Task Whois(CommandContext ctx, [Description("The user to lookup")] DiscordMember member)
        {
            await ctx.TriggerTypingAsync();

            if (member == null)
                return;

            var embed = new DiscordEmbed
            {
                Title = $"Who is {member.DisplayName}?",
                Fields = new List<DiscordEmbedField>(),
                Color = 2162503
            };

            var discordUserIcon = new DiscordEmbedThumbnail {Url = member.AvatarUrl};
            embed.Thumbnail = discordUserIcon;

            var discordID = new DiscordEmbedField
            {
                Name = "Discord ID",
                Inline = true,
                Value = member.Id.ToString()
            };
            string nick;
            nick = member.Nickname ?? "None";
            var NickName = new DiscordEmbedField
            {
                Name = "Nickname",
                Inline = true,
                Value = nick
            };
            var displayName = new DiscordEmbedField
            {
                Name = "DisplayName",
                Inline = true,
                Value = member.DisplayName
            };
            var joinDate = new DiscordEmbedField
            {
                Name = "Joined Discord",
                Inline = true,
                Value = member.JoinedAt.ToString("f")
            };
            var userStatus = new DiscordEmbedField
            {
                Name = "Status",
                Inline = true,
                Value = member.Presence.Status
            };
            

            embed.Fields.Add(discordID);
            embed.Fields.Add(userStatus);
            embed.Fields.Add(displayName);
            embed.Fields.Add(NickName);
            embed.Fields.Add(joinDate);

            await ctx.RespondAsync("", false, embed);
        }
    }
}

