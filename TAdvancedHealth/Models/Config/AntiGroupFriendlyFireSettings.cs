using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class AntiGroupFriendlyFireSettings
    {
        [YamlMember(Order = 0, Description = "Enables anti friendly fire system")]
        public bool Enable { get; set; }
        
        [YamlMember(Order = 1, Description = "Shows warning message on friendly fire")]
        public bool EnableWarnMessage { get; set; }
        
        [YamlMember(Order = 2, Description = "Delay in seconds between warning messages")]
        public float DelayBetweenMessages { get; set; }
        
        [YamlMember(Order = 3, Description = "Icon URL shown in the warning message")]
        public string MessageIcon { get; set; }
        
        [YamlMember(Order = 4, Description = "Warning message text for friendly fire")]
        public string Message { get; set; }
        
        [YamlMember(Order = 5, Description = "List of group names protected by anti friendly fire")]
        public List<string> Groups { get; set; }

        public AntiGroupFriendlyFireSettings()
        {
            MessageIcon = "";
            Message = "";
            Groups = new List<string>();
        }

        public AntiGroupFriendlyFireSettings(bool enable, bool enableWarnMessage, float delayBetweenMessages, string messageIcon, string message, List<string> groups)
        {
            Enable = enable;
            EnableWarnMessage = enableWarnMessage;
            DelayBetweenMessages = delayBetweenMessages;
            MessageIcon = messageIcon;
            Message = message;
            Groups = groups;
        }
    }
}
