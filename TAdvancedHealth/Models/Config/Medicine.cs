using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class Medicine
    {
        [YamlMember(Order = 0, Description = "Item ID of the medicine")]
        public ushort ItemID { get; set; }
        [YamlMember(Order = 1, Description = "Health restored to the head")]
        public float HeadHp { get; set; }
        [YamlMember(Order = 2, Description = "Health restored to the body")]
        public float BodyHp { get; set; }
        [YamlMember(Order = 3, Description = "Health restored to the right arm")]
        public float RightArmHp { get; set; }
        [YamlMember(Order = 4, Description = "Health restored to the left arm")]
        public float LeftArmHp { get; set; }
        [YamlMember(Order = 5, Description = "Health restored to the right leg")]
        public float RightLegHp { get; set; }
        [YamlMember(Order = 6, Description = "Health restored to the left leg")]
        public float LeftLegHp { get; set; }
        [YamlMember(Order = 7, Description = "Removes the pain effect on use")]
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
