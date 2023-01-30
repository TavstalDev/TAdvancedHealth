using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class HospitalSettings
    {
        public bool EnableRespawnInHospital { get; set; }
        public bool RandomSpawn { get; set; }
        public List<Hospital> hospitals { get; set; }
    }
}
