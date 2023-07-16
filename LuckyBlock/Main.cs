using QModManager.API.ModLoading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using LuckyBlock.BlockEvents;
using LuckyBlock.MonoBehaviours;
using Logger = QModManager.Utility.Logger;
using HarmonyLib;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using SMLHelper.V2.Handlers;
using LuckyBlock.Patches;

namespace LuckyBlock
{
    [QModCore]
    public static class Main
    {
        public static bool DebugLogs = false;//Not a config because I can just use runtime editor to set the field when necessary.
        public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static string Assets { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");
        public static AssetBundle Bundle { get; } = AssetBundle.LoadFromFile(Path.Combine(Assets, "luckyblockbundle"));

        public static Config Config { get; } = SMLHelper.V2.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();

        public static readonly bool rotaInstalled = QModManager.API.QModServices.Main.ModPresent("ProjectAncients");

        [QModPatch]
        public static void Patch()
        {
            string name = "EldritchCarMaker_LuckyBlocks";
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            RefreshLuckyBlockEvents();
            var harmony = Harmony.CreateAndPatchAll(Assembly, name);

            ConsoleCommandsHandler.Main.RegisterConsoleCommands(typeof(ConsoleCommands));

            if (rotaInstalled) RotAPatches.Patch(harmony);

            Logger.Log(Logger.Level.Info, $"Patched {name}");
        }

        public static void AddLuckyBlockEvent<T>() where T : LuckyBlockEvent
        {
            var @event = (LuckyBlockEvent)Activator.CreateInstance(typeof(T));
            AddLuckyBlockEvent(@event);
        }

        public static void AddLuckyBlockEvent(Type eventType)
        {
            if(eventType.IsAbstract || eventType.IsSubclassOf(typeof(LuckyBlockEvent)))
            {
                Logger.Log(Logger.Level.Error, "AddLuckyBlockEvent called with a type that is not a lucky block event. Don't do this");
                return;
            }

            var @event = (LuckyBlockEvent)Activator.CreateInstance(eventType);
            AddLuckyBlockEvent(@event);
        }

        public static void AddLuckyBlockEvent(this LuckyBlockEvent @event)
        {
            var list = LuckyBlockMono.luckyBlockEvents.ToList();
            list.Add(@event);
            LuckyBlockMono.luckyBlockEvents = list.ToArray();
        }

        public static void RefreshLuckyBlockEvents()
        {
            var events = new List<LuckyBlockEvent>();

            foreach (var type in Assembly.GetTypes())
            {
                if (type.IsAbstract || !type.IsSubclassOf(typeof(LuckyBlockEvent))) continue;

                //This isn't as good of a route as just using Activator, but I'm proud of it anyway cause this was clever. I refuse to delete it
                //var @event = (LuckyBlockEvent)(type.GetConstructor(Type.EmptyTypes).Invoke(null));//create new instance of the event
                var @event = (LuckyBlockEvent)Activator.CreateInstance(type);

                if (!CheckDependencies(@event))
                    continue;

                events.Add(@event);
            }
            LuckyBlockMono.luckyBlockEvents = events.ToArray();
        }

        public static bool CheckDependencies(LuckyBlockEvent @event)
        {
            if (@event is not IBlockEventDependency blockDependency)
                return true;

            foreach (var dependency in blockDependency.Dependencies)
                if (!QModManager.API.QModServices.Main.ModPresent(dependency)) return false;

            return true;
        }

        public static T WeightedRandom<T>(Func<T, int> getWeight, IEnumerable<T> list)
        {
            var chances = new Dictionary<Vector2, T>();
            var previousChance = new Vector2(0, 0);

            foreach (var entry in list)
            {
                var newChance = new Vector2(previousChance.y, previousChance.y + getWeight(entry));
                chances.Add(newChance, entry);
                previousChance = newChance;
            }

            var rand = UnityEngine.Random.Range(0, previousChance.y);
            return chances.FirstOrDefault(pair => pair.Key.x < rand && pair.Key.y > rand).Value;
        }
        public static T WeightedRandom<T>(Func<T, float> getWeight, IEnumerable<T> list)//overload for floats
        {
            var chances = new Dictionary<Vector2, T>();
            var previousChance = new Vector2(0, 0);

            foreach (var entry in list)
            {
                var newChance = new Vector2(previousChance.y, previousChance.y + getWeight(entry));
                chances.Add(newChance, entry);
                previousChance = newChance;
            }

            var rand = UnityEngine.Random.Range(0, previousChance.y);
            return chances.FirstOrDefault(pair => pair.Key.x < rand && pair.Key.y > rand).Value;
        }
    }

    [Menu("Lucky Blocks")]
    public class Config : ConfigFile
    {
        public UnityEngine.Events.UnityEvent<ConfigFileEventArgs> onConfigChange;
        [OnChange("OnConfigChange")]
        [Slider("Block Spawn Cooldown", Format = "{0:F1}", Min = 0.5f, Max = 60, DefaultValue = 10, Step = 0.5f, Tooltip = "The cooldown, in seconds, between block spawns")]
        public float blockSpawnCooldown = 10f;
        public void OnConfigChange(ConfigFileEventArgs args)
        {
            onConfigChange.Invoke(args);

            onConfigChange.AddListener((args) => OnConfigChange(args));
        }
        [Slider("Block Spawn Limit", Format = "{0:F1}", Min = -1, Max = 60, DefaultValue = 5, Step = 1, Tooltip = "The max number of blocks that can exist at a given time. Set to -1 to disable the limit")]
        public int maxBlocksCount = 5;
        [Toggle("Testing mode", Tooltip = "Enables testing mode. This will remove possible events once they have been triggered until all possible events have triggered. Meaning you have to go through every event before you can get a duplicate. Not recommended, as this largely bypasses random weighting")]
        public bool testVersion = false;
    }
}
