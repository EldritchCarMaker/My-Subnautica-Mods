using UnityEngine;

namespace Illuminautica.ColorOverrides;

internal class RandomColorOverride : ColorOverride
{
    public RandomColorOverride(int priority, float cooldown = 1) : base(default, priority)
    {
        this.cooldown = cooldown;
    }
    private Color color;
    private float timeLastRandom;
    private float cooldown;
    public override Color Color
    {
        get
        {
            if(timeLastRandom + cooldown < Time.time)
            {
                color = Random.ColorHSV();
                timeLastRandom = Time.time;
            }
            return color;
        }
    }
}
