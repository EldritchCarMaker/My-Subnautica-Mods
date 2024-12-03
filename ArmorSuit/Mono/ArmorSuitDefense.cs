using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ArmorSuit.ArmorSuitMono;

namespace ArmorSuit.Mono
{
    internal class ArmorSuitDefense
    {
        private readonly Dictionary<DamageType, DamageModifier> DamageModifiers = new Dictionary<DamageType, DamageModifier>();

        internal void SetUpDamageModifiers()
        {
            foreach (DamageType type in Enum.GetValues(typeof(DamageType)))
            {
                AddDamageModifier(type);

                /*
                if (!ArmorSuitMono.UnaffectedTypes.Contains(type))//too lazy to copy paste all the types I do want. easier to filter the ones I don't 
                {
                    AddDamageModifier(type);
                }*/
            }
        }
        private void AddDamageModifier(DamageType type)
        {
            var modifier = Player.main.gameObject.AddComponent<DamageModifier>();
            modifier.damageType = type;
            modifier.multiplier = 1;

            DamageModifiers.Add(type, modifier);
        }

        internal void EditDamageModifiers(DefenseInfo defenseInfo, float multiplier)
        {
            foreach (var pair in DamageModifiers)
            {
                if (defenseInfo.DamageTypes.Contains(pair.Key))
                {
                    pair.Value.multiplier = multiplier;
                }
                else
                {
                    pair.Value.multiplier = 1;
                }
            }
        }
    }
}
