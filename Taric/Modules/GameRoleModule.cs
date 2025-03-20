using Discord;
using Discord.Interactions;
using Taric.Utility;

namespace Taric.Modules;

public class GameRoleModule : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("togglegame", "Adds or removes a game role to you")]
    public async Task ToggleGameRole(
        IRole game)
    {
        if (game.Color != Constants.GameRoleColor)
        {
            await RespondAsync($"Not adding {game.Name}, that's not a game role :|", ephemeral:true);
            return;
        }

        if (Context.User is IGuildUser user)
        {
            await DeferAsync();

            if (!Context.Guild.Roles.First(r => r.Id.Equals(game.Id)).Members.Contains(user))
            {
                await user.AddRoleAsync(game);
                await FollowupAsync($"Added {game.Name} to {user.GlobalName}");
            }
            else
            {
                await user.RemoveRoleAsync(game);
                await FollowupAsync($"Removed {game.Name} from {user.GlobalName}");
            }
        }

        await Task.Delay(500);
        
        // await DeleteEmptyGameRoles();
    }

    [SlashCommand("creategame", "Creates a game role for pinging people")]
    public async Task CreateGameRole(
        [MaxLength(30)] string name)
    {
        name = name.Trim();

        var gameRole = Context.Guild.Roles
            .FirstOrDefault(r =>
                r.Color == Constants.GameRoleColor && r.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        if (gameRole != null)
        {
            await RespondAsync($"I won't make that, {gameRole.Name} already exists.", ephemeral:true);
            return;
        }

        await DeferAsync(ephemeral:false);

        var newRole = await Context.Guild.CreateRoleAsync(
            name,
            GuildPermissions.None,
            Constants.GameRoleColor,
            false,
            true);

        await Task.Delay(500);

        await ((IGuildUser)Context.User).AddRoleAsync(newRole);

        await FollowupAsync($"Created GameRole for {name}");
    }

    private async Task DeleteEmptyGameRoles()
    {
        var empty =
            Context.Guild.Roles.Where(
                r => r.Color == Constants.GameRoleColor && !r.Members.Any());

        foreach (var role in empty)
        {
            await Context.Guild.GetRole(role.Id).DeleteAsync();
        }
    }
}