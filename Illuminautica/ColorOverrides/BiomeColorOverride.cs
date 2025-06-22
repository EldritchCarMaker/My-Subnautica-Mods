using UnityEngine;

namespace Illuminautica.ColorOverrides;

internal class BiomeColorOverride : ColorOverride
{
    public BiomeColorOverride(int priority) : base(Color.gray, priority)
    {
    }
    public override Color Color
    {
        get
        {
            //Will have to see how this is
            WaterscapeVolume.Settings settings = null;
            WaterBiomeManager.main?.GetSettings(Player.main?.GetBiomeString(), out settings);
            if (settings != null)
            {
                return settings.emissive;
            }
            return Color.blue;
        }
    }
}
