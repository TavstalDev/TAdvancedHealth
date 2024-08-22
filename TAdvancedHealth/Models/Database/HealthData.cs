using System;
using Tavstal.TLibrary.Models.Database.Attributes;
using Tavstal.TLibrary.Helpers.General;

namespace Tavstal.TAdvancedHealth.Models.Database
{
    public class HealthData
    {
        [SqlMember(isPrimaryKey: true, isNullable: false)]
        public string PlayerId { get; set; }
        [SqlMember(isNullable: true)]
        public float BaseHealth { get; set; }
        [SqlMember]
        public float HeadHealth { get; set; }
        [SqlMember]
        public float BodyHealth { get; set; }
        [SqlMember]
        public float RightArmHealth { get; set; }
        [SqlMember]
        public float LeftArmHealth { get; set; }
        [SqlMember]
        public float RightLegHealth { get; set; }
        [SqlMember]
        public float LeftLegHealth { get; set; }
        [SqlMember]
        public bool IsInjured { get; set; }
        [SqlMember]
        public bool IsHUDEnabled { get; set; }
        [SqlMember]
        public ushort HUDEffectID { get; set; }
        [SqlMember]
        public DateTime DeathDate { get; set; }

        public HealthData() { }

        public HealthData(string playerId, float baseHealth, float headHealth, float bodyHealth, float rightArmHealth, float leftArmHealth, float rightLegHealth, float leftLegHealth, bool isInjured, bool isHUDEnabled, ushort hUDEffectID, DateTime deathDate)
        {
            PlayerId = playerId;
            BaseHealth = baseHealth;
            HeadHealth = headHealth;
            BodyHealth = bodyHealth;
            RightArmHealth = rightArmHealth;
            LeftArmHealth = leftArmHealth;
            RightLegHealth = rightLegHealth;
            LeftLegHealth = leftLegHealth;
            IsInjured = isInjured;
            IsHUDEnabled = isHUDEnabled;
            HUDEffectID = hUDEffectID;
            DeathDate = deathDate;
        }

        public void EnsureValueCap()
        {
            if (TAdvancedHealth.Instance.Config.HealthSystemSettings.EnableTarkovLikeHealth)
            {
                HeadHealth = MathHelper.Clamp(HeadHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth);
                BodyHealth = MathHelper.Clamp(BodyHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth);
                RightArmHealth = MathHelper.Clamp(RightArmHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth);
                LeftArmHealth = MathHelper.Clamp(LeftArmHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.LeftArmHealth);
                RightLegHealth = MathHelper.Clamp(RightLegHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth);
                LeftLegHealth = MathHelper.Clamp(LeftLegHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.LeftLegHealth);
            }
            else
            {
                BaseHealth = MathHelper.Clamp(BaseHealth, 0, TAdvancedHealth.Instance.Config.HealthSystemSettings.BaseHealth);
            }
        }
    }
}
