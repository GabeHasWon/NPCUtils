﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Terraria.GameContent.Bestiary;
using Terraria.Localization;
using Terraria.ModLoader;

namespace NPCUtils;

/// <summary>
/// Handles Bestiary automation and ease-of-use.
/// </summary>
public class BestiaryHelper
{
    /// <summary>
    /// The current instance of <see cref="BestiaryHelper"/>.
    /// </summary>
    public static BestiaryHelper Self { get; internal set; }

    private static Dictionary<string, IBestiaryInfoElement> _ConditionsByName = null;

    internal void Load()
    {
        // Set up dictionaries
        _ConditionsByName = [];

        // Load all conditions
        LoadNestedClassConditions(typeof(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes));
        LoadNestedClassConditions(typeof(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events));
        LoadNestedClassConditions(typeof(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions));
        LoadNestedClassConditions(typeof(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times));
        LoadNestedClassConditions(typeof(BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals));
    }

    /// <summary>
    /// Builds a BestiaryInfoElement for the given NPC with the given conditions.<br/>
    /// Also automatically generates a bestiary entry for the NPC, defaults to <see cref="string.Empty"/>.
    /// </summary>
    /// <param name="npc"></param>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public static IBestiaryInfoElement[] BuildEntry(ModNPC npc, string conditions)
    {
        string entryKey = $"Mods.{npc.Mod.Name}.NPCs.{npc.Name}.Entry"; // Make the key,
        Language.GetOrRegister(entryKey, () => ""); // Register it,

        var flavour = new FlavorTextBestiaryInfoElement(entryKey); // And use it automatically for the flavour text.

        if (conditions == string.Empty)
            return [flavour];

        string[] allConditions = conditions.Split(' ');
        var elements = new IBestiaryInfoElement[allConditions.Length + 1];

        elements[0] = flavour;

        for (int i = 1; i < elements.Length; ++i)
            elements[i] = _ConditionsByName[allConditions[i - 1]];

        return elements;
    }

    private static void LoadNestedClassConditions(Type containerType)
    {
        // Get all conditions in the given class 
        var allElements = containerType
            .GetFields(BindingFlags.Public | BindingFlags.Static)
            .Where(x => x.FieldType.IsAssignableTo(typeof(IBestiaryInfoElement)));

        foreach (var item in allElements)
        {
            var evnt = item.GetValue(null) as IBestiaryInfoElement;

            if (!_ConditionsByName.ContainsKey(item.Name))
                _ConditionsByName.Add(item.Name, evnt);
            else
                _ConditionsByName.Add(item.DeclaringType.Name + "." + item.Name, evnt);
        }
    }
}

/// <summary>
/// Used to simplify bestiary entry writing by extending a method onto <see cref="BestiaryEntry"/>.
/// </summary>
public static class BestiaryExtensions
{
    /// <summary>
    /// Automatically applies the localized entry and conditions to the npc.
    /// </summary>
    /// <param name="bestiaryEntry">The entry to modify.</param>
    /// <param name="npc">The NPC this entry is attatched to.</param>
    /// <param name="conditions">The conditions of the entry.</param>
    public static void AddInfo(this BestiaryEntry bestiaryEntry, ModNPC npc, string conditions) => bestiaryEntry.Info.AddRange(BestiaryHelper.BuildEntry(npc, conditions));
}