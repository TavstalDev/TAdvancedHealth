using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config.HealthSystem
{
    public class MovementSettings
    {
        [YamlMember(Order = 0, Description = "Default player walk speed multiplier")]
        public float DefaultWalkSpeed { get; set; }
        
        [YamlMember(Order = 1, Description = "Walk speed when one leg is broken")]
        public float WalkSpeedWithOneBrokenLeg { get; set; }
        
        [YamlMember(Order = 2, Description = "Walk speed when both legs are broken")]
        public float WalkSpeedWithBrokenLegs { get; set; }
        
        [YamlMember(Order = 3, Description = "Allows walking with one broken leg")]
        public bool CanWalkWithOneBrokenLeg { get; set; }
        
        [YamlMember(Order = 4, Description = "Allows walking with both legs broken")]
        public bool CanWalkWithBrokenLegs { get; set; }

        public MovementSettings() { }

        public MovementSettings(float defaultWalkSpeed, float walkSpeedWithOneBrokenLeg, float walkSpeedWithBrokenLegs, bool canWalkWithOneBrokenLeg, bool canWalkWithBrokenLegs)
        {
            DefaultWalkSpeed = defaultWalkSpeed;
            WalkSpeedWithOneBrokenLeg = walkSpeedWithOneBrokenLeg;
            WalkSpeedWithBrokenLegs = walkSpeedWithBrokenLegs;
            CanWalkWithOneBrokenLeg = canWalkWithOneBrokenLeg;
            CanWalkWithBrokenLegs = canWalkWithBrokenLegs;
        }
    }
}
