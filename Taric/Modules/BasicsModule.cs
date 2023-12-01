using Discord;
using Discord.Interactions;

namespace Taric.Modules
{
    public class BasicsModule : InteractionModuleBase<SocketInteractionContext>
    {
        private readonly Random _rnd = new();

        [SlashCommand("say", "Make the bot say something.")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task Say(
            string text)
        {
            await RespondAsync("Sending...", ephemeral: true);
            await Context.Channel.SendMessageAsync(text);
        }

        [SlashCommand("roll", "Roll a dice")]
        public async Task Roll(
            [MinValue(0)] [MaxValue(9999)] int d)
        {
            await RespondAsync(Context.User.Username + $" rolled a d{d} for: {_rnd.Next(d) + 1}");
        }
    }
}