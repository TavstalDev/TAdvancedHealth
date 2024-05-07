using System.Collections.Generic;

namespace Tavstal.TAdvancedHealth.Models.Config
{
    public class HealthSystemSettings
    {
        public ushort PainEffectID { get; set; }
        public bool EnableTarkovLikeHealth { get; set; }
        public float BaseHealth { get; set; }
        private float _headHealth { get; set; }
        private float _bodyHealth { get; set; }
        private float _rightArmHealth { get; set; }
        private float _rightLegHealth { get; set; }
        private float _leftArmHealth { get; set; }
        private float _leftLegHealth { get; set; }
        public float HeadHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _headHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _headHealth = value;
            }
        }
        public float BodyHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _bodyHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _bodyHealth = value;
            }
        }
        public float RightArmHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _rightArmHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _rightArmHealth = value;
            }
        }
        public float RightLegHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _rightLegHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _rightLegHealth = value;
            }
        }
        public float LeftArmHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _leftArmHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _leftArmHealth = value;
            }
        }
        public float LeftLegHealth
        {
            get
            {
                if (EnableTarkovLikeHealth)
                    return _leftLegHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (EnableTarkovLikeHealth)
                    _leftLegHealth = value;
            }
        }
        public float DefaultWalkSpeed { get; set; }
        public float WalkSpeedWithOneBrokenLeg { get; set; }
        public float WalkSpeedWithBrokenLegs { get; set; }
        public bool DieWhenArmsHealthIsZero { get; set; }
        public bool DieWhenLegsHealthIsZero { get; set; }
        public bool DieWhenBodyHealthIsZero { get; set; }
        public bool DieWhenHeadHealthIsZero { get; set; }
        public bool CanDriveWithOneBrokenLeg { get; set; }
        public bool CanDriveWithBrokenLegs { get; set; }
        public bool CanDriveWithOneBrokenArm { get; set; }
        public bool CanDriveWithBrokenArms { get; set; }
        public bool CanHoldOneHandItemsWithOneBrokenArm { get; set; }
        public bool CanHoldTwoHandItemsWithOneBrokenArm { get; set; }
        public bool CanHoldOneHandItemsWithBrokenArms { get; set; }
        public bool CanHoldTwoHandItemsWithBrokenArms { get; set; }
        public bool CanJumpWithOneBrokenLeg { get; set; }
        public bool CanJumpWithBrokenLegs { get; set; }
        public bool CanStartBleeding { get; set; }
        public float HeavyBleedingChance { get; set; }
        public float BleedingDamage { get; set; }
        public float HeavyBleedingDamage { get; set; }
        public bool CanWalkWithOneBrokenLeg { get; set; }
        public bool CanWalkWithBrokenLegs { get; set; }
        public bool CanBeInjured { get; set; }
        public double InjuredDeathTimeSecs { get; set; }
        public float HealthRegenMinFood { get; set; }
        public float HealthRegenMinWater { get; set; }
        public float HealthRegenMinVirus { get; set; }
        public bool CanHavePainEffect { get; set; }
        public float PainEffectChance { get; set; }
        public float PainEffectDuration { get; set; }
        public float InjuredChance { get; set; }
        public float LegRegenTicks { get; set; }
        public float ArmRegenTicks { get; set; }
        public float BodyRegenTicks { get; set; }
        public float HeadRegenTicks { get; set; }
        public List<StatusIcon> StatusIcons { get; set; }

        public HealthSystemSettings() { }

        public HealthSystemSettings(ushort painEffectID, bool enableTarkovLikeHealth, float baseHealth, float headHealth, float bodyHealth, float rightArmHealth, float rightLegHealth, float leftArmHealth, float leftLegHealth, float defaultWalkSpeed, float walkSpeedWithOneBrokenLeg, float walkSpeedWithBrokenLegs, bool dieWhenArmsHealthIsZero, bool dieWhenLegsHealthIsZero, bool dieWhenBodyHealthIsZero, bool dieWhenHeadHealthIsZero, bool canDriveWithOneBrokenLeg, bool canDriveWithBrokenLegs, bool canDriveWithOneBrokenArm, bool canDriveWithBrokenArms, bool canHoldOneHandItemsWithOneBrokenArm, bool canHoldTwoHandItemsWithOneBrokenArm, bool canHoldOneHandItemsWithBrokenArms, bool canHoldTwoHandItemsWithBrokenArms, bool canJumpWithOneBrokenLeg, bool canJumpWithBrokenLegs, bool canStartBleeding, float heavyBleedingChance, float bleedingDamage, float heavyBleedingDamage, bool canWalkWithOneBrokenLeg, bool canWalkWithBrokenLegs, bool canBeInjured, double injuredDeathTimeSecs, float healthRegenMinFood, float healthRegenMinWater, float healthRegenMinVirus, bool canHavePainEffect, float painEffectChance, float painEffectDuration, float injuredChance, float legRegenTicks, float armRegenTicks, float bodyRegenTicks, float headRegenTicks, List<StatusIcon> statusIcons)
        {
            PainEffectID = painEffectID;
            EnableTarkovLikeHealth = enableTarkovLikeHealth;
            BaseHealth = baseHealth;
            _headHealth = headHealth;
            _bodyHealth = bodyHealth;
            _rightArmHealth = rightArmHealth;
            _rightLegHealth = rightLegHealth;
            _leftArmHealth = leftArmHealth;
            _leftLegHealth = leftLegHealth;
            DefaultWalkSpeed = defaultWalkSpeed;
            WalkSpeedWithOneBrokenLeg = walkSpeedWithOneBrokenLeg;
            WalkSpeedWithBrokenLegs = walkSpeedWithBrokenLegs;
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
            CanStartBleeding = canStartBleeding;
            HeavyBleedingChance = heavyBleedingChance;
            BleedingDamage = bleedingDamage;
            HeavyBleedingDamage = heavyBleedingDamage;
            CanWalkWithOneBrokenLeg = canWalkWithOneBrokenLeg;
            CanWalkWithBrokenLegs = canWalkWithBrokenLegs;
            CanBeInjured = canBeInjured;
            InjuredDeathTimeSecs = injuredDeathTimeSecs;
            HealthRegenMinFood = healthRegenMinFood;
            HealthRegenMinWater = healthRegenMinWater;
            HealthRegenMinVirus = healthRegenMinVirus;
            CanHavePainEffect = canHavePainEffect;
            PainEffectChance = painEffectChance;
            PainEffectDuration = painEffectDuration;
            InjuredChance = injuredChance;
            LegRegenTicks = legRegenTicks;
            ArmRegenTicks = armRegenTicks;
            BodyRegenTicks = bodyRegenTicks;
            HeadRegenTicks = headRegenTicks;
            StatusIcons = statusIcons;
        }
    }
}
