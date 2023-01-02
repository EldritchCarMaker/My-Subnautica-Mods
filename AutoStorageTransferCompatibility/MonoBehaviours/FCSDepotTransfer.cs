using AutoStorageTransfer.Monobehaviours;
#if SN1
using FCS_AlterraHub.Mods.AlterraHubDepot.Mono;
using FCS_ProductionSolutions.Mods.DeepDriller.HeavyDuty.Mono;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoStorageTransferCompatibility.MonoBehaviours
{
    internal class FCSDepotTransfer : StorageTransfer
    {
        public AlterraHubDepotController container;
        public override bool HasRoomFor(Pickupable item)
        {
            return container.CanBeStored(1, item.GetTechType());
        }
        public override bool AddItem(InventoryItem item)
        {
            return false;
        }
        public override bool IsTransferReady()
        {
            return container != null;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            InventoryItem chosenItem = null;
            int itemsChecked = 0;

            foreach (var pair in container._storage)//shouldn't be using foreach for this, but fuckit idc. Doesn't change anything at all anyway
            {
                var item = new InventoryItem(Instantiate(CraftData.GetPrefabForTechType(pair.Key).GetComponent<Pickupable>()));
                item.item.gameObject.SetActive(false);


                if (item == null) continue;

                if (itemsChecked >= AutoStorageTransfer.QMod.config.itemChecksBeforeBreak) break;

                itemsChecked++;

                if (SortAttemptsPerItem.TryGetValue(item, out var attempts) && attempts >= 5 && (Time.time < timeLastThoroughSort + AutoStorageTransfer.QMod.config.thoroughSortCooldown))
                    continue;

                var reciever = FindTransfer(item.item, StorageID);

                var usedRecievers = new List<StorageTransfer>();

                while (reciever != null)
                {
                    if (reciever.AddItem(item))
                    {
                        chosenItem = item;
                        break;
                    }
                    else
                    {
                        usedRecievers.Add(reciever);
                        reciever = FindTransfer(item.item, StorageID, usedRecievers);//if first reciever found can't take item, blacklist it and look again.
                    }
                }

                if (chosenItem != null) break;

                Destroy(item.item.gameObject);
            }

            if (chosenItem == null) return;

            if (SortAttemptsPerItem.ContainsKey(chosenItem))
                SortAttemptsPerItem.Remove(chosenItem);
            container.RemoveItemFromContainer(chosenItem.item.GetTechType());
        }
    }
}
#endif