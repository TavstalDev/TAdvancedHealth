using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class Medicine
    {
        public ushort itemID { get; set; }
        public float HealsHeadHP { get; set; }
        public float HealsBodyHP { get; set; }
        public float HealsRightArmHP { get; set; }
        public float HealsLeftArmHP { get; set; }
        public float HealsRightLegHP { get; set; }
        public float HealsLeftLegHP { get; set; }
        public bool CuresPain { get; set; }

        public Medicine() { }
    }
}
