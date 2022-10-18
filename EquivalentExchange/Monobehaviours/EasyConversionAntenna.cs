using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace EquivalentExchange.Monobehaviours
{
    internal class EasyConversionAntenna : MonoBehaviour
    {
        public const float MaxAntennaRange = 40;
        public const float PowerDrain = 0.5f;

        public static List<EasyConversionAntenna> Antennas = new List<EasyConversionAntenna>();

        public bool Active;

        private Text text;
        private PowerRelay powerRelay;

        public static bool AntennaInRange()
        {
            foreach (var antenna in EasyConversionAntenna.Antennas)
            {
                if (antenna == null || !antenna.isActiveAndEnabled || !antenna.gameObject.activeInHierarchy) continue;

                if (antenna.CountsAsActiveAntenna())
                    return true;
            }
            return false;
        }

        public void Start()
        {
            Antennas.Add(this);
            text = transform.Find("UI/Canvas/Text").GetComponent<Text>();

            powerRelay = GetComponentInParent<SubRoot>().powerRelay;
        }
        public void OnDestroy()
        {
            Antennas.Remove(this);
        }
        public void Update()
        {
            if (GameModeUtils.RequiresPower() && (!powerRelay || powerRelay.GetPower() <= PowerDrain * Time.deltaTime))
            {
                text.text = "No power";
                text.color = Color.red;
                Active = false;
                return;
            }
            text.text = $"{QMod.SaveData.ECMAvailable} ECM";
            text.color = Color.green;
            Active = true;

            if (GameModeUtils.RequiresPower())
                powerRelay.ConsumeEnergy(PowerDrain * Time.deltaTime, out _);
        }
        public bool CountsAsActiveAntenna()
        {
            return Active && (transform.position - Player.main.transform.position).magnitude < MaxAntennaRange;
        }
    }
}
