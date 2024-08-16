using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace NPCUtils;

/// <summary>
/// Autoloads the given critter item if placed on an NPC.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AutoloadCritterAttribute(int value = 0, byte rarity = ItemRarityID.White) : Attribute
{
    /// <summary>
    /// Value, in copper coins, of the critter.
    /// </summary>
    public int Value = value;

    /// <summary>
    /// Rarity of the critter.
    /// </summary>
    public byte Rarity = rarity;
}

/// <summary>
/// Defines a critter item that spawns a critter.
/// </summary>
/// <param name="name">Internal name of the item.</param>
/// <param name="npcKey">NPC key (full name, such as Egg/EggMonster) to reference.</param>
/// <param name="texture">Texture of the item.</param>
/// <param name="rarity">Rarity of the item. Defaults to ItemRarityID.White.</param>
/// <param name="value">Value of the item, in copper coins. Defaults to 0.</param>
[Autoload(false)]
public sealed class CritterItem(string name, string npcKey, string texture, int value, byte rarity) : ModItem
{
    /// <summary>
    /// Overrides name with the internal name.
    /// </summary>
    public sealed override string Name => _name;

    /// <summary>
    /// New instances must be cloned.
    /// </summary>
    protected override bool CloneNewInstances => true;

    /// <summary>
    /// Overrides texture to be the original texture path with "Item" appended.
    /// </summary>
    public override string Texture => _texturePath + "Item";

    private string _name = name;
    private string _texturePath = texture;
    private string _npcKey = npcKey;
    private int _value = value;
    private byte _rarity = rarity;

    /// <summary>
    /// Clones the item.
    /// </summary>
    public override ModItem Clone(Item newEntity)
    {
        var entity = base.Clone(newEntity) as CritterItem;
        entity._name = _name;
        entity._npcKey = _npcKey;
        entity._texturePath = _texturePath;
        entity._value = _value;
        entity._rarity = _rarity;
        return entity;
    }

    /// <summary>
    /// Sets <see cref="Item.ResearchUnlockCount"/> to 3, and sets the associated NPC's type's <see cref="Main.npcCatchable"/> and <see cref="NPCID.Sets.CountsAsCritter"/> to true.
    /// </summary>
    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 3;

        var modNPC = ModContent.Find<ModNPC>(_npcKey);
        Main.npcCatchable[modNPC.Type] = true;
        NPCID.Sets.CountsAsCritter[modNPC.Type] = true;
    }

    /// <summary>
    /// Sets the item to the following:
    /// <code>        
    /// var modNPC = ModContent.Find{ModNPC}(_npcKey);
    /// Item.Size = modNPC.NPC.Size;
    /// Item.useAnimation = 16;
    /// Item.useTime = 16;
    /// Item.damage = 0;
    /// Item.rare = ItemRarityID.White;
    /// Item.maxStack = Item.CommonMaxStack;
    /// Item.noUseGraphic = true;
    /// Item.noMelee = false;
    /// Item.useStyle = ItemUseStyleID.Swing;
    /// Item.makeNPC = (short) modNPC.Type;
    /// Item.autoReuse = true;
    /// Item.consumable = true;
    /// </code>
    /// </summary>
    public override void SetDefaults()
    {
        var modNPC = ModContent.Find<ModNPC>(_npcKey);

        Item.Size = modNPC.NPC.Size;
        Item.useAnimation = 16;
        Item.useTime = 16;
        Item.damage = 0;
        Item.rare = ItemRarityID.White;
        Item.maxStack = Item.CommonMaxStack;
        Item.noUseGraphic = true;
        Item.noMelee = false;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.makeNPC = (short)modNPC.Type;
        Item.autoReuse = true;
        Item.consumable = true;
        Item.rare = _rarity;
        Item.value = _value;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="player"></param>
    /// <returns></returns>
    public override bool CanUseItem(Player player)
    {
        var modNPC = ModContent.Find<ModNPC>(_npcKey);

        return !Collision.SolidCollision(Main.MouseWorld - new Vector2(10), modNPC.NPC.width, modNPC.NPC.height) &&
            player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple);
    }
}