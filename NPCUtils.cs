using System.Collections.Generic;
using Terraria.ModLoader;

namespace NPCUtils;

/// <summary>
/// The mod. Used for loading all utilities contained in the mod.
/// </summary>
public class NPCUtils : Mod
{
    private static readonly HashSet<string> AutoloadedContent = [];

    private static int BestiaryLoadedCount = 0;

    /// <summary>
    /// Loads <see cref="BestiaryHelper"/> if it's not yet loaded. This should be called in <see cref="Mod.Load"/>, alongside <see cref="UnloadBestiaryHelper"/> in <see cref="Mod.Unload"/>.
    /// </summary>
    public static void TryLoadBestiaryHelper()
    {
        if (BestiaryLoadedCount == 0)
        {
            BestiaryHelper.Self = new();
            BestiaryHelper.Self.Load();
        }

        BestiaryLoadedCount++;
    }

    /// <summary>
    /// Unloads <see cref="BestiaryHelper"/> if no other mod is using it. 
    /// </summary>
    public static void UnloadBestiaryHelper()
    {
        BestiaryLoadedCount--;

        if (BestiaryLoadedCount < 0)
            throw new System.Exception("BestiaryHelper unloaded too many times! Only call it once in Mod.Unload.");

        if (BestiaryLoadedCount == 0)
            BestiaryHelper.Self = null;
    }

    /// <summary>
    /// Autoloads all banners and critter items in the mod, given that the associated NPC uses <see cref="AutoloadBannerAttribute"/> or <see cref="AutoloadCritterAttribute"/>.<br/>
    /// Make sure to use <see cref="UnloadMod(Mod)"/> if you use this in order to properly reload content if needed.
    /// </summary>
    /// <param name="mod">The mod to autoload banners from.</param>
    public static void AutoloadModBannersAndCritters(Mod mod)
    {
        if (!AutoloadedContent.Contains(mod.Name))
        {
            AutoloadedContentLoader.Load(mod);
            AutoloadedContent.Add(mod.Name);
        }
    }

    /// <summary>
    /// Removes the flag for having autoloaded content for the given mod, allowing rebuilds to add content again properly.
    /// </summary>
    /// <param name="mod">The mod to remove the flag from.</param>
    public static void UnloadMod(Mod mod) => AutoloadedContent.Remove(mod.Name);
}
