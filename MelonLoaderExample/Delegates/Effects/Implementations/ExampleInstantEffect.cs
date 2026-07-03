namespace CrowdControl.Delegates.Effects.Implementations;

/* == EXAMPLE - the standard shape of an instant (non-timed) effect ==
 * Copy this skeleton for each of your game's instant effects, replacing the placeholder
 * game calls with your game's real APIs (from Assembly-CSharp / the Il2Cpp assemblies).
 * It demonstrates:
 *   - one effect class handling multiple effect codes ([Effect("healPlayer", "hurtPlayer")])
 *   - reading the quantity the viewer purchased from request.quantity
 *   - failing cleanly with a specific StandardErrors value (or a human-readable reason)
 *     when the effect can't run - the viewer gets refunded with an explanation
 *   - wrapping game calls in try/catch and reporting errors back to the client

using ConnectorLib.JSON;

[Effect("healPlayer", "hurtPlayer")]
public class ExampleInstantEffect : Effect
{
    public ExampleInstantEffect(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            //look up the game objects you need and fail cleanly if they aren't available
            //(the game state was checked by GameStateManager.IsReady before this was called,
            // but individual objects can still be missing)
            var player = Player.Instance; //<-- your game's player class
            if (player == null)
                return EffectResponse.Failure(request.ID, StandardErrors.PlayerNotFound);

            //quantity is how many of the effect the viewer purchased (for quantity-enabled effects)
            float quantity = request.quantity ?? 1f;

            switch (request.code)
            {
                case "healPlayer":
                    if (player.health >= player.maxHealth)
                        return EffectResponse.Failure(request.ID, StandardErrors.AlreadyMaximum);
                    player.Heal(quantity);
                    break;

                case "hurtPlayer":
                    if (player.health <= 1)
                        return EffectResponse.Failure(request.ID, StandardErrors.AlreadyMinimum);
                    player.Damage(quantity);
                    break;

                default:
                    return EffectResponse.Failure(request.ID, StandardErrors.EffectUnknown);
            }

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Effect error: {e}");
            return EffectResponse.Failure(request.ID, StandardErrors.ExceptionThrown);
        }
    }
}

*/
