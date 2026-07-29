using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Models.Config.HealthSystem;
using YamlDotNet.Serialization;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HealthSystemSettings
    {
        [YamlMember(Order = 0, Description = "Effect ID for the pain screen overlay")]
        public ushort PainEffectID { get; set; }
        
        [YamlMember(Order = 1, Description = "Enables per-limb health system")]
        public bool EnableTarkovLikeHealth { get; set; }
        
        [YamlMember(Order = 2, Description = "Base health when Tarkov-like health is disabled")]
        public float BaseHealth { get; set; }
        
        [YamlMember(Order = 3, Description = "Per-limb maximum health values")]
        public LimbHealthSettings LimbHealth { get; set; } = new LimbHealthSettings();

        [YamlMember(Order = 4, Description = "Movement speed and walking restriction settings")]
        public MovementSettings Movement { get; set; } = new MovementSettings();

        [YamlMember(Order = 5, Description = "Death, driving, item and jump restriction settings")]
        public RestrictionSettings Restrictions { get; set; } = new RestrictionSettings();

        [YamlMember(Order = 6, Description = "Bleeding, injury and pain effect settings")]
        public CombatSettings Combat { get; set; } = new CombatSettings();

        [YamlMember(Order = 7, Description = "Health regeneration tick settings")]
        public RegenSettings Regen { get; set; } = new RegenSettings();

        [YamlMember(Order = 8, Description = "Status effect icon configurations")]
        public List<StatusIcon> StatusIcons { get; set; } = new List<StatusIcon>();

        [YamlIgnore]
        public float HeadHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.Head;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.Head = value;
            }
        }
        
        [YamlIgnore]
        public float BodyHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.Body;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.Body = value;
            }
        }
        
        [YamlIgnore]
        public float RightArmHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.RightArm;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.RightArm = value;
            }
        }
        
        [YamlIgnore]
        public float RightLegHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.RightLeg;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.RightLeg = value;
            }
        }
        
        [YamlIgnore]
        public float LeftArmHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.LeftArm;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.LeftArm = value;
            }
        }
        
        [YamlIgnore]
        public float LeftLegHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return LimbHealth.LeftLeg;
                return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    LimbHealth.LeftLeg = value;
            }
        }
    }
}
