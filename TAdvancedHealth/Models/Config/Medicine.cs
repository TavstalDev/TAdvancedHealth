namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Medicine
    {
        public ushort ItemID { get; set; }
        public float HealsHeadHp { get; set; }
        public float HealsBodyHp { get; set; }
        public float HealsRightArmHp { get; set; }
        public float HealsLeftArmHp { get; set; }
        public float HealsRightLegHp { get; set; }
        public float HealsLeftLegHp { get; set; }
        public bool CuresPain { get; set; }

        public Medicine() { }

        public Medicine(ushort itemID, float healsHeadHp, float healsBodyHp, float healsRightArmHp, float healsLeftArmHp, float healsRightLegHp, float healsLeftLegHp, bool curesPain)
        {
            ItemID = itemID;
            HealsHeadHp = healsHeadHp;
            HealsBodyHp = healsBodyHp;
            HealsRightArmHp = healsRightArmHp;
            HealsLeftArmHp = healsLeftArmHp;
            HealsRightLegHp = healsRightLegHp;
            HealsLeftLegHp = healsLeftLegHp;
            CuresPain = curesPain;
        }
    }
}
