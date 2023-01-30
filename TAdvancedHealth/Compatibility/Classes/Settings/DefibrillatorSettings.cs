using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Compatibility;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class DefibrillatorSettings
    {
        public bool Enabled { get; set; }
        public bool EnablePermission { get; set; }
        public string PermissionForUseDefiblirator { get; set; }
        public List<Defibrillator> DefibrillatorItems { get; set; }
    }
}
