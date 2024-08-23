using Newtonsoft.Json;
using SDG.Unturned;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TAdvancedHealth
{
    public class TAdvancedHealthConfig : ConfigurationBase
    {
        [JsonProperty(Order = 3)]
        public DatabaseData Database { get; set; }
        [JsonProperty(Order = 4)]
        public string MessageIcon { get; set; }
        [JsonProperty(Order = 5)]
        public DefibrillatorSettings DefibrillatorSettings { get; set; }
        [JsonProperty(Order = 6)]
        public HospitalSettings HospitalSettings { get; set; }
        [JsonProperty(Order = 7)]
        public AntiGroupFriendlyFireSettings AntiGroupFriendlyFireSettings { get; set; }
        [JsonProperty(Order = 8)]
        public HealthSystemSettings HealthSystemSettings { get; set; }
        [JsonProperty(Order = 9)]
        public List<HUDStyle> HUDStyles { get; set; }
        [JsonProperty(Order = 10)]
        public List<Medicine> Medicines { get; set; }
        [JsonProperty(Order = 11)]
        public RestrictedItems OneHandedItems { get; set; }
        [JsonProperty(Order = 12)]
        public RestrictedItems TwoHandedItems { get; set; }

        public override void LoadDefaults()
        {
            Database = new DatabaseData()
            {
                DatabaseTable_PlayerData = "tadh_players"
            };
            MessageIcon = "https://raw.githubusercontent.com/TavstalDev/Icons/master/Plugins/icon_plugins_TAdvancedHealth.png";
            DefibrillatorSettings = new DefibrillatorSettings
            {
                Enabled = true,
                EnablePermission = false,
                PermissionForUseDefiblirator = "EMS.defibrillator",
                DefibrillatorItems = new List<Defibrillator>
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
                    new Hospital { Name = "Hospital 1", SpawnPermission = "respawn.hospital1", Position = null }
                }
            };
            AntiGroupFriendlyFireSettings = new AntiGroupFriendlyFireSettings
            {
                EnableAntiGroupFriendlyFire = false,
                EnableWarnMessage = true,
                Message = "You are attacking a friendly player.",
                MessageIcon = "https://raw.githubusercontent.com/TavstalDev/Icons/master/Plugins/icon_plugins_TAdvancedHealth.png",
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
                ArmRegenTicks = 60,
                BodyRegenTicks = 65,
                HeadRegenTicks = 120,
                LegRegenTicks = 40,
                HealthRegenMinFood = 75,
                HealthRegenMinVirus = 75,
                HealthRegenMinWater = 75,
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
                CanStartBleeding = true,
                BleedingDamage = 3,
                HeavyBleedingChance = 25,
                HeavyBleedingDamage = 6,
                CanWalkWithBrokenLegs = false,
                CanWalkWithOneBrokenLeg = false,
                CanBeInjured = true,
                InjuredChance = 50,
                InjuredDeathTimeSecs = 60,
                DefaultWalkSpeed = 1.0f,
                WalkSpeedWithOneBrokenLeg = 0.5f,
                WalkSpeedWithBrokenLegs = 0.5f,
                CanHavePainEffect = false,
                PainEffectChance = 25,
                PainEffectDuration = -1,
                DieWhenArmsHealthIsZero = false,
                DieWhenBodyHealthIsZero = true,
                DieWhenHeadHealthIsZero = true,
                DieWhenLegsHealthIsZero = false,
                StatusIcons = new List<StatusIcon>
                {
                    new StatusIcon
                    {
                        Status = EPlayerState.ACID,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_acid.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.BLEEDING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_bleeding.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.BROKENBONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_brokenbone.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.BURNING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_hot.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.COLD,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_down.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.COVERED,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_shelter.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.DEADZONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_deadzone.png",
                        GroupIndex = 2
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.FREEZING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_cold.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.HANDCUFFED,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_handcuffed.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NOFOOD,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_food.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NOWATER,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_water.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NOOXYGEN,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_lunge.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.NOVIRUS,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_radiation.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.SAFEZONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_safezone.png",
                        GroupIndex = 2

                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.WARM,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_up.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerState.FULLMOON,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_fullmoon.png",
                        GroupIndex = -1
                    }
                }
            };
            HUDStyles = new List<HUDStyle>()
            {
                new HUDStyle()
                {
                    Enabled = true,
                    Name = "Square",
                    Aliases = new List<string> { "S" },
                    EffectID = 8807
                },
                new HUDStyle()
                {
                    Enabled = true,
                    Name = "Circular",
                    Aliases = new List<string> { "C" },
                    EffectID = 8808
                },
                new HUDStyle()
                {
                    Enabled = true,
                    Name = "Square Single Health",
                    Aliases = new List<string> { "SSH" },
                    EffectID = 8809
                },
                new HUDStyle()
                {
                    Enabled = true,
                    Name = "Circular Single Health",
                    Aliases = new List<string> { "CSH" },
                    EffectID = 8810
                },
                new HUDStyle()
                {
                    Enabled = true,
                    Name = "Tarkov Like",
                    Aliases = new List<string> { "TL" },
                    EffectID = 8811
                },
            };
            Medicines = new List<Medicine>
            {
                new Medicine { ItemID = 15, HealsHeadHP = 26.25f, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 0, HealsRightArmHP = 0, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { ItemID = 403, HealsHeadHP = 0, HealsBodyHP = 52.5f, HealsLeftArmHP = 0, HealsLeftLegHP = 0, HealsRightArmHP = 0, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { ItemID = 96, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 45, HealsRightArmHP = 0, HealsRightLegHP = 45, CuresPain = false },
                new Medicine { ItemID = 95, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 32.5f, HealsLeftLegHP = 0, HealsRightArmHP = 32.5f, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { ItemID = 394, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 45, HealsLeftLegHP = 0, HealsRightArmHP = 45, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { ItemID = 390, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 30, HealsRightArmHP = 0, HealsRightLegHP = 30, CuresPain = true }
            };
            OneHandedItems = new RestrictedItems
            {
                 ItemTypes = new List<EItemType>
                 {
                      EItemType.BACKPACK, EItemType.FOOD, EItemType.MEDICAL, EItemType.WATER, EItemType.VEHICLE_REPAIR_TOOL, EItemType.FILTER, EItemType.GLASSES, EItemType.HAT, EItemType.FISHER,
                      EItemType.MASK, EItemType.OPTIC, EItemType.PANTS, EItemType.SHIRT, EItemType.TOOL, EItemType.VEST
                 },
                 ItemID = new List<ushort>
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
                ItemID = new List<ushort>
                {
                    519
                }
            };
        }
    }
}
