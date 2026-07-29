using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config.HealthSystem
{
    public class CombatSettings
    {
        [YamlMember(Order = 0, Description = "Enables the bleeding system")]
        public bool CanStartBleeding { get; set; }
        
        [YamlMember(Order = 1, Description = "Chance of heavy bleeding on injury")]
        public float HeavyBleedingChance { get; set; }
        
        [YamlMember(Order = 2, Description = "Damage per tick for normal bleeding")]
        public float BleedingDamage { get; set; }
        
        [YamlMember(Order = 3, Description = "Damage per tick for heavy bleeding")]
        public float HeavyBleedingDamage { get; set; }
        
        [YamlMember(Order = 4, Description = "Enables the injured/downed state")]
        public bool CanBeInjured { get; set; }
        
        [YamlMember(Order = 5, Description = "Seconds before death when in injured state")]
        public double InjuredDeathTimeSecs { get; set; }
        
        [YamlMember(Order = 6, Description = "Chance of becoming injured on fatal damage")]
        public float InjuredChance { get; set; }
        
        [YamlMember(Order = 7, Description = "Enables the pain screen effect")]
        public bool CanHavePainEffect { get; set; }
        
        [YamlMember(Order = 8, Description = "Chance of triggering the pain effect")]
        public float PainEffectChance { get; set; }
        
        [YamlMember(Order = 9, Description = "Duration of the pain effect in seconds")]
        public float PainEffectDuration { get; set; }

        public CombatSettings() { }

        public CombatSettings(bool canStartBleeding, float heavyBleedingChance, float bleedingDamage, float heavyBleedingDamage, bool canBeInjured, double injuredDeathTimeSecs, float injuredChance, bool canHavePainEffect, float painEffectChance, float painEffectDuration)
        {
            CanStartBleeding = canStartBleeding;
            HeavyBleedingChance = heavyBleedingChance;
            BleedingDamage = bleedingDamage;
            HeavyBleedingDamage = heavyBleedingDamage;
            CanBeInjured = canBeInjured;
            InjuredDeathTimeSecs = injuredDeathTimeSecs;
            InjuredChance = injuredChance;
            CanHavePainEffect = canHavePainEffect;
            PainEffectChance = painEffectChance;
            PainEffectDuration = painEffectDuration;
        }
    }
}
