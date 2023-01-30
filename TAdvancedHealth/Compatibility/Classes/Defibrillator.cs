using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class Defibrillator
    {
        public ushort ItemID { get; set; }
        public double RechargeTimeSecs { get; set; }
        public double ReviveChance { get; set; }
    }
}
