namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Medicine
    {
        public ushort ItemID { get; set; }
        public float HealsHeadHP { get; set; }
        public float HealsBodyHP { get; set; }
        public float HealsRightArmHP { get; set; }
        public float HealsLeftArmHP { get; set; }
        public float HealsRightLegHP { get; set; }
        public float HealsLeftLegHP { get; set; }
        public bool CuresPain { get; set; }

        public Medicine() { }
    }
}
