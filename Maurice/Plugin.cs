using BepInEx;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maurice;

[BepInPlugin("EldritchCarMaker.Maurice", "Maurice", "0.0.3")]
public class Plugin : BaseUnityPlugin
{
    public void Awake()
    {
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), "EldritchCarMaker.Maurice");
    }
}
