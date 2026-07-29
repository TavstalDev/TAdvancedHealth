using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config.HealthSystem
{
    public class RestrictionSettings
    {
        [YamlMember(Order = 0, Description = "Kills the player when arm health reaches zero")]
        public bool DieWhenArmsHealthIsZero { get; set; }
        
        [YamlMember(Order = 1, Description = "Kills the player when leg health reaches zero")]
        public bool DieWhenLegsHealthIsZero { get; set; }
        
        [YamlMember(Order = 2, Description = "Kills the player when body health reaches zero")]
        public bool DieWhenBodyHealthIsZero { get; set; }
        
        [YamlMember(Order = 3, Description = "Kills the player when head health reaches zero")]
        public bool DieWhenHeadHealthIsZero { get; set; }
        
        [YamlMember(Order = 4, Description = "Allows driving with one broken leg")]
        public bool CanDriveWithOneBrokenLeg { get; set; }
        
        [YamlMember(Order = 5, Description = "Allows driving with both legs broken")]
        public bool CanDriveWithBrokenLegs { get; set; }
        
        [YamlMember(Order = 6, Description = "Allows driving with one broken arm")]
        public bool CanDriveWithOneBrokenArm { get; set; }
        
        [YamlMember(Order = 7, Description = "Allows driving with both arms broken")]
        public bool CanDriveWithBrokenArms { get; set; }
        
        [YamlMember(Order = 8, Description = "Allows holding one-handed items with one broken arm")]
        public bool CanHoldOneHandItemsWithOneBrokenArm { get; set; }
        
        [YamlMember(Order = 9, Description = "Allows holding two-handed items with one broken arm")]
        public bool CanHoldTwoHandItemsWithOneBrokenArm { get; set; }
        
        [YamlMember(Order = 10, Description = "Allows holding one-handed items with both arms broken")]
        public bool CanHoldOneHandItemsWithBrokenArms { get; set; }
        
        [YamlMember(Order = 11, Description = "Allows holding two-handed items with both arms broken")]
        public bool CanHoldTwoHandItemsWithBrokenArms { get; set; }
        
        [YamlMember(Order = 12, Description = "Allows jumping with one broken leg")]
        public bool CanJumpWithOneBrokenLeg { get; set; }
        
        [YamlMember(Order = 13, Description = "Allows jumping with both legs broken")]
        public bool CanJumpWithBrokenLegs { get; set; }

        public RestrictionSettings() { }

        public RestrictionSettings(bool dieWhenArmsHealthIsZero, bool dieWhenLegsHealthIsZero, bool dieWhenBodyHealthIsZero, bool dieWhenHeadHealthIsZero, bool canDriveWithOneBrokenLeg, bool canDriveWithBrokenLegs, bool canDriveWithOneBrokenArm, bool canDriveWithBrokenArms, bool canHoldOneHandItemsWithOneBrokenArm, bool canHoldTwoHandItemsWithOneBrokenArm, bool canHoldOneHandItemsWithBrokenArms, bool canHoldTwoHandItemsWithBrokenArms, bool canJumpWithOneBrokenLeg, bool canJumpWithBrokenLegs)
        {
            DieWhenArmsHealthIsZero = dieWhenArmsHealthIsZero;
            DieWhenLegsHealthIsZero = dieWhenLegsHealthIsZero;
            DieWhenBodyHealthIsZero = dieWhenBodyHealthIsZero;
            DieWhenHeadHealthIsZero = dieWhenHeadHealthIsZero;
            CanDriveWithOneBrokenLeg = canDriveWithOneBrokenLeg;
            CanDriveWithBrokenLegs = canDriveWithBrokenLegs;
            CanDriveWithOneBrokenArm = canDriveWithOneBrokenArm;
            CanDriveWithBrokenArms = canDriveWithBrokenArms;
            CanHoldOneHandItemsWithOneBrokenArm = canHoldOneHandItemsWithOneBrokenArm;
            CanHoldTwoHandItemsWithOneBrokenArm = canHoldTwoHandItemsWithOneBrokenArm;
            CanHoldOneHandItemsWithBrokenArms = canHoldOneHandItemsWithBrokenArms;
            CanHoldTwoHandItemsWithBrokenArms = canHoldTwoHandItemsWithBrokenArms;
            CanJumpWithOneBrokenLeg = canJumpWithOneBrokenLeg;
            CanJumpWithBrokenLegs = canJumpWithBrokenLegs;
        }
    }
}
