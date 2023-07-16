using Exploder.MeshCutter.Math;
using SMLHelper.V2.Commands;
using System.Collections.Generic;
using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class LuckyBlockSpawner : MonoBehaviour
    {
        internal static GameObject blockPrefab { get; private set; }
        private float BlockSpawnCooldown => Main.Config.blockSpawnCooldown;
        private int MaxBlocksCount => Main.Config.maxBlocksCount;

        private float _timeLastSpawn;
        internal static List<LuckyBlockMono> blocks = new List<LuckyBlockMono>();

        public void Update()
        {
            if(Time.time >= _timeLastSpawn + BlockSpawnCooldown && (blocks.Count < MaxBlocksCount || MaxBlocksCount == -1))
            {
                _timeLastSpawn = Time.time;
                Spawn();
            }
        }

        private void Spawn()
        {
            var position = Player.main.transform.position + (Random.onUnitSphere * Random.Range(50, 100));
            position.y = Mathf.Min(-5, position.y);//shouldn't spawn in air, and preferably spawns slightly below the water line
            if(!blockPrefab)
            {
                blockPrefab = Main.Bundle.LoadAsset<GameObject>("LuckyBlock");
                MaterialUtils.ApplySNShaders(blockPrefab);
            }
            var obj = Instantiate(blockPrefab);
            obj.transform.position = position;
        }
    }
}
