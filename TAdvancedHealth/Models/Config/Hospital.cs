using System.Collections.Generic;
using Tavstal.TLibrary.Compatibility;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Hospital
    {
        public string Name { get; set; }
        public string SpawnPermission { get; set; }
        public List<SerializableVector3> Position { get; set; }
    }
}
