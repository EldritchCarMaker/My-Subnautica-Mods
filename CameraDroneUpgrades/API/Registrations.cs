using SMLHelper.V2.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !SN2
using Logger = QModManager.Utility.Logger;
#endif

namespace CameraDroneUpgrades.API
{
    public static class Registrations
    {
        public static string[] upgradeModulePaths = new[] { "DroneUpgrades" };
        public static List<CameraDroneUpgrade> upgrades = new List<CameraDroneUpgrade>();
        public static CameraDroneUpgrade RegisterDroneUpgrade(string name, TechType type, Action setupmethod)
        {
            var upgrade = new CameraDroneUpgrade(name, type, setupmethod);
#if !SN2
            Logger.Log(Logger.Level.Info, $"Recieved CameraDroneUpgrade: {name}");
#else
            QMod.logger.LogInfo($"Recieved CameraDroneUpgrade: {name}");
#endif
            if (upgrades.Contains(upgrade))
            {
#if !SN2
                QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Warn, $"Blocked duplicate upgrade: {name}");
#else
                QMod.logger.LogWarning($"Blocked duplicate upgrade: {name}");
#endif
                return upgrade;
            }
            upgrades.Add(upgrade);
            return upgrade;
        }
        public static CameraDroneUpgrade RegisterDroneUpgrade(string name, Craftable item, Action setupmethod)
        {
            return RegisterDroneUpgrade(name, item.TechType, setupmethod);
        }
    }
}
