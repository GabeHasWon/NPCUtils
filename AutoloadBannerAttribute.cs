using System;

namespace Arbour.Content.Tiles.Banners;

/// <summary>
/// Apply this to a ModNPC's class to autoload their items as NPCNameBanner and NPCNameBannerItem.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
internal class AutoloadBannerAttribute : Attribute
{
}
