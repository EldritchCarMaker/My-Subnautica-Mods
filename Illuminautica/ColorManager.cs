using System.Collections.Generic;
using Illuminautica.ColorOverrides;
using Illuminautica.Interop;
using UnityEngine;

namespace Illuminautica;

public class ColorManager : MonoBehaviour
{
    public static ColorManager instance { get; private set; }

    private List<ColorOverride> colorOverrides = new();
    private ColorOverride currentOverride;
    private Color currentColor;

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
        var overridesToRemove = new List<ColorOverride>();
        foreach(var colorOverride in colorOverrides)
        {
            if (colorOverride.IsValid)
                continue;
            //Filter out invalid overrides


            //I feel like an event could go here, but it just doesn't seem necessary
            //What use could that event have?
            overridesToRemove.Add(colorOverride);

            listDirty = true;
        }
        foreach(var colorOverride in overridesToRemove) 
            colorOverrides.Remove(colorOverride);

        if (listDirty)
        {
            RecheckCurrentPriority();
        }


        if (currentOverride == null)
            return;

        //Even if the list isn't dirty, check current color
        //The same override could change colors
        if(currentOverride.Color != currentColor)
        {
            currentColor = currentOverride.Color;
            InteropManager.SetCurrentColor(currentColor);
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

        currentOverride = highestPriority;
    }

    public void AddNewColorOverride(ColorOverride colorOverride)
    {
        colorOverrides.Add(colorOverride);//Duplicate protection just ain't necessary here, it don't matter
        RecheckCurrentPriority();
    }
}
