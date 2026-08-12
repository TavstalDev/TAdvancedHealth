using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Defibrillator
    {
        [YamlMember(Order = 0)]
        public ushort ItemId { get; set; }
        [YamlMember(Order = 1)]
        public double RechargeTimeSecs { get; set; }
        [YamlMember(Order = 2)]
        public double ReviveChance { get; set; }

        public Defibrillator() { }

        public Defibrillator(ushort itemID, double rechargeTimeSecs, double reviveChance)
        {
            ItemId = itemID;
            RechargeTimeSecs = rechargeTimeSecs;
            ReviveChance = reviveChance;
        }
    }
}
