using HarmonyLib;
using SMLHelper.V2.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Logger = QModManager.Utility.Logger;

namespace AutoStorageTransfer.Monobehaviours
{
    internal class FCSCompatStorageTransfer : StorageTransfer
    {
        public object fakeContainer;
        public void Awake()
        {
            Type displayManagerType = Patches.FCSCompatPatches.FindType("FCS_StorageSolutions", "FCS_StorageSolutions.Mods.DataStorageSolutions.Mono.Terminal.DSSTerminalDisplayManager");

            var displayManager = GetComponent(displayManagerType);

            var baseManager = displayManagerType.GetField("_currentBase", AccessTools.all).GetValue(displayManager);

            fakeContainer = baseManager.GetType().GetField("_dumpContainer", AccessTools.all).GetValue(baseManager);

            _itemsContainer = (ItemsContainer)fakeContainer.GetType().GetField("_dumpContainer", AccessTools.all).GetValue(fakeContainer);
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if(Time.time >= timeLastThoroughSort + THOROUGHSORTCOOLDOWN)
            {
                fakeContainer.GetType().GetMethod("OnDumpClose", AccessTools.all).Invoke(fakeContainer, new[] { Player.main.GetPDA() } );
            }
        }
    }
}
