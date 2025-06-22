using UnityEngine;

namespace Illuminautica.ColorOverrides;

public class TemporaryColorOverride : ColorOverride
{
    public TemporaryColorOverride(float duration, Color color, int priority) : base(color, priority)
    {
        targetTime = Time.time + duration;
    }
    private float targetTime;

    public override bool IsValid => Time.time < targetTime;
}
