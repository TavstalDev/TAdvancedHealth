using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config.HealthSystem
{
    public class LimbHealthSettings
    {
        [YamlMember(Order = 0, Description = "Maximum head health value")]
        public float Head { get; set; }
        
        [YamlMember(Order = 1, Description = "Maximum body health value")]
        public float Body { get; set; }
        
        [YamlMember(Order = 2, Description = "Maximum right arm health value")]
        public float RightArm { get; set; }
        
        [YamlMember(Order = 3, Description = "Maximum right leg health value")]
        public float RightLeg { get; set; }
        
        [YamlMember(Order = 4, Description = "Maximum left arm health value")]
        public float LeftArm { get; set; }
        
        [YamlMember(Order = 5, Description = "Maximum left leg health value")]
        public float LeftLeg { get; set; }

        public LimbHealthSettings() { }

        public LimbHealthSettings(float head, float body, float rightArm, float rightLeg, float leftArm, float leftLeg)
        {
            Head = head;
            Body = body;
            RightArm = rightArm;
            RightLeg = rightLeg;
            LeftArm = leftArm;
            LeftLeg = leftLeg;
        }
    }
}
