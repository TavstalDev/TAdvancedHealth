using System.Collections.Generic;
using Tavstal.TLibrary.Models;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Hospital
    {
        public string Name { get; set; }
        public string SpawnPermission { get; set; }
        public List<SerializableVector3> Position { get; set; }

        public Hospital() { }

        public Hospital(string name, string spawnPermission, List<SerializableVector3> position)
        {
            Name = name;
            SpawnPermission = spawnPermission;
            Position = position;
        }
    }
}
