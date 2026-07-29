using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Config.HealthSystem;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Models.Config;
using YamlDotNet.Serialization;
// ReSharper disable ClassNeverInstantiated.Global

namespace Tavstal.TAdvancedHealth
{
    public class AdvancedHealthConfig : YamlConfiguration
    {
        [YamlMember(Order = 3, Description = "Database connection configuration")] 
        public DatabaseData Database { get; set; } = new DatabaseData();
        
        [YamlMember(Order = 4, Description = "Defibrillator item and permission settings")]
        public DefibrillatorSettings DefibrillatorSettings { get; set; } = new DefibrillatorSettings();
        
        [YamlMember(Order = 5, Description = "Hospital respawn configuration")]
        public HospitalSettings HospitalSettings { get; set; } = new HospitalSettings();
        
        [YamlMember(Order = 6, Description = "Anti friendly fire settings per group")]
        public AntiGroupFriendlyFireSettings AntiGroupFriendlyFireSettings { get; set; } = new AntiGroupFriendlyFireSettings();
        
        [YamlMember(Order = 7, Description = "Core health system configuration")]
        public HealthSystemSettings HealthSystemSettings { get; set; } = new  HealthSystemSettings();
        
        [YamlMember(Order = 8, Description = "List of available HUD style presets")]
        public List<HUDStyle> HUDStyles { get; set; } = new List<HUDStyle>();
        
        [YamlMember(Order = 9, Description = "List of medical items and their healing values")]
        public List<Medicine> Medicines { get; set; } = new List<Medicine>();
        
        [YamlMember(Order = 10, Description = "Item types/IDs restricted to one-handed use")]
        public RestrictedItems OneHandedItems { get; set; } = new RestrictedItems();
        
        [YamlMember(Order = 11, Description = "Item types/IDs restricted to two-handed use")] 
        public RestrictedItems TwoHandedItems { get; set; } = new RestrictedItems();

        public override void LoadDefaults()
        {
            General = new GeneralConfig
            {
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon.png"
            };
            Database = new DatabaseData();
            DefibrillatorSettings = new DefibrillatorSettings
            {
                Enable = true,
                EnablePermission = false,
                Permission = "EMS.defibrillator",
                Items = new List<Defibrillator>
                {
                    new Defibrillator { ItemID = 21380, RechargeTimeSecs = 5, ReviveChance = 100 },
                }
            };
            HospitalSettings = new HospitalSettings
            {
                EnableRespawnInHospital = false,
                RandomSpawn = true,
                Hospitals = new List<Hospital>
                {
                    new Hospital { Name = "Hospital 1", Permission = "respawn.hospital1", Position = null }
                }
            };
            AntiGroupFriendlyFireSettings = new AntiGroupFriendlyFireSettings
            {
                Enable = false,
                EnableWarnMessage = true,
                Message = "You are attacking a friendly player.",
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon.png",
                Groups = new List<string> { "police", "swat" }
            };
            HealthSystemSettings = new HealthSystemSettings
            {
                EnableTarkovLikeHealth = true,
                PainEffectID = 0,
                BaseHealth = 100.0f,
                BodyHealth = 70.0f,
                HeadHealth = 35.0f,
                LeftArmHealth = 60.0f,
                RightArmHealth = 60.0f,
                LeftLegHealth = 65.0f,
                RightLegHealth = 65.0f,
                Regen = new RegenSettings
                {
                    ArmRegenTicks = 60,
                    BodyRegenTicks = 65,
                    HeadRegenTicks = 120,
                    LegRegenTicks = 40,
                    HealthRegenMinFood = 75,
                    HealthRegenMinVirus = 75,
                    HealthRegenMinWater = 75
                },
                Restrictions = new RestrictionSettings
                {
                    CanDriveWithBrokenLegs = false,
                    CanDriveWithOneBrokenLeg = false,
                    CanDriveWithBrokenArms = false,
                    CanDriveWithOneBrokenArm = false,
                    CanHoldOneHandItemsWithBrokenArms = false,
                    CanHoldOneHandItemsWithOneBrokenArm = true,
                    CanHoldTwoHandItemsWithBrokenArms = false,
                    CanHoldTwoHandItemsWithOneBrokenArm = false,
                    CanJumpWithBrokenLegs = false,
                    CanJumpWithOneBrokenLeg = false,
                    DieWhenArmsHealthIsZero = false,
                    DieWhenBodyHealthIsZero = true,
                    DieWhenHeadHealthIsZero = true,
                    DieWhenLegsHealthIsZero = false
                },
                Combat = new CombatSettings
                {
                    CanStartBleeding = true,
                    BleedingDamage = 3,
                    HeavyBleedingChance = 25,
                    HeavyBleedingDamage = 6,
                    CanBeInjured = true,
                    InjuredChance = 50,
                    InjuredDeathTimeSecs = 60,
                    CanHavePainEffect = false,
                    PainEffectChance = 25,
                    PainEffectDuration = -1
                },
                Movement = new MovementSettings
                {
                    CanWalkWithBrokenLegs = false,
                    CanWalkWithOneBrokenLeg = false,
                    DefaultWalkSpeed = 1.0f,
                    WalkSpeedWithOneBrokenLeg = 0.5f,
                    WalkSpeedWithBrokenLegs = 0.5f
                },
                StatusIcons = new List<StatusIcon>
                {
                    new StatusIcon
                    {
                        Status = EPlayerState.Acid,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_acid.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Bleeding,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_bleeding.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.BrokenBone,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_brokenbone.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Burning,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_temperature_hot.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Cold,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_temperature_down.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Covered,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_temperature_shelter.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.DeadZone,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_deadzone.png",
                        GroupIndex = 2
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Freezing,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_temperature_cold.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Handcuffed,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_handcuffed.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NoFood,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_food.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NoWater,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_water.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NoOxygen,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_lunge.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NoVirus,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_radiation.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.SafeZone,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_safezone.png",
                        GroupIndex = 2

                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.Warm,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_temperature_up.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.FullMoon,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/TAdvancedHealth/refs/heads/master/assets/icon_fullmoon.png",
                        GroupIndex = -1
                    }
                }
            };
            HUDStyles = new List<HUDStyle>()
            {
                new HUDStyle()
                {
                    Enable = true,
                    Name = "Square",
                    Aliases = new List<string> { "S" },
                    EffectID = 8807
                },
                new HUDStyle()
                {
                    Enable = true,
                    Name = "Circular",
                    Aliases = new List<string> { "C" },
                    EffectID = 8808
                },
                new HUDStyle()
                {
                    Enable = true,
                    Name = "Square Single Health",
                    Aliases = new List<string> { "SSH" },
                    EffectID = 8809
                },
                new HUDStyle()
                {
                    Enable = true,
                    Name = "Circular Single Health",
                    Aliases = new List<string> { "CSH" },
                    EffectID = 8810
                },
                new HUDStyle()
                {
                    Enable = true,
                    Name = "Tarkov Like",
                    Aliases = new List<string> { "TL" },
                    EffectID = 8811
                },
            };
            Medicines = new List<Medicine>
            {
                new Medicine { ItemID = 15, HeadHp = 26.25f, BodyHp = 0, LeftArmHp = 0, LeftLegHp = 0, RightArmHp = 0, RightLegHp = 0, CuresPain = false },
                new Medicine { ItemID = 403, HeadHp = 0, BodyHp = 52.5f, LeftArmHp = 0, LeftLegHp = 0, RightArmHp = 0, RightLegHp = 0, CuresPain = false },
                new Medicine { ItemID = 96, HeadHp = 0, BodyHp = 0, LeftArmHp = 0, LeftLegHp = 45, RightArmHp = 0, RightLegHp = 45, CuresPain = false },
                new Medicine { ItemID = 95, HeadHp = 0, BodyHp = 0, LeftArmHp = 32.5f, LeftLegHp = 0, RightArmHp = 32.5f, RightLegHp = 0, CuresPain = false },
                new Medicine { ItemID = 394, HeadHp = 0, BodyHp = 0, LeftArmHp = 45, LeftLegHp = 0, RightArmHp = 45, RightLegHp = 0, CuresPain = false },
                new Medicine { ItemID = 390, HeadHp = 0, BodyHp = 0, LeftArmHp = 0, LeftLegHp = 30, RightArmHp = 0, RightLegHp = 30, CuresPain = true }
            };
            OneHandedItems = new RestrictedItems
            {
                 ItemTypes = new List<EItemType>
                 {
                      EItemType.BACKPACK, EItemType.FOOD, EItemType.MEDICAL, EItemType.WATER, EItemType.VEHICLE_REPAIR_TOOL, EItemType.FILTER, EItemType.GLASSES, EItemType.HAT, EItemType.FISHER,
                      EItemType.MASK, EItemType.OPTIC, EItemType.PANTS, EItemType.SHIRT, EItemType.TOOL, EItemType.VEST
                 },
                 Items = new List<ushort>
                 {
                     138
                 }
            };
            TwoHandedItems = new RestrictedItems
            {
                ItemTypes = new List<EItemType>
                 {
                    EItemType.CHARGE, EItemType.BARREL, EItemType.BARRICADE, EItemType.BEACON, EItemType.BOX, EItemType.DETONATOR, EItemType.FARM,
                    EItemType.FUEL, EItemType.GENERATOR, EItemType.GRIP, EItemType.GROWER, EItemType.GUN, EItemType.KEY, EItemType.LIBRARY, EItemType.MAGAZINE, EItemType.MAP, EItemType.MELEE,
                    EItemType.OIL_PUMP, EItemType.REFILL, EItemType.SENTRY, EItemType.SIGHT, EItemType.STORAGE, EItemType.STRUCTURE, EItemType.SUPPLY, EItemType.TACTICAL, EItemType.TANK, EItemType.THROWABLE,
                    EItemType.TIRE, EItemType.TRAP
                 },
                Items = new List<ushort>
                {
                    519
                }
            };
        }
        
        public AdvancedHealthConfig() { }
        public AdvancedHealthConfig(string fileName, string path) : base(fileName, path) { }
    }
}
