using UnityEngine;
using UnityEngine.Bindings;

namespace Illuminautica.ColorOverrides;

public class ColorOverride
{
    public ColorOverride(Color color, int priority)
    {
        _color = color;
        this.priority = priority;
    }
    private Color _color;

    public int priority;
    public virtual bool IsValid => true;
    public virtual Color Color => _color;
}
