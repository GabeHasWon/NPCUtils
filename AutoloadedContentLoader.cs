using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Core;
using Terraria.ModLoader;
using Terraria;
using System.Diagnostics;

namespace NPCUtils;

/// <summary>
/// Loads all banners, critter items, and detours <see cref="NPCLoader"/>'s SetDefaults to set <see cref="ModNPC.Banner"/> and <see cref="ModNPC.BannerItem"/>.
/// </summary>
public class AutoloadedContentLoader
{
    static Hook setDefaultsDetour;

    internal static void Load(Mod mod)
    {
        var types = AssemblyManager.GetLoadableTypes(mod.Code).Where(x => !x.IsAbstract && typeof(ModNPC).IsAssignableFrom(x));
        string banners = "";
        string critters  = "";

        foreach (var type in types)
        {
            if (Attribute.IsDefined(type, typeof(AutoloadBannerAttribute)))
            { 
                var banner = type.GetCustomAttribute(typeof(AutoloadBannerAttribute));
                var npc = mod.Find<ModNPC>(type.Name);

                mod.AddContent(new BaseBannerTile(npc.Type, npc.Name + "Banner", npc.Texture + "Banner"));
                mod.AddContent(new BaseBannerItem(npc.Name + "BannerItem", mod.Find<ModTile>(npc.Name + "Banner").Type, npc.Type, npc.Texture + "BannerItem"));
                banners += $"{mod.Name}.{npc.Name}Banner, ";
            }

            if (Attribute.IsDefined(type, typeof(AutoloadCritterAttribute)))
            {
                var banner = type.GetCustomAttribute(typeof(AutoloadCritterAttribute)) as AutoloadCritterAttribute;
                var npc = mod.Find<ModNPC>(type.Name);

                mod.AddContent(new CritterItem(npc.Name + "Item", npc.FullName, npc.Texture, banner.Value, banner.Rarity));
                critters += $"{mod.Name}.{npc.Name}Item, ";
            }
        }

        if (banners.Length > 2)
            mod.Logger.Debug($"[NPCUtils] AutoloadBanner: Autoloaded banners: {banners[..^2]}");

        if (critters.Length > 2)
            mod.Logger.Debug($"[NPCUtils] CritterItem: Autoloaded critters: {critters[..^2]}.");

        setDefaultsDetour = new Hook(typeof(NPCLoader).GetMethod("SetDefaults", BindingFlags.Static | BindingFlags.NonPublic), SetAutoloadedValues, true);
    }

    internal static void Unload()
    {
        setDefaultsDetour.Undo();
        setDefaultsDetour.Dispose();
        setDefaultsDetour = null;
    }

    private static void SetAutoloadedValues(Action<NPC, bool> orig, NPC self, bool createModNPC)
    {
        orig(self, createModNPC);

        if (self.ModNPC is null)
            return;

        if (Attribute.IsDefined(self.ModNPC.GetType(), typeof(AutoloadBannerAttribute)))
        {
            self.ModNPC.Banner = self.type;
            self.ModNPC.BannerItem = self.ModNPC.Mod.Find<ModItem>(self.ModNPC.Name + "BannerItem").Type;
        }

        if (Attribute.IsDefined(self.ModNPC.GetType(), typeof(AutoloadCritterAttribute)))
            self.catchItem = self.ModNPC.Mod.Find<ModItem>(self.ModNPC.Name + "Item").Type;
    }
}