using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace NPCUtils;

/// <summary>
/// Extends <see cref="ILoot"/> and <see cref="LeadingConditionRule"/> to add ease-of-use methods, such as <see cref="AddCommon(ILoot, int, int, int, int)"/>.
/// </summary>
public static class LootExtensions
{
    // Non-generic extension methods
    // ILoot

    /// <summary>
    /// Adds the given list of drop rules to the <see cref="ILoot"/>.
    /// </summary>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
    /// <param name="rules">Array of rules to add.</param>
    public static void Add(this ILoot loot, params IItemDropRule[] rules)
	{
		foreach (var item in rules)
			loot.Add(item);
	}

    /// <summary>
    /// Adds an <see cref="ItemDropRule.Common(int, int, int, int)"/> to the given <see cref="ILoot"/>.
    /// </summary>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
    /// <param name="itemID">Item ID to drop.</param>
    /// <param name="chanceDenominator">1/x chance to drop.</param>
    /// <param name="minStack">Minimum stack.</param>
    /// <param name="maxStack">Maximum stack.</param>
    public static void AddCommon(this ILoot loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1)
	{
		if (maxStack < minStack)
			maxStack = minStack;
		loot.Add(ItemDropRule.Common(itemID, chanceDenominator, minStack, maxStack));
	}

    /// <summary>
    /// Almost exactly the same as <see cref="AddCommon(ILoot, int, int, int, int)"/>, but uses <see cref="ItemDropRule.Food(int, int, int, int)"/> instead.
    /// </summary>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
    /// <param name="itemID">Item ID to drop.</param>
    /// <param name="chanceDenominator">1/x chance to drop.</param>
    /// <param name="minStack">Minimum stack.</param>
    /// <param name="maxStack">Maximum stack.</param>
    public static void AddFood(this ILoot loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) => loot.Add(ItemDropRule.Food(itemID, chanceDenominator, minStack, maxStack));

    /// <summary>
    /// Adds an <see cref="ItemDropRule.OneFromOptions(int, int[])"/> to the given <see cref="ILoot"/>.
    /// </summary>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
    /// <param name="chanceDenominator">1/x chance to drop.</param>
    /// <param name="types">Array of item IDs to choose one from</param>
    public static void AddOneFromOptions(this ILoot loot, int chanceDenominator, params int[] types) => loot.Add(ItemDropRule.OneFromOptions(chanceDenominator, types));

    /// <summary>
    /// Adds a <see cref="ItemDropRule.BossBag(int)"/> to the given <see cref="ILoot"/>.
    /// </summary>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
    /// <param name="itemID">Item ID to drop.</param>
    public static void AddBossBag(this ILoot loot, int itemID) => loot.Add(ItemDropRule.BossBag(itemID));

	//LeadingConditionRule

	/// <inheritdoc cref="AddFood(ILoot, int, int, int, int)"/>
	public static void AddFood(this LeadingConditionRule loot, int itemID, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) => loot.OnSuccess(ItemDropRule.Food(itemID, chanceDenominator, minStack, maxStack));

	/// <inheritdoc cref="AddOneFromOptions(ILoot, int, int[])"/>
	public static void AddOneFromOptions(this LeadingConditionRule loot, int chanceDenominator, params int[] types) => loot.OnSuccess(ItemDropRule.OneFromOptions(chanceDenominator, types));

    /// <inheritdoc cref="AddBossBag(ILoot, int)"/>
    public static void AddBossBag(this LeadingConditionRule loot, int itemID) => loot.OnSuccess(ItemDropRule.BossBag(itemID));

    //Generic extension methods
    //NPCLoot

    /// <summary>
    /// <inheritdoc cref="AddCommon(ILoot, int, int, int, int)"/> <typeparamref name="TItem"/> parameter is the ModItem to use instead of typing the ID manually.
    /// </summary>
    /// <typeparam name="TItem">ModItem class to reference for the item ID.</typeparam>
    public static void AddCommon<TItem>(this ILoot loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where TItem : ModItem 
		=> loot.AddCommon(ModContent.ItemType<TItem>(), chanceDenominator, minStack, maxStack);

    /// <summary>
    /// <inheritdoc cref="AddFood(ILoot, int, int, int, int)"/> <typeparamref name="TItem"/> parameter is the ModItem to use instead of typing the ID manually.
    /// </summary>
    /// <typeparam name="TItem">ModItem class to reference for the item ID.</typeparam>
    public static void AddFood<TItem>(this ILoot loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where TItem : ModItem 
		=> loot.Add(ItemDropRule.Food(ModContent.ItemType<TItem>(), chanceDenominator, minStack, maxStack));

    /// <summary>
    /// <inheritdoc cref="AddBossBag(ILoot, int)"/> <typeparamref name="TItem"/> parameter is the ModItem to use instead of typing the ID manually.
    /// </summary>
    /// <typeparam name="TItem">ModItem class to reference for the item ID.</typeparam>
	public static void AddBossBag<TItem>(this ILoot loot) where TItem : ModItem => loot.Add(ItemDropRule.BossBag(ModContent.ItemType<TItem>()));

    /// <summary>
    /// Adds a <see cref="ItemDropRule.MasterModeCommonDrop(int)"/> to the given <see cref="ILoot"/>. <typeparamref name="TItem"/> parameter is the ModItem to use instead of typing the ID manually.
    /// </summary>
    /// <typeparam name="TItem">ModItem class to reference for the item ID.</typeparam>
    /// <param name="loot"><see cref="ILoot"/> object to add to.</param>
	public static void AddMasterModeCommonDrop<TItem>(this ILoot loot) where TItem : ModItem => loot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TItem>()));

    /// <summary>
    /// Adds a <see cref="ItemDropRule.MasterModeDropOnAllPlayers(int, int)"/> to the given <see cref="ILoot"/>. <typeparamref name="TItem"/> parameter is the ModItem to use instead of typing the ID manually.
    /// </summary>
    /// <typeparam name="TItem">ModItem class to reference for the item ID.</typeparam>
    public static void AddMasterModeDropOnAllPlayers<TItem>(this ILoot loot, int chanceDenominator = 1) where TItem : ModItem 
		=> loot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<TItem>(), chanceDenominator));

    /// <summary>
    /// Adds a <see cref="ItemDropRule.MasterModeCommonDrop(int)"/> to the given <see cref="ILoot"/>. The <typeparamref name="TRelic"/> and <typeparamref name="TPet"/> parameters are the ModItems to use instead of typing the IDs manually.
    /// </summary>
    /// <typeparam name="TRelic">ModItem class to reference for the relic's item ID.</typeparam>
    /// <typeparam name="TPet">ModItem class to reference for the pet's item ID.</typeparam>
    public static void AddMasterModeRelicAndPet<TRelic, TPet>(this ILoot loot) where TRelic : ModItem where TPet : ModItem
	{
		loot.AddMasterModeCommonDrop<TRelic>();
		loot.AddMasterModeDropOnAllPlayers<TPet>(4);
	}

	//LeadingConditionRule
    
    /// <inheritdoc cref="AddCommon{TItem}(ILoot, int, int, int)"/>
	public static void AddCommon<TItem>(this LeadingConditionRule loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where TItem : ModItem => loot.OnSuccess(ItemDropRule.Common(ModContent.ItemType<TItem>(), chanceDenominator, minStack, maxStack));

    /// <inheritdoc cref="AddFood{TItem}(ILoot, int, int, int)"/>
    public static void AddFood<TItem>(this LeadingConditionRule loot, int chanceDenominator = 1, int minStack = 1, int maxStack = 1) where TItem : ModItem => loot.OnSuccess(ItemDropRule.Food(ModContent.ItemType<TItem>(), chanceDenominator, minStack, maxStack));
	
    /// <inheritdoc cref="AddBossBag{TItem}(ILoot)"/>
    public static void AddBossBag<TItem>(this LeadingConditionRule loot) where TItem : ModItem => loot.OnSuccess(ItemDropRule.BossBag(ModContent.ItemType<TItem>()));
	
    /// <inheritdoc cref="AddMasterModeCommonDrop{TItem}(ILoot)"/>
    public static void AddMasterModeCommonDrop<TItem>(this LeadingConditionRule loot) where TItem : ModItem => loot.OnSuccess(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TItem>()));
	
    /// <inheritdoc cref="AddMasterModeCommonDrop{TItem}(ILoot)"/>
    public static void AddMasterModeDropOnAllPlayers<TItem>(this LeadingConditionRule loot, int chanceDenominator = 1) where TItem : ModItem => loot.OnSuccess(ItemDropRule.MasterModeDropOnAllPlayers(ModContent.ItemType<TItem>(), chanceDenominator));

	//NPCLoot

    /// <summary>
    /// Adds a <see cref="ItemDropRule.OneFromOptions(int, int[])"/> to the given <see cref="ILoot"/>
    /// </summary>
	public static void AddOneFromOptions<T1, T2>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
	public static void AddOneFromOptions<T1, T2, T3>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>());
    
    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6, T7>(this ILoot loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem where T7 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>(), ModContent.ItemType<T7>());

    //LeadingConditionRule

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem => loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>());

    /// <inheritdoc cref="AddOneFromOptions{T1, T2}(ILoot, int)"/>
    public static void AddOneFromOptions<T1, T2, T3, T4, T5, T6, T7>(this LeadingConditionRule loot, int chance = 1) where T1 : ModItem where T2 : ModItem where T3 : ModItem where T4 : ModItem where T5 : ModItem where T6 : ModItem where T7 : ModItem
		=> loot.AddOneFromOptions(chance, ModContent.ItemType<T1>(), ModContent.ItemType<T2>(), ModContent.ItemType<T3>(), ModContent.ItemType<T4>(), ModContent.ItemType<T5>(), ModContent.ItemType<T6>(), ModContent.ItemType<T7>());
}
