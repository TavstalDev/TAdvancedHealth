using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    public class AntiGroupFriendlyFireSettings
    {
        public bool EnableAntiGroupFriendlyFire { get; set; }
        public bool EnableWarnMessage { get; set; }
        public float DelayBetweenMessages { get; set; }
        public string MessageIcon { get; set; }
        public string Message { get; set; }
        public List<string> groups { get; set; }
    }
}
