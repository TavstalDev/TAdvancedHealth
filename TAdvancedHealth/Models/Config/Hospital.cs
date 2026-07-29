using System.Collections.Generic;
using Tavstal.TLibrary.Models;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Hospital
    {
        [YamlMember(Order = 0, Description = "Display name of the hospital")]
        public string Name { get; set; }
        [YamlMember(Order = 1, Description = "Permission required to respawn at this hospital")]
        public string Permission { get; set; }
        [YamlMember(Order = 2, Description = "Spawn position at the hospital")]
        public List<SerializableVector3>? Position { get; set; }

        public Hospital()
        {
            Name = string.Empty;
            Permission = string.Empty;
        }

        public Hospital(string name, string permission, List<SerializableVector3>? position)
        {
            Name = name;
            Permission = permission;
            Position = position;
        }
    }
}
