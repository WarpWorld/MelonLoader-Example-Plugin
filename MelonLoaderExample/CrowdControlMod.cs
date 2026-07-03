using CrowdControl;
using CrowdControl.Delegates.Effects;
using MelonLoader;
using UnityEngine;

//ALWAYS SET THIS FOR A NEW GAME! - the developer and game name MelonLoader should match against
//(find the values in the game's Game_Data\app.info file - e.g. [assembly: MelonGame("Ved", "Megabonk")] -
// or leave both null to allow the mod to load in any game)
[assembly: MelonGame(null, null)]
[assembly: MelonInfo(typeof(CrowdControlMod), CrowdControlMod.MOD_NAME, CrowdControlMod.MOD_VERSION, CrowdControlMod.MOD_DEVELOPER, "https://crowdcontrol.live/")]

namespace CrowdControl;

/// <summary>
/// The main Crowd Control mod class.
/// </summary>
public class CrowdControlMod : MelonMod
{
    // Mod Details - ALWAYS SET THESE FOR A NEW GAME!
    // (these must be compile-time constants for the [MelonInfo] attribute, so they can't live in the
    // csproj - the DLL name is set separately via the GameName property in MelonLoaderExample.csproj)
    public const string MOD_GUID = "WarpWorld.CrowdControl"; //unique mod ID - fine to leave as-is since only one Crowd Control mod is installed per game
    public const string MOD_DEVELOPER = "Warp World";
    public const string MOD_NAME = "Crowd Control for My Game"; //display name shown in the MelonLoader log - put your game's name here
    public const string MOD_VERSION = "1.0.0"; //bump this with each release of your mod

    /// <summary>The real-time duration of the current tick, used to advance timed effect countdowns.</summary>
    /// <remarks>
    /// Dividing by the time scale makes timed effects count down in real time even during slow motion.
    /// The time scale is checked to avoid Infinity/NaN corrupting effect timers in games that
    /// run FixedUpdate with the time scale at (or below) zero.
    /// Change this to use Time.deltaTime if ticking from OnUpdate instead of OnFixedUpdate.
    /// </remarks>
    public static float DeltaTime => (Time.timeScale > 0f) ? (Time.fixedDeltaTime / Time.timeScale) : 0f;

    private readonly HarmonyLib.Harmony harmony = new(MOD_GUID);

    /// <summary>The logger for the mod.</summary>
    public MelonLogger.Instance Logger => LoggerInstance;

    /// <summary>The singleton instance of the game mod.</summary>
    internal static CrowdControlMod Instance { get; private set; } = null!;

    /// <summary>The game state manager object.</summary>
    public GameStateManager GameStateManager { get; private set; } = null!;

    /// <summary>The effect class loader.</summary>
    public EffectLoader EffectLoader { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the client is connected.
    /// </summary>
    public bool ClientConnected => Client.Connected;

    public NetworkClient Client { get; private set; } = null!;

    public Scheduler Scheduler { get; private set; } = null!;

    private const double MANUAL_RECONNECT_COOLDOWN_SECONDS = 5.0;
    private DateTime m_nextManualReconnectAllowedUtc = DateTime.MinValue;

    /// <summary>
    /// Called when the mod is created.
    /// </summary>
    public override void OnInitializeMelon()
    {
        Instance = this;

        Logger.Msg($"Loaded {MOD_GUID}. Patching.");
        harmony.PatchAll();

        Logger.Msg("Initializing Crowd Control");

        try
        {
            GameStateManager = new(this);
            Client = new(this);
            EffectLoader = new(this, Client);
            Scheduler = new(this, Client);
        }
        catch (Exception e)
        {
            Logger.Error($"Crowd Control Init Error: {e}");
        }

        Logger.Msg("Crowd Control Initialized");
    }

    /// <summary>Called when the game is quitting normally. Shuts the connection down cleanly.</summary>
    public override void OnApplicationQuit()
    {
        try
        {
            Client?.Stop();
            Client?.Dispose();
        }
        catch {/**/}
    }

    /// <summary>Called when the mod is unloaded. Shuts the connection down cleanly.</summary>
    public override void OnDeinitializeMelon()
    {
        try
        {
            Client?.Stop();
            Client?.Dispose();
        }
        catch {/**/}
    }

    /// <summary>Called every fixed frame (physics) update.</summary>
    /// <remarks>This function is called on the main game thread. Blocking here may cause lag or crash the game entirely.</remarks>
    public override void OnFixedUpdate()
    {
        if (GameStateManager == null) return; //initialization failed - do nothing rather than throw every tick

        //recompute the game state once per tick (everything else this tick reads the cached value)
        //and report it if it changed - state changes reach the Crowd Control client within one tick
        GameStateManager.InvalidateStateCache();
        GameStateManager.UpdateGameState();

        Scheduler?.Tick();
    }

    private bool m_hadFocus = true;

    private void HandleManualReconnectHotkey()
    {
        if (!Input.GetKeyDown(KeyCode.F9))
            return;

        DateTime now = DateTime.UtcNow;
        if (now < m_nextManualReconnectAllowedUtc)
            return;

        m_nextManualReconnectAllowedUtc = now.AddSeconds(MANUAL_RECONNECT_COOLDOWN_SECONDS);
        Logger.Msg("F9 pressed - manual Crowd Control reconnect requested.");

        if (Client?.RequestReconnect() == true)
        {
            ShowGameUiMessage("Reconnecting to Crowd Control...");
            Logger.Msg("Manual Crowd Control reconnect queued.");
        }
        else
        {
            ShowGameUiMessage("Crowd Control client not found.");
            Logger.Msg("Manual Crowd Control reconnect skipped because the Crowd Control client was not found.");
        }
    }

    /// <summary>
    /// Displays a message to the player using the game's UI/toast system.
    /// </summary>
    /// <remarks>
    /// This is intentionally a no-op in the example pack because UI/toast APIs are game-specific.
    /// Wire this to your game's toast, subtitle, HUD message, or dialog system when available.
    /// </remarks>
    public void ShowGameUiMessage(string message)
    {
        //TODO: Replace this with your game's UI/toast call, e.g. ToastManager.Show(message).
    }

    /// <summary>Called every rendered frame.</summary>
    /// <remarks>
    /// MelonLoader mods don't receive Unity's OnApplicationFocus callback, so focus changes are detected here
    /// and a game state update is pushed immediately rather than waiting for the next physics tick - this
    /// matters because OnFixedUpdate may stop running entirely while the game is unfocused.
    /// </remarks>
    public override void OnUpdate()
    {
        try
        {
            HandleManualReconnectHotkey();

            bool hasFocus = Application.isFocused;
            if (hasFocus == m_hadFocus) return;
            m_hadFocus = hasFocus;

            GameStateManager?.InvalidateStateCache(); //the cached state predates the focus change
            GameStateManager?.UpdateGameState();
        }
        catch {/**/}
    }

    /***** == ONLY USE THIS IF OnFixedUpdate() ISN'T ALREADY BEING CALLED EVERY TICK == *****/
    //attach this to some game class with a function that runs every frame like the player's Update()
    //[HarmonyPatch(typeof(PlayerMovement), nameof(PlayerMovement.FixedUpdate))]
    //private class PlayerMovement_FixedUpdate { static void Prefix() => Instance.OnFixedUpdate(); }
}
