using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using ECCLibrary.Data;
using DroneBuddy.MonoBehaviours;
using ECCLibrary;
using Nautilus.Assets;
using System.Collections;
using UnityEngine;
using DroneBuddy.MonoBehaviours.Actions;

namespace DroneBuddy
{
    [BepInPlugin("EldritchCarMaker.DroneBuddy", "Drone Buddy", "0.0.1")]
    [BepInDependency("com.snmodding.nautilus")]
    [BepInDependency("com.lee23.ecclibrary")]
    public class Plugin : BaseUnityPlugin
    {
        public new static ManualLogSource Logger { get; private set; }

        private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        private void Awake()
        {
            Logger = base.Logger;

            DoEelThings();
            
            Harmony.CreateAndPatchAll(Assembly);
            Logger.LogInfo($"Drone buddy loaded! :)");
        }

        private void DoEelThings()
        {

            var cretin = new EEEEEEEEEEEEEEEL(PrefabInfo.WithTechType("DronePal", true));
            cretin.Register();
        }
        private class EEEEEEEEEEEEEEEL : CreatureAsset
        {
            public EEEEEEEEEEEEEEEL(PrefabInfo prefabInfo) : base(prefabInfo)
            {
            }

            protected override CreatureTemplate CreateTemplate()
            {
                var crtur = new CreatureTemplate(TechType.MapRoomCamera, 100);
                crtur.CellLevel = LargeWorldEntity.CellLevel.Global;
                crtur.BioReactorCharge = 0;
                crtur.CanBeInfected = false;
                crtur.ItemSoundsType = ItemSoundsType.Tank;
                crtur.LocomotionData = new()
                {
                    canMoveAboveWater = true,
                    driftFactor = 0.1f,
                    maxAcceleration = 12,
                };
                crtur.LiveMixinData = new()
                {
                    canResurrect = true,
                    destroyOnDeath = false,
                    invincibleInCreative = true,
                    maxHealth = 100,
                    weldable = true,
                };
                crtur.RespawnData = new(false);
                crtur.StayAtLeashData = new(0.7f, 4, 8);
                crtur.SwimBehaviourData = new(5);
                crtur.SwimRandomData.swimRadius = new(5, 5, 5);
                crtur.SwimRandomData.swimVelocity = 2;
                crtur.SwimRandomData.swimInterval = 10;
                crtur.SwimRandomData = null;

                crtur.SetCreatureComponentType<DroneCreature>();

                return crtur;
            }

            protected override IEnumerator ModifyPrefab(GameObject prefab, CreatureComponents components)
            {
                Destroy(prefab.GetComponent<MapRoomCamera>());
                Destroy(prefab.GetComponent<DealDamageOnImpact>());

                prefab.AddComponent<SwimAroundPoint>().minActionCheckInterval = 5;
                var col = prefab.AddComponent<Collect>();
                col.minActionCheckInterval = 3;
                col.evaluatePriority = 1;//This should be top priority when appplicable
                yield break;
            }
        }
    }
}