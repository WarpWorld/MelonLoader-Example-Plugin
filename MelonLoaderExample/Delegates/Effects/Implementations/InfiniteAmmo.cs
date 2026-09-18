namespace CrowdControl.Delegates.Effects.Implementations;

/* == EXAMPLE (Anger Foot) - a timed effect that toggles a game flag on start/stop ==
 * See GodMode.cs for a full explanation of this pattern.
 * Uncomment and adapt this for your game.

using ConnectorLib.JSON;

//the selfConflict flag is set true here for clarity, but it's actually the default value if the duration is greater than 0
[Effect(
    id: "infinite_ammo",
    defaultDuration: 30,
    selfConflict: true)
]
public class InfiniteAmmo : Effect
{
    public InfiniteAmmo(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        //setting selfConflict to true above means that this should only hit if the player is just playing with infinite ammo on
        //this would never run if the effect were already running since it would be blocked by the selfConflict check
        //as such, we assume a player just playing with infinite ammo on isn't going to be turning it off in a few seconds
        //and just fail here rather than retrying waiting for something that probably isn't happening
        if (Cheats.InfiniteAmmo) return EffectResponse.Failure(request.ID, StandardErrors.AlreadyInState);

        Cheats.InfiniteAmmo = true;
        Mod.ShowGameUiMessage($"{request.GetViewerDisplayName()} enabled Infinite Ammo");

        return EffectResponse.Success(request.ID);
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        //already off somehow(?), just abort with success since our job here is done
        if (!Cheats.InfiniteAmmo) return EffectResponse.Finished(request.ID);

        Cheats.InfiniteAmmo = false;
        Mod.ShowGameUiMessage("Infinite Ammo Ended");

        return EffectResponse.Finished(request.ID);
    }
}

*/
