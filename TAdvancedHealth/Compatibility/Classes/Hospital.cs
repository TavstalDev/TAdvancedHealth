using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class Hospital
    {
        public string Name { get; set; }
        public string SpawnPermission { get; set; }
        public List<SerializableVector3> Position { get; set; }
    }
}
