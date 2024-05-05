using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class DefibrillatorSettings
    {
        public bool Enabled { get; set; }
        public bool EnablePermission { get; set; }
        public string PermissionForUseDefiblirator { get; set; }
        public List<Defibrillator> DefibrillatorItems { get; set; }
    }
}
