using Rocket.API;
using System.Collections.Generic;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Compatibility;

namespace Tavstal.TAdvancedHealth
{
    public class TAdvancedHealthConfig : IRocketPluginConfiguration
    {
        public DatabaseData databaseData { get; set; }
        public string MessageIcon { get; set; }
        public DefibrillatorSettings DefibrillatorSettings { get; set; }
        public HospitalSettings HospitalSettings { get; set; }
        public AntiGroupFriendlyFireSettings AntiGroupFriendlyFireSettings { get; set; }
        public CustomHealtSystemAndComponentSettings CustomHealtSystemAndComponentSettings { get; set; }
        public List<HUDStyle> HUDStyles { get; set; }
        public List<Medicine> Medicines { get; set; }
        public OneHandedItem oneHandedItems { get; set; }
        public TwoHandedItem twoHandedItems { get; set; }

        public void LoadDefaults()
        {
            databaseData = new DatabaseData()
            {
                DatabaseAddress = "127.0.0.1",
                DatabasePort = 3306,
                DatabaseUser = "root",
                DatabasePassword = "ascent",
                DatabaseName = "unturned",
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
                hospitals = new List<Hospital>
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
                groups = new List<string> { "police", "swat" }
            };
            CustomHealtSystemAndComponentSettings = new CustomHealtSystemAndComponentSettings
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
                statusIcons = new List<StatusIcon>
                {
                    new StatusIcon
                    {
                        Status = EPlayerStates.ACID,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_acid.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.BLEEDING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_bleeding.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.BROKENBONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_brokenbone.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.BURNING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_hot.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.COLD,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_down.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.COVERED,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_shelter.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.DEADZONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_deadzone.png",
                        GroupIndex = 2
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.FREEZING,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_cold.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.HANDCUFFED,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_handcuffed.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.NOFOOD,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_food.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.NOWATER,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_water.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.NOOXYGEN,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_lunge.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.NOVIRUS,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_radiation.png",
                        GroupIndex = -1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.SAFEZONE,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_safezone.png",
                        GroupIndex = 2

                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.WARM,
                        IconUrl = "https://raw.githubusercontent.com/TavstalDev/Icons/master/TBetterHealthSystem/icon_temperature_up.png",
                        GroupIndex = 1
                    },
                    new StatusIcon
                    {
                        Status = EPlayerStates.FULLMOON,
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
                new Medicine { itemID = 15, HealsHeadHP = 26.25f, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 0, HealsRightArmHP = 0, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { itemID = 403, HealsHeadHP = 0, HealsBodyHP = 52.5f, HealsLeftArmHP = 0, HealsLeftLegHP = 0, HealsRightArmHP = 0, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { itemID = 96, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 45, HealsRightArmHP = 0, HealsRightLegHP = 45, CuresPain = false },
                new Medicine { itemID = 95, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 32.5f, HealsLeftLegHP = 0, HealsRightArmHP = 32.5f, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { itemID = 394, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 45, HealsLeftLegHP = 0, HealsRightArmHP = 45, HealsRightLegHP = 0, CuresPain = false },
                new Medicine { itemID = 390, HealsHeadHP = 0, HealsBodyHP = 0, HealsLeftArmHP = 0, HealsLeftLegHP = 30, HealsRightArmHP = 0, HealsRightLegHP = 30, CuresPain = true }
            };
            oneHandedItems = new OneHandedItem
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
            twoHandedItems = new TwoHandedItem
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
