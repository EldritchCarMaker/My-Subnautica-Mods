using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquivalentExchange
{
    public class ExternalModCompat
    {
        public static void SetTechTypeValue(TechType tech, int value)
        {
            if(!QMod.config.BaseMaterialCosts.TryGetValue(tech, out _)) 
            {
                QMod.config.BaseMaterialCosts.Add(tech, value);
            }
            else
            {
                QMod.config.BaseMaterialCosts[tech] = value;
            }
        }
    }
}
