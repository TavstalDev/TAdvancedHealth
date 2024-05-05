using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
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
