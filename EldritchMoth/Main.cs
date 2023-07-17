using System.IO;
using UnityEngine;
using System.Reflection;
using HarmonyLib;
using EldritchMoth.Items;
using SMLHelper.V2.Json;
using SMLHelper.V2.Options.Attributes;
using System.Collections.Generic;
using SMLHelper.V2.Assets;
using SMLHelper.V2.Crafting;
using rail;
using UnityEngine.UIElements;
#if SN1
using Logger = QModManager.Utility.Logger;
using QModManager.API.ModLoading;
#else
using BepInEx;
using BepInEx.Logging;
#endif

namespace EldritchMoth
{
#if SN1
    [QModCore]
    public static class Main
    {
        public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static string Assets { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");

        public static Config Config { get; } = SMLHelper.V2.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();

        [QModPatch]
        public static void Patch()
        {
            string name = "EldritchCarMaker_LuckyBlocks";
            Logger.Log(Logger.Level.Info, $"Patching {name}");
            Harmony.CreateAndPatchAll(Assembly, name);
            new EldritchMothSpawnable().Patch();
            Logger.Log(Logger.Level.Info, $"Patched {name}");
        }
    }
#else
    [BepInPlugin("EldritchCarMaker.EldritchMoth", "Eldritch Moth", "1.0.0")]
    public class Main : BaseUnityPlugin
    {
        public static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        public static string Assets { get; } = Path.Combine(Path.GetDirectoryName(Assembly.Location), "Assets");

        public static Config Config { get; } = SMLHelper.V2.Handlers.OptionsPanelHandler.RegisterModOptions<Config>();

        public void Awake()
        {
            string name = "EldritchCarMaker_LuckyBlocks";
            Logger.Log(LogLevel.Info, $"Patching {name}");
            Harmony.CreateAndPatchAll(Assembly, name);
            new EldritchMothSpawnable().Patch();
            //Base.Piece.RoomWaterParkBottom
            //Base.Piece.RoomWaterParkCeilingGlassDome
            //Base.Piece.RoomWaterParkFloorBottom
            Logger.Log(LogLevel.Info, $"Patched {name}");
        }
    }
#endif

    public class Config : ConfigFile
    {
        public List<TechType> blueprintRequirements = new()
        {
            TechType.Seamoth,
            TechType.PrecursorIonPowerCell,
        };

        public Dictionary<TechType, int> recipe = new()
        {
            { TechType.PlasteelIngot, 4 },
            { TechType.EnameledGlass, 4 },
            { TechType.PrecursorIonPowerCell, 2 },
            { TechType.Lubricant, 2 },
            { TechType.AdvancedWiringKit, 2 },
        };

        public bool useElecEffect = true;
    }

    public class TestACU : Buildable
    {
        public TestACU() : base("TestACU", "TestACU", "TestACUe")
        {

        }

        protected override TechData GetBlueprintRecipe()
        {
            return new()
            {
                craftAmount = 1,
                Ingredients = new() { new(TechType.Titanium, 1) }
            };
        }
        
        public override bool UnlockedAtStart => true;

        protected override Atlas.Sprite GetItemSprite() => SpriteManager.Get(TechType.Titanium);

        public override TechCategory CategoryForPDA => TechCategory.BasePiece;

        public override TechGroup GroupForPDA => TechGroup.BasePieces;

        public override GameObject GetGameObject()
        {
            var main = new GameObject("TestACU");
            main.transform.rotation = Quaternion.identity;

            var scaler = new GameObject("TestACUasdasdasda");
            scaler.transform.parent = main.transform;

            var pieces = new[] { Base.Piece.RoomWaterParkBottom, Base.Piece.RoomWaterParkCeilingGlassDome, Base.Piece.RoomWaterParkFloorBottom, Base.Piece.RoomWaterParkHatch };

            foreach (var piece in pieces)
            {
                var prefab = Base.pieces[(int)piece].prefab;
                var copy = GameObject.Instantiate(prefab);
                copy.name = copy.name + "Test";
                copy.gameObject.SetActive(true);
                copy.parent = scaler.transform;
            }
            scaler.transform.localScale = Vector3.one * 0.5f;
            scaler.transform.rotation = Quaternion.identity;

            var con = main.EnsureComponent<Constructable>();
            con.alignWithSurface = true;
            con.allowedInBase = true;
            con.allowedInSub = true;
            con.allowedOnCeiling = true;
            con.allowedOnConstructables = true;
            con.allowedOnGround = true;
            con.allowedOnWall = true;
            con.allowedOutside = true;
            con.allowedUnderwater = true;
            con.model = scaler;

            return main;
        }
    }
}
