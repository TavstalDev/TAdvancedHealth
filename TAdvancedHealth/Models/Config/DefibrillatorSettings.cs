using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class DefibrillatorSettings
    {
        public bool Enabled { get; set; }
        public bool EnablePermission { get; set; }
        public string PermissionForUseDefiblirator { get; set; }
        public List<Defibrillator> DefibrillatorItems { get; set; }

        public DefibrillatorSettings() { }

        public DefibrillatorSettings(bool enabled, bool enablePermission, string permissionForUseDefiblirator, List<Defibrillator> defibrillatorItems)
        {
            Enabled = enabled;
            EnablePermission = enablePermission;
            PermissionForUseDefiblirator = permissionForUseDefiblirator;
            DefibrillatorItems = defibrillatorItems;
        }
    }
}
