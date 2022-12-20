using CyclopsTorpedoes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECMModLogger.ECMCapsules
{
    public class CapsuleManager
    {
        internal static bool CollectedFive;
        public static void CapsuleCollected(string capsuleID)
        {
            if(QMod.Save.CollectedCapsules.Contains(capsuleID))
            {
                ErrorMessage.AddMessage("Collected twice");
            }
            QMod.Save.CollectedCapsules.Add(capsuleID);
            if(QMod.Save.CollectedCapsules.Count == 5)
            {
                ErrorMessage.AddMessage("collected 5");
                CollectedFive = true;
            }
        }
    }
}
