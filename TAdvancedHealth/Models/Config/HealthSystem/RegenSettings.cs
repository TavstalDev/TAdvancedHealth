using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config.HealthSystem
{
    public class RegenSettings
    {
        [YamlMember(Order = 0, Description = "Minimum food % required for health regen")]
        public float HealthRegenMinFood { get; set; }
        
        [YamlMember(Order = 1, Description = "Minimum water % required for health regen")]
        public float HealthRegenMinWater { get; set; }
        
        [YamlMember(Order = 2, Description = "Minimum virus % required for health regen")]
        public float HealthRegenMinVirus { get; set; }
        
        [YamlMember(Order = 3, Description = "Ticks between leg health regeneration")]
        public float LegRegenTicks { get; set; }
        
        [YamlMember(Order = 4, Description = "Ticks between arm health regeneration")]
        public float ArmRegenTicks { get; set; }
        
        [YamlMember(Order = 5, Description = "Ticks between body health regeneration")]
        public float BodyRegenTicks { get; set; }
        
        [YamlMember(Order = 6, Description = "Ticks between head health regeneration")]
        public float HeadRegenTicks { get; set; }

        public RegenSettings() { }

        public RegenSettings(float healthRegenMinFood, float healthRegenMinWater, float healthRegenMinVirus, float legRegenTicks, float armRegenTicks, float bodyRegenTicks, float headRegenTicks)
        {
            HealthRegenMinFood = healthRegenMinFood;
            HealthRegenMinWater = healthRegenMinWater;
            HealthRegenMinVirus = healthRegenMinVirus;
            LegRegenTicks = legRegenTicks;
            ArmRegenTicks = armRegenTicks;
            BodyRegenTicks = bodyRegenTicks;
            HeadRegenTicks = headRegenTicks;
        }
    }
}
