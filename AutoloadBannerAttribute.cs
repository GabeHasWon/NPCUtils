using System;

namespace NPCUtils;

/// <summary>
/// Apply this to a ModNPC's class to autoload their items as NPCNameBanner and NPCNameBannerItem.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public class AutoloadBannerAttribute : Attribute
{
}
