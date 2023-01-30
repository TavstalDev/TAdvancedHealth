using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class StatusIcon
    {
        public EPlayerStates Status { get; set; }
        public string IconUrl { get; set; }
        public int GroupIndex { get; set; }
    }
}
