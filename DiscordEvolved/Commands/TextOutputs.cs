using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace DiscordEvolved.Commands
{
    class TextOutputsCommands
    {
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
                    "The DiscordEvolved Bot is created by BinaryEvolved with the goal to create a best friend to use when managing your discord servers.",
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
                Value = "Coming Soon",
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
    }
}
