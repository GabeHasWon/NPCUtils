using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace NPCUtils;

/// <summary>
/// Defines a banner tile to be autoloaded by <see cref="AutoloadBannerAttribute"/>.
/// </summary>
public class BaseBannerTile : ModTile
{
    /// <summary>
    /// The NPC this banner is associated with.
    /// </summary>
    public readonly int NPCType;

    /// <summary>
    /// The internal name of this banner, which is NPCNameBanner.
    /// </summary>
    public readonly string InternalName;

    /// <summary>
    /// Overrides the internal name to use the <see cref="InternalName"/>.
    /// </summary>
    public sealed override string Name => InternalName;

    /// <summary>
    /// Creates a banner tile with no name and no internal name.
    /// </summary>
    public BaseBannerTile()
    {
        NPCType = -1;
        InternalName = "";
    }

    /// <summary>
    /// Creates a banner tile with the given name and internal name.
    /// </summary>
    /// <param name="npcType">The associated NPC ID.</param>
    /// <param name="internalName">The internal name of the banner.</param>
    public BaseBannerTile(int npcType, string internalName)
    {
        NPCType = npcType;
        InternalName = internalName;
    }

    /// <summary>
    /// Disables loading if NPCType is invalid.
    /// </summary>
    /// <param name="mod"></param>
    /// <returns></returns>
    public override bool IsLoadingEnabled(Mod mod) => NPCType != -1;

    /// <summary>
    /// Sets defaults to a normal banner.
    /// </summary>
    public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2Top);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.CoordinateHeights = [16, 16, 16];
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide | AnchorType.SolidBottom, TileObjectData.newTile.Width, 0);
		TileObjectData.addTile(Type);

		TileID.Sets.DisableSmartCursor[Type] = true;

		AddMapEntry(new Color(13, 88, 130));
	
		DustType = -1;
    }

    /// <summary>
    /// Sets banner buff and hasBanner flags.
    /// </summary>
    /// <param name="i"><inheritdoc/></param>x
    /// <param name="j"><inheritdoc/></param>
    /// <param name="closer"><inheritdoc/></param>
    public override void NearbyEffects(int i, int j, bool closer)
	{
		Main.SceneMetrics.NPCBannerBuff[NPCType] = true;
		Main.SceneMetrics.hasBanner = true;
	}

    /// <inheritdoc/>
	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
	{
		if (i % 2 == 1)
			spriteEffects = SpriteEffects.FlipHorizontally;
	}
}

/// <summary>
/// Defines the base of a banner item.
/// </summary>
public class BaseBannerItem : ModItem
{
    readonly string InternalName;
    readonly int PlaceID;
    readonly int NPCType;

    /// <summary>
    /// New banners must be cloned.
    /// </summary>
    protected sealed override bool CloneNewInstances => true;

    /// <summary>
    /// New banners must use the internal name.
    /// </summary>
    public sealed override string Name => InternalName;

    /// <summary>
    /// New banners use only the BannerBonus tooltip line.
    /// </summary>
    public override LocalizedText Tooltip => Language.GetText("CommonItemTooltip.BannerBonus");

    /// <summary>
    /// Creates a banner item with default values.
    /// </summary>
    public BaseBannerItem()
    {
        InternalName = "";
        PlaceID = TileID.Dirt;
        NPCType = NPCID.None;
    }

    /// <summary>
    /// Creates a banner item with the given values.
    /// </summary>
    /// <param name="internalName">The given internal name.</param>
    /// <param name="placeID">The banner ID to place.</param>
    /// <param name="npcType">The NPC type associated with the banner. Used for the tooltip.</param>
    public BaseBannerItem(string internalName, int placeID, int npcType)
    {
        InternalName = internalName;
        PlaceID = placeID;
        NPCType = npcType;
    }

    /// <summary>
    /// Adds the name of the associated NPC to the BannerBuff tooltip.
    /// </summary>
    /// <param name="tooltips"></param>
    public override void ModifyTooltips(List<TooltipLine> tooltips)
    {
        var tooltip = tooltips.First(x => x.Name == "Tooltip0");
        tooltip.Text += Lang.GetNPCName(NPCType);
    }

    /// <summary>
    /// Disables loading if this item places only dirt.
    /// </summary>
    public override bool IsLoadingEnabled(Mod mod) => PlaceID != TileID.Dirt;

    /// <summary>
    /// Sets the defaults of the banner item to be a 12x30 tile placing item with a Blue rarity.
    /// </summary>
    public override void SetDefaults()
    {
        Item.DefaultToPlaceableTile(PlaceID);
        Item.Size = new Vector2(12, 30);
        Item.rare = ItemRarityID.Blue;
    }
}