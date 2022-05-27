using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CameraDroneUpgrades.MultiStorageClasses
{
    public class MultiStorageItemsContainer : ItemsContainer
    {
        public MultiStorageStorageContainer multiStorage;
        public MultiStorageItemsContainer(StorageContainer container) : base(container.width, container.height, container.transform, container.storageLabel, container.errorSound)
        {

        }
        public new int GetCount(TechType techType)
        {
            return multiStorage.GetCountInContainers(techType);
        }
    }
}
