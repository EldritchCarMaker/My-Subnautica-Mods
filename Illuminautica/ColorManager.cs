using System.Collections.Generic;
using Illuminautica.ColorOverrides;
using Illuminautica.Interop;
using UnityEngine;

namespace Illuminautica;

public class ColorManager : MonoBehaviour
{
    private static ColorManager instance;

    private List<ColorOverride> colorOverrides = new();
    private void Awake()
    {
        if(instance)
        {
            Plugin.logger.LogError("Multiple color manager instances! This is bad! Do not do this! Destroying new one.");
            Destroy(this);
            return;
        }
        instance = this;
    }
    private void Update()
    {
        var listDirty = false;
        foreach(var colorOverride in colorOverrides)
        {
            if (colorOverride.IsValid)
                continue;
            //Filter out invalid overrides


            //I feel like an event could go here, but it just doesn't seem necessary
            //What use could that event have?
            colorOverrides.Remove(colorOverride);

            listDirty = true;
        }

        if(listDirty)
        {
            RecheckCurrentPriority();
        }
    }

    private void RecheckCurrentPriority()
    {
        ColorOverride highestPriority = null;

        foreach (var colorOverride in colorOverrides)
        {
            if(highestPriority == null || colorOverride.priority > highestPriority.priority)
                highestPriority = colorOverride;
        }

        if(highestPriority != null)
        {
            //Should be safe to call with duplicate colors, I think... Only one way to find out!
            InteropManager.SetCurrentColor(highestPriority.Color, highestPriority.LerpDuration);
        }
    }

    public void AddNewColorOverride(ColorOverride colorOverride)
    {
        colorOverrides.Add(colorOverride);//Duplicate protection just ain't necessary here, it don't matter
        RecheckCurrentPriority();
    }
}
