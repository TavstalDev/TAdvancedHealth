using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HospitalSettings
    {
        public bool EnableRespawnInHospital { get; set; }
        public bool RandomSpawn { get; set; }
        public List<Hospital> hospitals { get; set; }
    }
}
