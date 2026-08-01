using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Medicine
    {
        [YamlMember(Order = 0)]
        public ushort ItemID { get; set; }
        [YamlMember(Order = 1)]
        public float HeadHp { get; set; }
        [YamlMember(Order = 2)]
        public float BodyHp { get; set; }
        [YamlMember(Order = 3)]
        public float RightArmHp { get; set; }
        [YamlMember(Order = 4)]
        public float LeftArmHp { get; set; }
        [YamlMember(Order = 5)]
        public float RightLegHp { get; set; }
        [YamlMember(Order = 6)]
        public float LeftLegHp { get; set; }
        [YamlMember(Order = 7)]
        public bool CuresPain { get; set; }

        public Medicine() { }

        public Medicine(ushort itemID, float headHp, float bodyHp, float rightArmHp, float leftArmHp, float rightLegHp, float leftLegHp, bool curesPain)
        {
            ItemID = itemID;
            HeadHp = headHp;
            BodyHp = bodyHp;
            RightArmHp = rightArmHp;
            LeftArmHp = leftArmHp;
            RightLegHp = rightLegHp;
            LeftLegHp = leftLegHp;
            CuresPain = curesPain;
        }
    }
}
