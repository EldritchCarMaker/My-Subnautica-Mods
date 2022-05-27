using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CameraDroneUpgrades.MultiStorageClasses
{
    public class MultiStorageStorageContainer : StorageContainer
    {
        public List<ItemsContainer> containersList = new List<ItemsContainer>();

        new MultiStorageItemsContainer container;

        public MultiStorageStorageContainer(StorageContainer container)
        {
            //CopyAll<StorageContainer>(container, this);
            //was giving errors immediately, no clue what they meant though. Some null reference in CopyAll but no clue where

            this.container = new MultiStorageItemsContainer(container);
            this.container.multiStorage = this;

            //CopyAll<ItemsContainer>(container.container, this.container);
        }

        public void AddContainer(ItemsContainer con)
        {
            if(!containersList.Contains(con))
                containersList.Add(con);
        }
        public int GetCountInContainers(TechType techType)
        {
            int count = 0;
            foreach(ItemsContainer con in containersList)
            {
                count += con.GetCount(techType);
            }
            return count;
        }
        public static void CopyAll<T>(T source, T target)
        {
            var type = typeof(T);
            foreach (var sourceProperty in type.GetProperties())
            {
                var targetProperty = type.GetProperty(sourceProperty.Name);
                targetProperty.SetValue(target, sourceProperty.GetValue(source, null), null);
            }
            foreach (var sourceField in type.GetFields())
            {
                var targetField = type.GetField(sourceField.Name);
                targetField.SetValue(target, sourceField.GetValue(source));
            }
        }
    }
}
