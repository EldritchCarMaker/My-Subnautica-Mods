using System.Collections.Generic;
using HarmonyLib;
using Illuminautica.ColorOverrides;
using UnityEngine;

namespace Illuminautica.BuiltInColorEvents;

[HarmonyPatch(typeof(Player), nameof(Player.OnTakeDamage))]
internal class PlayerDamageTaken
{
    public static Dictionary<DamageType, Color> damageColors = new()
    {
        { DamageType.Radiation, Color.yellow },
        { DamageType.Electrical, Color.blue },
        { DamageType.Poison, Color.green },
        { DamageType.Fire, Color.magenta },
        { DamageType.Heat, Color.magenta },
    };
    [HarmonyPostfix]
    public static void Postfix(DamageInfo damageInfo)
    {
        if(damageInfo == null) return;

        Color color = Color.red;
        if(damageColors.TryGetValue(damageInfo.type, out var damColor))
            color = damColor;

        ColorManager.instance.AddNewColorOverride(new TemporaryColorOverride(5, color, 10));
    }
}
