using System;
using static EquivalentExchange.QMod;
#if SN1
using SMLHelper.V2.Commands;
#else
using Nautilus.Commands;
#endif

namespace EquivalentExchange
{
    internal class Commands
    {
        [ConsoleCommand("ExchangeUnlockAll")]
        public static void ExchangeUnlockAll()
        {
            ErrorMessage.AddMessage("Unlocked all techtypes for exchange");
            foreach (string typeString in Enum.GetNames(typeof(TechType)))
            {
                TryUnlockTechType(GetTechType(typeString));
            }
        }

        [ConsoleCommand("ExchangeLockAll")]
        public static void ExchangeLockAll()
        {
            ErrorMessage.AddMessage("Locked all techtypes for exchange");
            QMod.SaveData.learntTechTypes.Clear();
        }

        [ConsoleCommand("UnlockExchangeType")]
        public static void UnlockExchangeType(string str)
        {
            var unlocked = TryUnlockTechType(GetTechType(str), out string reason);

            ErrorMessage.AddMessage(unlocked ? $"Unlocked {str}" : $"Could not unlock {str} due to: {reason}");
        }

        [ConsoleCommand("lockExchangeType")]
        public static void LockExchangeType(string str)
        {
            TechType type = GetTechType(str);
            if (type == TechType.None) return;
            if (QMod.SaveData.learntTechTypes.Contains(type))
                QMod.SaveData.learntTechTypes.Remove(type);
            ErrorMessage.AddMessage($"Locked {type}");
        }

        [ConsoleCommand("AddECM")]
        public static void AddAmount(int amount) => QMod.SaveData.ECMAvailable += amount;
    }
}
