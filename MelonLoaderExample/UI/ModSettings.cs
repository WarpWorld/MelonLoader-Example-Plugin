using MelonLoader;

namespace CrowdControl.UI;

/// <summary>
/// User-facing mod settings, persisted by MelonLoader to
/// <c>UserData\MelonPreferences.cfg</c> so a streamer can turn the on-screen pieces off.
/// </summary>
/// <remarks>
/// Everything here defaults to ON. The overlay is genuinely useful - knowing at a glance whether
/// Crowd Control is connected saves a lot of "is it broken?" - but it is drawn over someone's
/// stream, so it must be possible to opt out without touching the DLL.
/// </remarks>
public static class ModSettings
{
    private const string CATEGORY = "CrowdControl";

    private static MelonPreferences_Entry<bool> _showMessages;
    private static MelonPreferences_Entry<bool> _showIndicator;
    private static MelonPreferences_Entry<float> _messageSeconds;

    /// <summary>Show a line on screen when an effect fires.</summary>
    public static bool ShowMessages => _showMessages?.Value ?? true;

    /// <summary>Show the small connection dot.</summary>
    public static bool ShowIndicator => _showIndicator?.Value ?? true;

    /// <summary>How long each on-screen message stays up, in seconds.</summary>
    public static float MessageSeconds => _messageSeconds?.Value ?? 4f;

    /// <summary>Creates the settings category. Safe to call more than once.</summary>
    public static void Initialize()
    {
        try
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory(CATEGORY, "Crowd Control");

            _showMessages = category.CreateEntry(
                "ShowMessages", true,
                "Show effect messages on screen",
                "Displays a short line when an effect fires. Turn off for a clean capture.");

            _showIndicator = category.CreateEntry(
                "ShowConnectionIndicator", true,
                "Show connection indicator",
                "Small dot showing whether the mod is connected to the Crowd Control app. " +
                "Green connected, red not.");

            _messageSeconds = category.CreateEntry(
                "MessageSeconds", 4f,
                "Seconds to show each message",
                "How long an on-screen effect message stays visible.");
        }
        catch (Exception e)
        {
            //settings are a convenience - never let them stop the mod loading
            CrowdControlMod.Instance?.Logger.Warning($"Could not create settings: {e.Message}");
        }
    }
}
