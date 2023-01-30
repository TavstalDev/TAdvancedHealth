using System;
using UnityEngine;
using System.Collections.Generic;
using SDG.Unturned;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    [Serializable]
    public class HUDStyle
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public ushort EffectID { get; set; }

        public HUDStyle() { }
    }
}
