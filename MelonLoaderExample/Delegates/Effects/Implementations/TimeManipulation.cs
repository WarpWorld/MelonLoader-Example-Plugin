using ConnectorLib.JSON;
using UnityEngine;

namespace CrowdControl.Delegates.Effects.Implementations;

/// <summary>
/// == EXAMPLE - a timed effect where multiple codes conflict with each other ==
/// This effect only uses standard Unity APIs, so it works in any Unity game and ships live
/// as a working demonstration. It shows the standard shape of a timed effect:
///   - a duration and a conflicts list declared on the [Effect] attribute
///     (slowTime and speedUpTime conflict with each other AND with themselves, so only
///     one time manipulation can ever be active - the Scheduler enforces this locally)
///   - Start() applies the change and Stop() reverts it
///   - pausing/resuming and remaining-time reporting are handled automatically by TimedEffectState
/// </summary>
[Effect(
    ids: new[] { "slowTime", "speedUpTime" },
    defaultDuration: 15,
    conflicts: new[] { "slowTime", "speedUpTime" }
)]
public class TimeManipulation : Effect
{
    public TimeManipulation(CrowdControlMod mod, NetworkClient client) : base(mod, client) { }

    private float _previousTimeScale = 1.0f;

    public override EffectResponse Start(EffectRequest request)
    {
        try
        {
            _previousTimeScale = Time.timeScale;

            switch (request.code)
            {
                case "slowTime":
                    Time.timeScale = 0.5f; // Half speed
                    break;

                case "speedUpTime":
                    Time.timeScale = 2.0f; // Double speed
                    break;

                default:
                    return EffectResponse.Failure(request.ID, StandardErrors.EffectUnknown);
            }

            //request.GetViewerDisplayName() returns a sanitized viewer name that is safe to
            //show on screen or in logs (handles odd character sets, markup, etc)
            CrowdControlMod.Instance.Logger.Msg($"{request.GetViewerDisplayName()} started {request.code}");

            return EffectResponse.Success(request.ID);
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Time manipulation start error: {e}");
            return EffectResponse.Retry(request.ID);
        }
    }

    public override EffectResponse Stop(EffectRequest request)
    {
        try
        {
            Time.timeScale = _previousTimeScale;
        }
        catch (Exception e)
        {
            CrowdControlMod.Instance.Logger.Error($"Time manipulation stop error: {e}");
        }

        return EffectResponse.Finished(request.ID);
    }
}
