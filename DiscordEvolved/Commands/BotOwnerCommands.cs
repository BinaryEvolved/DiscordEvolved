using System;
using System.Threading;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordEvolved.Commands
{
    internal class BotOwnerCommands
    {
        [Command("shutdown")]
        [Description("Shutsdown DiscordEvolved Bot")]
        [Aliases("killself")]
        [RequireOwner]
        public async Task ShutdownBot(CommandContext ctx)
        {
            //If BinaryEvolved
            if (ctx.Message.Author.Id == ctx.Client.CurrentApplication.Owner.Id)
            {
                await ctx.Message.CreateReactionAsync(DiscordEmoji.FromName(ctx.Client, ":sob:"));
                await ctx.Client.UpdateStatusAsync(new Game("Shutting Down.."), UserStatus.Invisible);
                await ctx.Client.DisconnectAsync();
                Thread.Sleep(2500);
                Environment.Exit(0);
            }
        }
    }
}
