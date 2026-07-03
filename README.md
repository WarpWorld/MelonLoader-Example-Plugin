Example project for setting up a MelonLoader (Il2Cpp) mod to connect a game to Crowd Control

The project builds and runs as-is without any game-specific code. Game-specific pieces
(effects, metadata, and game state checks) are included as generic skeletons marked with
`== EXAMPLE ==` - adapt them to your game's APIs.

Instructions:

1) Name the Project for Your Game  
	All settings are in the `ALWAYS SET THESE FOR A NEW GAME!` blocks:  
	- `GameName` in `MelonLoaderExample\MelonLoaderExample.csproj` - names the output DLL (e.g. `CrowdControl.Megabonk.dll`)  
	- `MOD_NAME` (and `MOD_VERSION`) in `MelonLoaderExample\CrowdControlMod.cs` - the display name shown in the MelonLoader log  
	- the `[assembly: MelonGame(...)]` attribute in `MelonLoaderExample\CrowdControlMod.cs` - the developer/game
	  names MelonLoader matches against (find them in the game's `Game_Data\app.info` file, or leave null to match any game)  
	(`MOD_NAME` can't be moved into the csproj because the MelonLoader attributes require compile-time constants.)  
	`MOD_GUID` can stay as-is - only one Crowd Control mod is installed per game.

2) Update References  
	Set `GameBaseDir` in `MelonLoaderExample\MelonLoaderExample.csproj` to your game's install folder.  
	The MelonLoader and Il2Cpp assembly references resolve from there automatically
	(run the game once with MelonLoader installed so the `MelonLoader\Il2CppAssemblies` folder is generated).  
	Add references to any additional game assemblies your effects need.

3) Create Effect Functions  
	`Delegates\Effects\Implementations\` contains the classes implementing effects:  
	- `TimeManipulation.cs` - a live, working timed effect (it only uses standard Unity APIs,
	  so it runs in any Unity game) demonstrating durations and effect conflicts  
	- `ExampleInstantEffect.cs` - a commented skeleton for instant (non-timed) effects showing
	  multiple codes per class, quantities, and clean failure reporting

4) Create Timed Effects  
	Timed effects are any effects with a `defaultDuration` on their `[Effect]` attribute.  
	Pausing while the game is busy, resuming, and reporting the remaining time to the
	Crowd Control client are all handled automatically by `TimedEffectState`.

5) Setup IsReady & GetGameState Functions  
	`GameStateManager.cs` contains functions called `IsReady` and `GetGameState`.  
	`IsReady` returns a boolean indicating whether the game is in a state ready to execute effects.  
	`GetGameState` returns the current game state (Ready, Paused, NotFocused, Menu, Loading, ...).  
	State changes are automatically reported to the Crowd Control client as they happen;
	add your game-specific checks where marked with TODO.

6) Define Metadata (Optional)  
	`Delegates\Metadata\MetadataDelegates.cs` contains the metadata delegates.  
	Static methods tagged `[Metadata("key")]` answer `DataRequest` queries from the client,
	and any keys listed in `CommonMetadata` are attached to every effect response.

7) Attach Action Queue (Uncommon)  
	In rare cases, the OnFixedUpdate() method of the mod is not called automatically as part of the standard game loop.  
	In `CrowdControlMod.cs` there is an example harmony patch to attach to the FixedUpdate() function of some universal object.  
	This should be used if and only if the OnFixedUpdate() method is not called automatically.

Displaying viewer names:  
	Viewer names come from external services and may contain characters your game can't render
	(emoji, control characters, rich-text markup, etc). Use `request.GetViewerDisplayName()`
	(from `EffectRequestEx.cs`) instead of reading `request.viewer` directly - it returns a
	sanitized name and falls back to "the crowd" when no usable name is present.

`CrowdControlMod.Instance.Client` offers helper functions for hiding or disabling effects on the menu:  
	`ShowEffects(params string[] codes)` / `ShowAllEffects()`  
	`HideEffects(params string[] codes)` / `HideAllEffects()`  
	`EnableEffects(params string[] codes)` / `EnableAllEffects()`  
	`DisableEffects(params string[] codes)` / `DisableAllEffects()`  
	Async variants of all of the above are also available.
