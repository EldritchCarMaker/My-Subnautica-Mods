using DroneBuddy.MonoBehaviours;
using DroneBuddy.MonoBehaviours.DroneBehaviours;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Assets.PrefabTemplates;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace DroneBuddy.Items;

internal static class DroneItem
{
    internal static PrefabInfo DroneInfo { get; } = PrefabInfo.WithTechType("DronePal", "Drone pal", "Drone buddy o pal o friend o", unlockAtStart: true);
    
    internal static void RegisterDrone()
    {
        var customPrefab = new CustomPrefab(DroneInfo);

        var clone = new CloneTemplate(DroneInfo, TechType.MapRoomCamera);
        clone.ModifyPrefabAsync += ModifyPrefab;

        customPrefab.SetGameObject(clone);
        customPrefab.SetEquipment(EquipmentType.Hand).WithQuickSlotType(QuickSlotType.Selectable);
        customPrefab.SetRecipe(new Nautilus.Crafting.RecipeData(new[] { new CraftData.Ingredient(TechType.MapRoomCamera, 1) }))
            .WithFabricatorType(CraftTree.Type.Fabricator)
            .WithStepsToFabricatorTab("Personal", "Tools");
        customPrefab.SetPdaGroupCategory(TechGroup.Personal, TechCategory.Tools);

        customPrefab.Register();
    }
    internal static IEnumerator ModifyPrefab(GameObject obj)
    {
        obj.EnsureComponent<Drone>();
        foreach(var type in Assembly.GetExecutingAssembly().GetTypes())
        {
            if (type.IsAbstract) continue;
            if (type.IsInterface) continue;
            if (!typeof(IDroneBehaviour).IsAssignableFrom(type)) continue;

            Plugin.Logger.LogMessage($"Adding component to drone {type}");
            obj.AddComponent(type);
        }
        obj.EnsureComponent<LargeWorldEntity>().cellLevel = LargeWorldEntity.CellLevel.Global;
        GameObject.Destroy(obj.GetComponent<DealDamageOnImpact>());

        yield return null;
    }
}
