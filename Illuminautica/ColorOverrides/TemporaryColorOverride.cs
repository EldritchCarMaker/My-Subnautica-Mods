using UnityEngine;

namespace Illuminautica.ColorOverrides;

public class TemporaryColorOverride : ColorOverride
{
    public TemporaryColorOverride(float duration, Color color, int priority, float lerpDuration = 0) : base(color, priority, lerpDuration)
    {
        targetTime = Time.time + duration;
    }
    private float targetTime;

    public override bool IsValid => Time.time < targetTime;
}
