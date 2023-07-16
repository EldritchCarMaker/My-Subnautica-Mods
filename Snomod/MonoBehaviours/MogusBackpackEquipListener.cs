using Snomod.Prefabs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Snomod.MonoBehaviours
{
    internal class MogusBackpackEquipListener : MonoBehaviour, IEquippable
    {
        [SerializeField]//means its set in thunderkit/unity rather than through code
        private StorageContainer container;

        [SerializeField]
        private MogusColorChanger colorChanger;

        private static MogusColorChanger backpackModel;
        public static MogusBackpackEquipListener instance { get; private set; }

        public void Open()
        {
            container.Open();
        }

        public void OnEquip(GameObject sender, string slot)
        {
            // The item is disabled when in storage, but OnEquip is called when save is loaded if the item is equipped. So need to do a makeshift awake using it
            if (container.container == null) container.Awake();
            if (colorChanger.Color == MogusColorChanger.ColorType.None) colorChanger.Awake();
            if (!backpackModel) CreateBackpackModel();

            backpackModel.Color = colorChanger.Color;
            backpackModel.gameObject.SetActive(true);
            instance = this;
        }

        public void OnUnequip(GameObject sender, string slot)
        {
            if (!backpackModel) CreateBackpackModel();
            backpackModel.gameObject.SetActive(false);
            instance = null;
        }

        public void UpdateEquipped(GameObject sender, string slot)
        {

        }

        public void CreateBackpackModel()
        {
            var parent = Player.main.transform.Find("body/player_view/export_skeleton/head_rig/neck/chest/spine_3");
            var model = AmogusBackpack.GetGameObject();
            var obj = Instantiate(model, parent, Player.main.transform.position - (0.5f * Player.main.transform.forward), Player.main.transform.rotation, false);
            Destroy(obj.transform.Find("Collision").gameObject);

            foreach (var comp in obj.GetComponents<Component>()) if (comp is not SkyApplier && comp is not MogusColorChanger) Destroy(comp);//remove all components, this is simply the dummy model
            
            obj.transform.localEulerAngles = new Vector3(1, 157.8f, 271.9f);
            obj.transform.localPosition = new Vector3(0.2f, 0, -0.1f);
            backpackModel = obj.GetComponent<MogusColorChanger>();
        }
    }
}
