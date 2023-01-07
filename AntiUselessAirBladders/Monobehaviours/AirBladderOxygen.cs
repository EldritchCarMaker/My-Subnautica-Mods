using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AntiUselessAirBladders.Monobehaviours
{
    public class AirBladderOxygen : MonoBehaviour
    {
        private const float oxygenRechargePerSecond = 2.5f;
        private const float maxOxygen = 15;

        public float oxygen
        {
            get { return _oxygen; }
            set { _oxygen = Mathf.Clamp(value, 0, maxOxygen); }
        }

        private float _oxygen;
        private AirBladder oxygenBladder;
        public void Awake()
        {
            oxygenBladder = GetComponent<AirBladder>();
        }
        public void Update()
        {
            if(Player.main.CanBreathe())
            {
                oxygen = Mathf.Min(maxOxygen, oxygen + (oxygenRechargePerSecond * Time.deltaTime));
            }
            if (oxygenBladder.usingPlayer)
            {
                HandReticle.main.useText2 = $"oxygen: {oxygen}/{maxOxygen}";
                HandReticle.main.useText1 = $"{Language.main.GetFormat<string, string>("HandReticleAddButtonFormat", "Consume:", uGUI.FormatButton(GameInput.Button.AltTool))}";
            }
        }
        public static float ConsumeOxygen(AirBladder airBladder, float amountToConsume)
        {
            if (airBladder.TryGetComponent<AirBladderOxygen>(out var oxygenHolder))
            {
                float num = Mathf.Min(amountToConsume, oxygenHolder.oxygen);
                oxygenHolder.oxygen = oxygenHolder.oxygen - num;
                return num;
            }
            ErrorMessage.AddMessage("Could not find AirBladderOxygen on air bladder!");
            return 0;
        }
    }
}
