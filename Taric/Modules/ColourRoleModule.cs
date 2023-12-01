using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Taric.Extensions;
using Taric.Utility;

namespace Taric.Modules;

public class ColourRoleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("setcolour", "Changes the colour of your name")]
    public async Task SetColourRole(
        string colourHex)
    {
        if (!colourHex.IsValidHexString())
        {
            await RespondAsync("That is an invalid colour, " +
                               "try generating one here https://htmlcolorcodes.com/color-picker/", ephemeral: true);
            return;
        }
        
        if (colourHex.ToColour() == Constants.GameRoleColor)
        {
            await RespondAsync($"That's a very specific colour... You're not allowed to use that... Stop.");
            return;
        }
        
        await DeferAsync(ephemeral: true);

        await RemoveNonGameColoredRolesFromUser(Context.User);
        
        var newRole = await Context.Guild.CreateRoleAsync(
            Context.User.GlobalName,
            GuildPermissions.None,
            colourHex.ToColour() );
        
        await Task.Delay(500);
        
        await newRole.ModifyAsync(x =>
            x.Position = Context.Guild.Roles.Count(r => r.Color == Constants.GameRoleColor) + 2);

        await Task.Delay(500);
        
        await ((IGuildUser)Context.User).AddRoleAsync(newRole);

        await FollowupAsync($"Colour set to {colourHex}", ephemeral: true);
    }
    
    private async Task RemoveNonGameColoredRolesFromUser(SocketUser contextUser)
    {
        var colored =
            Context.Guild.Roles.Where(
                role =>
                    role.Color != Constants.GameRoleColor &&
                    role.Color != Color.Default &&
                    role.Members.Contains(contextUser));

        foreach (var role in colored)
        {
            await Context.Guild.GetRole(role.Id).DeleteAsync();
        }
    }
}