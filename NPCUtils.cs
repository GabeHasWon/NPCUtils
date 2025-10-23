using System.Collections.Generic;
using Terraria.ModLoader;

namespace NPCUtils;

/// <summary>
/// Used for loading all utilities contained in the mod.
/// </summary>
public class NPCUtils : Mod
{
    private static readonly HashSet<string> AutoloadedContent = [];

    private static int BestiaryLoadedCount = 0;

    /// <summary>
    /// Loads <see cref="BestiaryHelper"/> if it's not yet loaded. This should be called in <see cref="Mod.Load"/>, and is unloaded automatically.
    /// </summary>
    public static void TryLoadBestiaryHelper(Mod mod)
    {
        if (BestiaryLoadedCount == 0)
        {
            BestiaryHelper.Self = new();
            BestiaryHelper.Self.Load();
        }

        BestiaryLoadedCount++;
        mod.AddContent(new ContentUnloader(false));
    }

    /// <summary>
    /// Unloads <see cref="BestiaryHelper"/> if no other mod is using it. 
    /// </summary>
    internal static void UnloadBestiaryHelper()
    {
        BestiaryLoadedCount--;

        if (BestiaryLoadedCount < 0)
            throw new System.Exception("BestiaryHelper unloaded too many times! How is this possible?");

        if (BestiaryLoadedCount == 0)
            BestiaryHelper.Self = null;
    }

    /// <summary>
    /// Autoloads all banners and critter items in the mod, given that the associated NPC uses <see cref="AutoloadBannerAttribute"/> or <see cref="AutoloadCritterAttribute"/>.<br/>
    /// This is unloaded automatically.
    /// </summary>
    /// <param name="mod">The mod to autoload banners from.</param>
    public static void AutoloadModBannersAndCritters(Mod mod)
    {
        if (!AutoloadedContent.Contains(mod.Name))
        {
            AutoloadedContentLoader.Load(mod);
            AutoloadedContent.Add(mod.Name);

            mod.AddContent(new ContentUnloader(true));
        }
    }

    /// <summary>
    /// Removes the flag for having autoloaded content for the given mod, allowing rebuilds to add content again properly.
    /// </summary>
    /// <param name="mod">The mod to remove the flag from.</param>
    internal static void UnloadMod(Mod mod)
    {
        AutoloadedContent.Remove(mod.Name);

        if (AutoloadedContent.Count == 0)
        {
            AutoloadedContentLoader.Unload();
        }
    }
}

internal class ContentUnloader(bool unloadMod) : ILoadable
{
    private Mod _mod = null;

    public void Load(Mod mod) => _mod = mod;

    public void Unload()
    {
        if (unloadMod)
            NPCUtils.UnloadMod(_mod);
        else
            NPCUtils.UnloadBestiaryHelper();

        _mod.Logger.Debug($"[NPCUtils] Successfully unloaded {(unloadMod ? "automatic content" : "bestiary helper reference")}");
    }
}