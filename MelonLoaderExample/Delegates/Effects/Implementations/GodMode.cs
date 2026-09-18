namespace CrowdControl.Delegates.Effects.Implementations;

/* == EXAMPLE (Anger Foot) - a timed effect that toggles a game flag on start/stop ==
 * This demonstrates the standard shape of a timed effect:
 *   - a duration and selfConflict declared on the [Effect] attribute
 *   - Start() applies the change and Stop() reverts it
 *   - pausing/resuming and time tracking are handled automatically by TimedEffectState
 * It also demonstrates request.GetViewerDisplayName(), which returns a sanitized viewer
 * name that is safe to show on screen (handles odd character sets, markup, etc), and
 * Mod.ShowGameUiMessage(), which shows a line on the mod's overlay (and, if you wire it up,
 * your game's own dialog system - see DialogMsgAsync in GameStateManager.cs).
 * Uncomment and adapt this for your game.

using ConnectorLib.JSON;

//the selfConflict flag is set true here for clarity, but it's actually the default value if the duration is greater than 0
[Effect(
    id: "god_mode",
    defaultDuration: 30,
    selfConflict: true)
]
public class GodMode : Effect
{
    public GodMode(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        //setting selfConflict to true above means that this should only hit if the player is just playing with god mode on
        //this would never run if the effect were already running since it would be blocked by the selfConflict check
        //as such, we assume a player just playing with god mode on isn't going to be turning it off in a few seconds
        //and just fail here rather than retrying waiting for something that probably isn't happening
        if (Cheats.GodMode) return EffectResponse.Failure(request.ID, StandardErrors.AlreadyInState);

        Cheats.GodMode = true;
        Mod.ShowGameUiMessage($"{request.GetViewerDisplayName()} enabled God Mode");

        return EffectResponse.Success(request.ID);
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        //already off somehow(?), just abort with success since our job here is done
        if (!Cheats.GodMode) return EffectResponse.Finished(request.ID);

        Cheats.GodMode = false;
        Mod.ShowGameUiMessage("God Mode Ended");

        return EffectResponse.Finished(request.ID);
    }
}

*/
