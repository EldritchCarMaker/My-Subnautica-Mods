using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyBlock.BlockEvents
{
    public interface IDelayedEffect
    {
        public float Delay { get; }
        public string Message { get; }
    }
}
