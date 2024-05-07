using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HospitalSettings
    {
        public bool EnableRespawnInHospital { get; set; }
        public bool RandomSpawn { get; set; }
        public List<Hospital> Hospitals { get; set; }

        public HospitalSettings() { }

        public HospitalSettings(bool enableRespawnInHospital, bool randomSpawn, List<Hospital> hospitals)
        {
            EnableRespawnInHospital = enableRespawnInHospital;
            RandomSpawn = randomSpawn;
            Hospitals = hospitals;
        }
    }
}
