using UnityEngine;

namespace LuckyBlock.MonoBehaviours
{
    internal class BoneSharkRainer : MonoBehaviour
    {
        public float duration = 60;

        private const float minSpawnCooldown = 1;
        private const float maxSpawnCooldown = 4;

        private float _timeNextSpawn = 0;
        private float _timeCreated = Time.time;
        private static GameObject _bonesharkPrefab;
        public static GameObject BonesharkPrefab 
        { 
            get
            {
                if (!_bonesharkPrefab) _bonesharkPrefab = CraftData.GetPrefabForTechType(TechType.BoneShark);
                return _bonesharkPrefab;
            }
        }

        public void Update()
        {
            if(Time.time >= _timeNextSpawn)
            {
                SpawnShark();
                _timeNextSpawn = Time.time + Random.Range(minSpawnCooldown, maxSpawnCooldown);
            }
            if (Time.time > _timeCreated + duration) Destroy(this);
        }

        public void SpawnShark()
        {
            var circs = Random.insideUnitCircle * 25;
            var position = Player.main.transform.position + new Vector3(circs.x, 40, circs.y);
            Instantiate(BonesharkPrefab, position, Random.rotation).GetComponent<Rigidbody>().AddForce(Vector3.up * -25, ForceMode.Acceleration);
        }
    }
}
