using Nautilus.Utility;
using System.Collections.Generic;
using UnityEngine;

namespace DroneBuddy.MonoBehaviours;

public class DroneContainer : MonoBehaviour
{
    public IItemsContainer Container { get; private set; }
    public TechType[] TechTypes => _techTypes.ToArray();
    private List<TechType> _techTypes = new();
    private StorageContainer _storageContainer;
    private void Awake()
    {
        gameObject.SetActive(false);//Inactive gameobject delays Awake method until object is active again. Needed to avoid null reference in StorageContainer.Awake
        _storageContainer = gameObject.EnsureComponent<StorageContainer>();

        var root = new GameObject("DroneStorageRoot");
        root.transform.parent = transform;

        _storageContainer.storageRoot = root.EnsureComponent<ChildObjectIdentifier>();
        _storageContainer.width = 1;
        _storageContainer.height = 1;
        _storageContainer.storageLabel = "Drone filter";

        gameObject.SetActive(true);
        _storageContainer.CreateContainer();
        Container = _storageContainer.container;

        Container.onAddItem += OnAdd;
        Container.onRemoveItem += OnRemove;
        _storageContainer.container.isAllowedToAdd += AllowedToAdd;
    }

    private bool AllowedToAdd(Pickupable pickupable, bool verbose)
    {
        if(!ResourceTrackerDatabase.resources.ContainsKey(pickupable.GetTechType()))
        {
            ErrorMessage.AddMessage("Resource isn't in resource tracker database. If this seems wrong ping me and let me know");
            return false;
        }
        if(_techTypes.Contains(pickupable.GetTechType()))
        {
            if (verbose)
                ErrorMessage.AddMessage("Item already exists in filter");
            return false;
        }
        return true;
    }

    private void OnRemove(InventoryItem item)
    {
        _techTypes.Remove(item.item.GetTechType());
    }

    private void OnAdd(InventoryItem item)
    {
        _techTypes.Add(item.item.GetTechType());
    }
    private void Update()
    {
        if (!_storageContainer.container.HasRoomFor(1, 1))
        {
            var width = _storageContainer.width;
            var height = _storageContainer.height;

            var newWidth = width == height ? width + 1 : width;
            var newHeight = height == width ? height : height + 1;

            ErrorMessage.AddMessage($"Resizing to {newWidth}, {newHeight}");

            _storageContainer.Resize(newWidth, newHeight);//Alternate between adding to height and width when full
        }
    }

    public void Open()
    {
        _storageContainer.Open();
    }
}
