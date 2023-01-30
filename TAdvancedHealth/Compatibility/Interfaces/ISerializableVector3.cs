using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public interface ISerializableVector3
    {
        float X { get; set; }
        float Y { get; set; }
        float Z { get; set; }

        bool Equals(object obj);
        Vector3 GetVector3();
    }
}
