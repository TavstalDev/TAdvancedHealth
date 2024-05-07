using Tavstal.TAdvancedHealth.Models.Enums;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class StatusIcon
    {
        public EPlayerState Status { get; set; }
        public string IconUrl { get; set; }
        public int GroupIndex { get; set; }

        public StatusIcon() { }

        public StatusIcon(EPlayerState status, string iconUrl, int groupIndex)
        {
            Status = status;
            IconUrl = iconUrl;
            GroupIndex = groupIndex;
        }
    }
}
