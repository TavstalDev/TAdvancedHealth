using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Defibrillator
    {
        [YamlMember(Order = 0, Description = "Item ID of the defibrillator")]
        public ushort ItemID { get; set; }
        [YamlMember(Order = 1, Description = "Cooldown in seconds between uses")]
        public double RechargeTimeSecs { get; set; }
        [YamlMember(Order = 2, Description = "Chance of successful revival")]
        public double ReviveChance { get; set; }

        public Defibrillator() { }

        public Defibrillator(ushort itemID, double rechargeTimeSecs, double reviveChance)
        {
            ItemID = itemID;
            RechargeTimeSecs = rechargeTimeSecs;
            ReviveChance = reviveChance;
        }
    }
}
