﻿namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Defibrillator
    {
        public ushort ItemID { get; set; }
        public double RechargeTimeSecs { get; set; }
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
