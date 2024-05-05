using System;
using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    [Serializable]
    public class HUDStyle
    {
        public bool Enabled { get; set; }
        public string Name { get; set; }
        public List<string> Aliases { get; set; }
        public ushort EffectID { get; set; }

        public HUDStyle() { }

        public HUDStyle(bool enabled, string name, List<string> aliases, ushort effectID)
        {
            Enabled = enabled;
            Name = name;
            Aliases = aliases;
            EffectID = effectID;
        }
    }
}
