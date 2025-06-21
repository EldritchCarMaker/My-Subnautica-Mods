using UnityEngine;
using UnityEngine.Bindings;

namespace Illuminautica.ColorOverrides;

public class ColorOverride
{
    public ColorOverride(Color color, int priority, float lerpDuration = 0)
    {
        _color = color;
        _lerp = lerpDuration;
    }
    private Color _color;
    private float _lerp;

    public int priority;
    public float LerpDuration => _lerp;
    public virtual bool IsValid => true;
    public virtual Color Color => _color;
}
