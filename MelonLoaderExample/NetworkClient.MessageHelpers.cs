using System.Diagnostics.CodeAnalysis;
using ConnectorLib.JSON;
using CrowdControl.Delegates.Metadata;

namespace CrowdControl;

[SuppressMessage("ReSharper", "InvalidXmlDocComment")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
public partial class NetworkClient
{
    /// <summary>Attaches common metadata to the specified effect response.</summary>
    public void AttachMetadata(EffectResponse response)
    {
        response.metadata = new();
        foreach (string key in MetadataDelegates.CommonMetadata)
        {
            if (MetadataLoader.Metadata.TryGetValue(key, out MetadataDelegate del))
                response.metadata.Add(key, del.Invoke(m_mod));
            else
                CrowdControlMod.Instance.Logger.Error($"Metadata delegate \"{key}\" could not be found. Available delegates: {string.Join(", ", MetadataLoader.Metadata.Keys)}");
        }
    }
    
    #region Show Effects

    /// <summary>Shows the specified effects on the menu.</summary>
    /// <param name="codes">The effect IDs to show.</param>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool ShowEffects(params string[] codes) => Send(new EffectUpdate(codes, EffectStatus.Visible));

    /// <inheritdoc cref="ShowEffects(string[])"/>
    public bool ShowEffects(IEnumerable<string> codes, string message = null) => Send(new EffectUpdate(codes, EffectStatus.Visible, message));

    /// <inheritdoc cref="ShowEffects(string[])"/>
    /// <summary>Asynchronously shows the specified effects on the menu.</summary>
    public Task<bool> ShowEffectsAsync(params string[] codes) => SendAsync(new EffectUpdate(codes, EffectStatus.Visible));

    /// <inheritdoc cref="ShowEffectsAsync(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public Task<bool> ShowEffectsAsync(IEnumerable<string> codes, string message = null) => SendAsync(new EffectUpdate(codes, EffectStatus.Visible, message));

    /// <summary>Shows all effects on the menu.</summary>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool ShowAllEffects() => ShowEffects(m_mod.EffectLoader.EffectIDs);

    /// <inheritdoc cref="ShowAllEffects()"/>
    /// <summary>Asynchronously shows all effects on the menu.</summary>
    public Task<bool> ShowAllEffectsAsync() => ShowEffectsAsync(m_mod.EffectLoader.EffectIDs);

    #endregion
    
    #region Hide Effects
    
    /// <summary>Hides the specified effects on the menu.</summary>
    /// <param name="codes">The effect IDs to hide.</param>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool HideEffects(params string[] codes) => Send(new EffectUpdate(codes, EffectStatus.NotVisible));

    /// <inheritdoc cref="HideEffects(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public bool HideEffects(IEnumerable<string> codes, string message = null) => Send(new EffectUpdate(codes, EffectStatus.NotVisible, message));

    /// <inheritdoc cref="HideEffects(string[])"/>
    /// <summary>Asynchronously hides the specified effects on the menu.</summary>
    public Task<bool> HideEffectsAsync(params string[] codes) => SendAsync(new EffectUpdate(codes, EffectStatus.NotVisible));
    
    /// <inheritdoc cref="HideEffectsAsync(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public Task<bool> HideEffectsAsync(IEnumerable<string> codes, string message = null) => SendAsync(new EffectUpdate(codes, EffectStatus.NotVisible, message));

    /// <summary>Hides all effects on the menu.</summary>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool HideAllEffects() => HideEffects(m_mod.EffectLoader.EffectIDs);

    /// <inheritdoc cref="HideAllEffects()"/>
    /// <summary>Asynchronously hides all effects on the menu.</summary>
    public Task<bool> HideAllEffectsAsync() => HideEffectsAsync(m_mod.EffectLoader.EffectIDs);

    #endregion

    #region Enable Effects

    /// <summary>Makes the specified effects selectable on the menu.</summary>
    /// <param name="codes">The effect IDs to make selectable.</param>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool EnableEffects(params string[] codes) => Send(new EffectUpdate(codes, EffectStatus.Selectable));

    /// <inheritdoc cref="EnableEffects(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public bool EnableEffects(IEnumerable<string> codes, string message = null) => Send(new EffectUpdate(codes, EffectStatus.Selectable, message));

    /// <inheritdoc cref="EnableEffects(string[])"/>
    /// <summary>Asynchronously makes the specified effects selectable on the menu.</summary>
    public Task<bool> EnableEffectsAsync(params string[] codes) => SendAsync(new EffectUpdate(codes, EffectStatus.Selectable));

    /// <inheritdoc cref="EnableEffectsAsync(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public Task<bool> EnableEffectsAsync(IEnumerable<string> codes, string message = null) => SendAsync(new EffectUpdate(codes, EffectStatus.Selectable, message));

    /// <summary>Makes all effects selectable on the menu.</summary>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool EnableAllEffects() => ShowEffects(m_mod.EffectLoader.EffectIDs);

    /// <inheritdoc cref="EnableAllEffects()"/>
    /// <summary>Asynchronously makes all effects selectable on the menu.</summary>
    public Task<bool> EnableAllEffectsAsync() => ShowEffectsAsync(m_mod.EffectLoader.EffectIDs);

    #endregion
    
    #region Disable Effects
    
    /// <summary>Makes the specified effects unselectable on the menu.</summary>
    /// <param name="codes">The effect IDs to make unselectable.</param>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool DisableEffects(params string[] codes) => Send(new EffectUpdate(codes, EffectStatus.NotSelectable));

    /// <inheritdoc cref="DisableEffects(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public bool DisableEffects(IEnumerable<string> codes, string message = null) => Send(new EffectUpdate(codes, EffectStatus.NotSelectable, message));

    /// <inheritdoc cref="DisableEffects(string[])"/>
    /// <summary>Asynchronously makes the specified effects unselectable on the menu.</summary>
    public Task<bool> DisableEffectsAsync(params string[] codes) => SendAsync(new EffectUpdate(codes, EffectStatus.NotSelectable));

    /// <inheritdoc cref="DisableEffectsAsync(string[])"/>
    /// <param name="message">An optional message to display when hiding the effects.</param>
    public Task<bool> DisableEffectsAsync(IEnumerable<string> codes, string message = null) => SendAsync(new EffectUpdate(codes, EffectStatus.NotSelectable, message));

    /// <summary>Makes all effects unselectable on the menu.</summary>
    /// <returns>True if the message was sent successfully, false otherwise.</returns>
    public bool DisableAllEffects() => ShowEffects(m_mod.EffectLoader.EffectIDs);

    /// <inheritdoc cref="DisableAllEffects()"/>
    /// <summary>Asynchronously makes all effects unselectable on the menu.</summary>
    public Task<bool> DisableAllEffectsAsync() => ShowEffectsAsync(m_mod.EffectLoader.EffectIDs);
    
    #endregion
}