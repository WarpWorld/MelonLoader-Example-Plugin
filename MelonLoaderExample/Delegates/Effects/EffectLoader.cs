using System.Collections.Concurrent;
using System.Reflection;
using System.Text.RegularExpressions;

namespace CrowdControl.Delegates.Effects;

/// <summary>An effect delegate container.</summary>
public class EffectLoader
{
    private const BindingFlags BINDING_FLAGS = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
    
    /// <summary>Provides a mapping of effect IDs to their respective delegates.</summary>
    /// <remarks>
    /// This should not need to be explicitly filled out, it is done automatically via reflection in the static constructor.
    /// Just make sure to add the [Effect] attribute to your methods.
    /// </remarks>
    private readonly ConcurrentDictionary<string, Effect> m_effects = new();
    
    /// <summary>Provides a mapping of effect ID regex patterns to their respective delegate keys.</summary>
    private readonly ConcurrentDictionary<string, Regex> m_regexes = new();
    
    public IEnumerable<string> EffectIDs => m_effects.Keys;

    /// <summary>Gets the mapping of effect IDs to their respective delegates.</summary>
    /// <param name="id">The effect ID to look up.</param>
    /// <param name="effect">The effect delegate, if found.</param>
    /// <returns>A dictionary mapping effect IDs to effect delegates.</returns>
    public bool TryGetEffect(string id, out Effect effect)
    {
        if (m_effects.TryGetValue(id, out effect))
            return true;

        foreach (KeyValuePair<string, Effect> kvp in m_effects)
        {
            if (!m_regexes.GetOrAdd(kvp.Key, key => new(key, RegexOptions.Compiled)).IsMatch(id)) continue;
            effect = kvp.Value;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Automatically loads all effect delegates from the assembly.
    /// </summary>
    public EffectLoader(CrowdControlMod mod, NetworkClient client)
    {
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes().Where(type => type.IsSubclassOf(typeof(Effect))))
        {
            try
            {
                foreach (EffectAttribute attribute in type.GetCustomAttributes<EffectAttribute>())
                {
                    foreach (string id in attribute.IDs)
                    {
                        try { m_effects[id] = (Effect)Activator.CreateInstance(type, mod, client); }
                        catch (Exception e) { CrowdControlMod.Instance.Logger.Error(e); }
                    }
                }
            }
            catch (Exception e) { CrowdControlMod.Instance.Logger.Error(e); }
        }
    }
}