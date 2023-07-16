using LuckyBlock.MonoBehaviours;
using System;
using System.Collections;
using UnityEngine;
using UWE;

namespace LuckyBlock.BlockEvents.SpawnEvents
{
    internal abstract class SpawnEventBase : LuckyBlockEvent
    {
        public abstract TechType[] TypesToSpawn { get; }
        public virtual string[] PathsToSpawn => null;
        public abstract Vector2int MinMaxToSpawn { get; }

        public sealed override void TriggerEffect(LuckyBlockMono luckyBlock)
        {
            var amount = UnityEngine.Random.Range(MinMaxToSpawn.x, MinMaxToSpawn.y);
            for (int i = 0; i < amount; i++)
            {
                var obj = GameObject.Instantiate(GetGameObject());
                obj.transform.position = GetSpawnPosition(luckyBlock);
                if (Main.DebugLogs) ErrorMessage.AddMessage($"Spawned {obj} from event {GetType().Name}, object was {(obj.activeSelf ? "active" : "inactive")}");
                obj.SetActive(true);
            }
        }

        public virtual Vector3 GetSpawnPosition(LuckyBlockMono luckyBlock)
        {
            return luckyBlock.transform.position + (UnityEngine.Random.insideUnitSphere * 2f);
        }

        public virtual GameObject GetGameObject()
        {
            if(!IsNullOrEmpty(TypesToSpawn))
            {
                var index = UnityEngine.Random.Range(0, TypesToSpawn.Length);
                return CraftData.GetPrefabForTechType(TypesToSpawn[index]);
            }

            if (!IsNullOrEmpty(PathsToSpawn))
            {
                var index = UnityEngine.Random.Range(0, PathsToSpawn.Length);
#pragma warning disable CS0618 // Type or member is obsolete
                return PrefabDatabase.GetPrefabForFilename(PathsToSpawn[index]);
#pragma warning restore CS0618 // Type or member is obsolete
            }
            return null;
        }

        public static bool IsNullOrEmpty(Array array)
        {
            return array == null || array.Length == 0;
        }
    }
}
