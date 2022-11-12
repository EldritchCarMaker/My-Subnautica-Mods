using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace QuantumBase.Mono
{
    internal class EnergyGenerator : HandTarget, IHandTarget
    {
        internal static Dictionary<TechType, float> substanceChargeValues = new Dictionary<TechType, float>();

        public PowerSource powerSource;

        public StorageContainer container { get; private set; }

        private float extraEnergy = 0;
        private const float percentExtraEnergyPerSecond = 5;
        private const float MaxExtraEnergy = 400;
        public override void Awake()
        {
            Destroy(GetComponentInParent<BaseBioReactorGeometry>());
            Destroy(GetComponent<GenericHandTarget>());//for some reason the destroy doesn't always work, so I do it here too

            base.Awake();

            container = GetComponentInChildren<StorageContainer>();

            if (container)
                return;

            var storageRoot = new GameObject("StorageRoot");
            storageRoot.SetActive(false);
            storageRoot.transform.parent = transform;

            container = storageRoot.AddComponent<StorageContainer>();

            container.storageRoot = storageRoot.EnsureComponent<ChildObjectIdentifier>();
            container.storageLabel = "Generator Storage";
            container.height = 4;
            container.width = 4;
            container.onUse = new StorageContainer.UseEvent();

            container.CreateContainer();

            container.container.isAllowedToAdd += AllowedToAdd;

            storageRoot.SetActive(true);
        }
        public void Update()
        {
            UpdateContainer();

            var energy = (extraEnergy * (percentExtraEnergyPerSecond / 100)) * Time.deltaTime;

            if (extraEnergy < 10) energy = extraEnergy;

            powerSource.AddEnergy(energy, out _);
            extraEnergy -= energy;

            QMod.SaveData.Energy = powerSource.GetPower();
        }
        public void UpdateContainer()
        {
            if (container.open || container.IsEmpty()) return;

            if (extraEnergy >= MaxExtraEnergy) return;

            var item = container.container.FirstOrDefault();

            if (item == null) return;

            var techType = item.item.GetTechType();

            if(container.container.DestroyItem(techType))
            {
                extraEnergy += substanceChargeValues[techType];
            }
        }
        public bool AllowedToAdd(Pickupable pickupable, bool verbose)
        {
            return substanceChargeValues.ContainsKey(pickupable.GetTechType());
        }
        public void OpenContainer()
        {
            if(!KnownTech.Contains(QMod.substanceUnlockType))
            {
                KnownTech.Add(QMod.substanceUnlockType);
                ErrorMessage.AddMessage("Recieved blueprints for mysterious energetic substances");
            }

            Inventory.main.SetUsedStorage(container.container);
            if(Player.main.GetPDA().Open(PDATab.Inventory, transform, container.OnClosePDA, container.modelSizeRadius + 2))
            {
                container.open = true;
            }
        }
        public void OnHandClick(GUIHand hand)
        {
            OpenContainer();
        }
        public void OnHandHover(GUIHand hand)
        {
            HandReticle.main.SetInteractText("Open Generator Storage", $"{(int)extraEnergy} energy stored");
            HandReticle.main.SetIcon(HandReticle.IconType.Hand);
        }
    }
}
