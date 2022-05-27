using SMLHelper.V2.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = QModManager.Utility.Logger;

namespace CameraDroneUpgrades.API
{
    public static class Registrations
    {
        public static List<CameraDroneUpgrade> upgrades = new List<CameraDroneUpgrade>();
        public static CameraDroneUpgrade RegisterDroneUpgrade(string name, Craftable item, Action setupmethod)
        {
            var upgrade = new CameraDroneUpgrade(name, item, setupmethod);
            Logger.Log(Logger.Level.Info, $"Recieved CameraDroneUpgrade: {name}");
            if (upgrades.Contains(upgrade))
            {
                QModManager.Utility.Logger.Log(QModManager.Utility.Logger.Level.Warn, $"Blocked duplicate upgrade: {name}");
                return upgrade;
            }
            upgrades.Add(upgrade);
            return upgrade;
        }
    }
}
