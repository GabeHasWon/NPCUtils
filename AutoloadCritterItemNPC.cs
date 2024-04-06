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
public class AutoloadCritterItemAttribute : Attribute
{
}

[Autoload(false)]
internal class CritterItem(string name, string npcKey, string texture) : ModItem
{
    public override string Name => _name;
    protected override bool CloneNewInstances => true;
    public override string Texture => _texturePath + "Item";

    private string _name = name;
    private string _texturePath = texture;
    private string _npcKey = npcKey;

    public override ModItem Clone(Item newEntity)
    {
        var entity = base.Clone(newEntity) as CritterItem;
        entity._name = _name;
        entity._npcKey = _npcKey;
        entity._texturePath = _texturePath;
        return entity;
    }

    public override void SetStaticDefaults()
    {
        Item.ResearchUnlockCount = 3;

        var modNPC = ModContent.Find<ModNPC>(_npcKey);
        Main.npcCatchable[modNPC.Type] = true;
        NPCID.Sets.CountsAsCritter[modNPC.Type] = true;
    }

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
    }

    public override bool CanUseItem(Player player) => !Collision.SolidCollision(Main.MouseWorld - new Vector2(10), 20, 20) &&
        player.IsInTileInteractionRange(Player.tileTargetX, Player.tileTargetY, TileReachCheckSettings.Simple);
}