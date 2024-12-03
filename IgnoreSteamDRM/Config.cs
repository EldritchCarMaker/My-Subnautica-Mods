using System;
using Nautilus.Json;
using Nautilus.Options.Attributes;

namespace IgnoreSteamDRM;

public class Config : ConfigFile
{
    public enum ServicesType
    {
        Default,
        ActLikeSteam,
        ActLikeStrippedSteam
    }
    [Choice(Label = "Platform Services Type", Tooltip = $"The type of platform services to use. Default should be the safest, ActLikeSteam should offer the most capability, ActLikeStrippedSteam is for fun/testing")]
    public ServicesType servicesType = ServicesType.Default;
}
