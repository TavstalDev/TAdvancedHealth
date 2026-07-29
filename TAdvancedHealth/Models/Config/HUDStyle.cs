using System;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HUDStyle
    {
        [YamlMember(Order = 0, Description = "Enables this HUD style")]
        public bool Enable { get; set; }
        [YamlMember(Order = 1, Description = "Name identifier for the HUD style")]
        public string Name { get; set; }
        [YamlMember(Order = 2, Description = "Alternative names for the HUD style")]
        public List<string> Aliases { get; set; }
        [YamlMember(Order = 3, Description = "Effect ID for the HUD overlay")]
        public ushort EffectID { get; set; }

        public HUDStyle()
        {
            Name = string.Empty;
            Aliases = new List<string>();
        }

        public HUDStyle(bool enable, string name, List<string> aliases, ushort effectID)
        {
            Enable = enable;
            Name = name;
            Aliases = aliases;
            EffectID = effectID;
        }
    }
}
