#if !SN2
using QModManager.Utility;
using Logger = QModManager.Utility.Logger;
#else
using AutoStorageTransfer.Json;
using BepInEx.Logging;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AutoStorageTransfer.Monobehaviours
{
    public class StorageTransfer : MonoBehaviour
    {
        private static List<StorageTransfer> storageTransfers = new List<StorageTransfer>();
        
        private StorageContainer _storageContainer;
        public StorageContainer StorageContainer 
        { 
            get 
            { 
                if( _storageContainer == null )
                    _storageContainer = GetComponent<StorageContainer>();
                return _storageContainer; 
            }
        }

        protected IItemsContainer _itemsContainer;
        public IItemsContainer Container 
        { 
            get 
            { 
                if (_itemsContainer == null) 
                    _itemsContainer = StorageContainer?.container; 
                return _itemsContainer; 
            } 
        }

        private UniqueIdentifier _uniqueIdentifier;
        public UniqueIdentifier UniqueIdentifier => GetUniqueIdentifier();

        protected string StorageID;
        public bool IsReciever { get; private set; } = true;


        protected Dictionary<InventoryItem, int> SortAttemptsPerItem = new Dictionary<InventoryItem, int>();
        //so that we don't end up with 30 items all doing a bunch of looking around and string comparing constantly, just because they can't find a spot.

        protected float timeLastThoroughSort = 0;
        public UniqueIdentifier GetUniqueIdentifier()
        {
            if (StorageContainer != null)
                _uniqueIdentifier = StorageContainer.storageRoot;

            if (_uniqueIdentifier == null)
                _uniqueIdentifier = UWE.Utils.GetComponentInHierarchy<UniqueIdentifier>(gameObject);

            return _uniqueIdentifier;
        }
        public string GetStorageID()
        {
            return StorageID;
        }

        public void Start()
        {
            if(!gameObject.activeInHierarchy)
            {
                Destroy(this);
                return;
            }
#if SN
            if (TryGetComponent(out SpawnEscapePodSupplies asd))
            {
                Destroy(this);//fuck this. This single container is causing problems all on its own. Fuck it.
                return;
            }
#endif

            if (!storageTransfers.Contains(this)) 
                storageTransfers.Add(this);

            if (UniqueIdentifier == null) return;//there's not much I can do regarding containers that can't find a prefab identifier. Sucks to suck lul

            QMod.SaveData.OnStartedSaving += OnBeforeSave;
            if(QMod.SaveData.SavedStorages.TryGetValue(UniqueIdentifier.Id, out SaveInfo saveInfo))
            {
                StorageID = saveInfo.StorageID;
                IsReciever = saveInfo.IsReciever;
            }
        }
        public void OnBeforeSave(object sender, EventArgs e)
        {
            try//fuck this shit. I'm not dealing with it
            {
                if (QMod.SaveData.SavedStorages.TryGetValue(UniqueIdentifier.Id, out var saveInfo))
                {
                    saveInfo.IsReciever = IsReciever;
                    saveInfo.StorageID = StorageID;
                }
                else
                {
                    var newSaveInfo = new SaveInfo()
                    {
                        StorageID = StorageID,
                        IsReciever = IsReciever
                    };
                    QMod.SaveData.SavedStorages.Add(UniqueIdentifier.id, newSaveInfo);
                }
            }
            catch
            {
#if !SN2
                Logger.Log(Logger.Level.Error, $"Error caught when saving storage transfer! Saving will continue for everything except this container's transfer settings.");
                try
                {
                    Logger.Log(Logger.Level.Error, $"unique identifier of previous failed save: {_uniqueIdentifier}");
                }
                catch(Exception)
                {
                    Logger.Log(Logger.Level.Error, "Couldn't even get prefab identifier without error. fuck this", null, true);
                }
#else
                var logger = QMod.logger;
                logger.LogError($"Error caught when saving storage transfer! Saving will continue for everything except this container's transfer settings.");
                try
                {
                    logger.LogError($"unique identifier of previous failed save: {_uniqueIdentifier}");
                }
                catch (Exception)
                {
                    logger.LogError("Couldn't even get prefab identifier without error. fuck this");
                }
#endif
            }
        }


        public void OnDisable()
        {
            storageTransfers.Remove(this);
        }
        public void OnDestroy()
        {
            storageTransfers.Remove(this);
        }
        public virtual bool IsTransferReady()
        {
            return StorageContainer || (Container != null);
        }
        public virtual void FixedUpdate()
        {
            if(!IsTransferReady())
            {
                Destroy(this);
                return;
            }

            if (!storageTransfers.Contains(this))
            {
                storageTransfers.Add(this);
            }

            if (IsReciever || string.IsNullOrEmpty(StorageID)) return;

            if (Container == null) return;

            InventoryItem chosenItem = null;
            int itemsChecked = 0;

            bool isThoroughSort = (Time.time > timeLastThoroughSort + QMod.config.thoroughSortCooldown);
            if (isThoroughSort) timeLastThoroughSort = Time.time;

            foreach (var item in Container)//shouldn't be using foreach for this, but fuckit idc. Doesn't change anything at all anyway
            {
                if (item == null) continue;

                if (itemsChecked >= QMod.config.itemChecksBeforeBreak) break;

                itemsChecked++;

                if (!isThoroughSort && SortAttemptsPerItem.TryGetValue(item, out var attempts) && attempts >= 5)
                    continue;

                var reciever = FindTransfer(item.item, StorageID);

                var usedRecievers = new List<StorageTransfer>();

                while (reciever != null)
                {
                    if(reciever.AddItem(item))
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

                if(SortAttemptsPerItem.TryGetValue(item, out var value))
                    SortAttemptsPerItem[item] = value + 1;
                else
                    SortAttemptsPerItem.Add(item, 1);
            }

            if (chosenItem == null) return;

            if (SortAttemptsPerItem.ContainsKey(chosenItem))
                SortAttemptsPerItem.Remove(chosenItem);
            Container.RemoveItem(chosenItem, true, false);
        }
        public static StorageTransfer FindTransfer(Pickupable item, string storageID, List<StorageTransfer> ignoreTransfers = null)
        {
            List<StorageTransfer> transfersToRemove = new List<StorageTransfer>();

            if (string.IsNullOrEmpty(storageID)) return null;

            StorageTransfer storageTransfer = null;

            foreach (StorageTransfer reciever in storageTransfers)
            {
                if (reciever == null || !reciever.gameObject.activeInHierarchy)
                {
                    transfersToRemove.Add(reciever);
                    continue;
                }

                if (ignoreTransfers != null && ignoreTransfers.Contains(reciever)) continue;

                if (!reciever.IsReciever) continue;
                if (storageID != reciever.StorageID) continue;

                try
                {
                    if (reciever.HasRoomFor(item))
                    {
                        storageTransfer = reciever;
                        break;
                    }
                }
                catch
                {
                    ErrorMessage.AddError($"Error caught! Failed with {reciever.name}. Handled safely");
                    if(storageTransfer == null)
                    {
                        storageTransfer = reciever;
                        break;
                    }
                }
            }
            foreach (var transfer in transfersToRemove)
            {
                storageTransfers.Remove(transfer);
            }
            return storageTransfer;
        }
        public void ToggleRecieverStatus()
        {
            IsReciever = !IsReciever;
        }
        public void SetIDString(string ID)
        {
            StorageID = ID;
        }
        public void SetContainer(IItemsContainer container)
        {
            if (container == null) return;

            if (Container != null) return;//should be no reason to replace existing container

            _itemsContainer = container;
        }
        public virtual bool HasRoomFor(Pickupable item)
        {
            return Container.HasRoomFor(item, null);
        }
        public virtual bool AddItem(InventoryItem item)
        {
            return Container.AddItem(item);
        }
    }
}
