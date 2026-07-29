using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HospitalSettings
    {
        [YamlMember(Order = 0, Description = "Enables hospital respawn system")]
        public bool EnableRespawnInHospital { get; set; }
        [YamlMember(Order = 1, Description = "Randomizes spawn point within hospital area")]
        public bool RandomSpawn { get; set; }
        [YamlMember(Order = 2, Description = "List of hospital definitions")]
        public List<Hospital> Hospitals { get; set; }

        public HospitalSettings()
        {
            Hospitals = new List<Hospital>();
        }

        public HospitalSettings(bool enableRespawnInHospital, bool randomSpawn, List<Hospital> hospitals)
        {
            EnableRespawnInHospital = enableRespawnInHospital;
            RandomSpawn = randomSpawn;
            Hospitals = hospitals;
        }
    }
}
