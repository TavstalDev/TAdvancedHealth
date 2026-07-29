using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class DefibrillatorSettings
    {
        [YamlMember(Order = 0, Description = "Enables defibrillator system")]
        public bool Enable { get; set; }
        [YamlMember(Order = 1, Description = "Requires permission to use defibrillators")]
        public bool EnablePermission { get; set; }
        [YamlMember(Order = 2, Description = "Permission required to use defibrillators")]
        public string Permission { get; set; }
        [YamlMember(Order = 3, Description = "List of defibrillator item definitions")]
        public List<Defibrillator> Items { get; set; }

        public DefibrillatorSettings()
        {
            Permission = "";
            Items = new List<Defibrillator>();
        }

        public DefibrillatorSettings(bool enable, bool enablePermission, string permission, List<Defibrillator> items)
        {
            Enable = enable;
            EnablePermission = enablePermission;
            Permission = permission;
            Items = items;
        }
    }
}
