using MonoMod.RuntimeDetour;
using System;
using System.Linq;
using System.Reflection;
using Terraria.ModLoader.Core;
using Terraria.ModLoader;
using Terraria;

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

        foreach (var type in types)
        {
            if (Attribute.IsDefined(type, typeof(AutoloadBannerAttribute)))
            { 
                var banner = type.GetCustomAttribute(typeof(AutoloadBannerAttribute));
                var npc = mod.Find<ModNPC>(type.Name);

                mod.AddContent(new BaseBannerTile(npc.Type, npc.Name + "Banner"));
                mod.AddContent(new BaseBannerItem(npc.Name + "BannerItem", mod.Find<ModTile>(npc.Name + "Banner").Type, npc.Type));
            }

            if (Attribute.IsDefined(type, typeof(AutoloadCritterItemAttribute)))
            {
                var banner = type.GetCustomAttribute(typeof(AutoloadCritterItemAttribute));
                var npc = mod.Find<ModNPC>(type.Name);

                mod.AddContent(new CritterItem(npc.Name + "Item", npc.FullName, npc.Texture));
            }
        }

        setDefaultsDetour = new Hook(typeof(NPCLoader).GetMethod("SetDefaults", BindingFlags.Static | BindingFlags.NonPublic), SetAutoloadedValues);
        setDefaultsDetour.Apply();
    }

    internal static void Unload()
    {
        setDefaultsDetour.Undo();
        setDefaultsDetour.Dispose();
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

        if (Attribute.IsDefined(self.ModNPC.GetType(), typeof(AutoloadCritterItemAttribute)))
            self.catchItem = self.ModNPC.Mod.Find<ModItem>(self.ModNPC.Name + "Item").Type;
    }
}