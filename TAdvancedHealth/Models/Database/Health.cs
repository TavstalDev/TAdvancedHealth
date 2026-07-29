using System;
using Tavstal.TAdvancedHealth.Utils.Managers;

namespace Tavstal.TAdvancedHealth.Models.Database
{
    public class Health
    {
        public string PlayerId { get;  }
        
        public float BaseHealth { get; private set; } = 0.0f;

        public float HeadHealth { get; private set; } = 0.0f;

        public float BodyHealth { get; private set; } = 0.0f;

        public float RightArmHealth { get; private set; } = 0.0f;

        public float LeftArmHealth { get; private set; } = 0.0f;

        public float RightLegHealth { get; private set; } = 0.0f;

        public float LeftLegHealth { get; private set; } = 0.0f;

        public bool IsInjured { get; private set; } = false;

        public bool IsHUDEnabled { get; private set; } = false;

        public ushort HUDEffectID { get; private set; } = 0;

        public DateTime DeathDate { get; private set; } = DateTime.Now;

        public Health(string playerId)
        {
            PlayerId = playerId;
        }

        public Health(HealthData data)
        {
            PlayerId = data.PlayerId;
            BaseHealth = data.BaseHealth;
            HeadHealth = data.HeadHealth;
            BodyHealth = data.BodyHealth;
            RightArmHealth = data.RightArmHealth;
            LeftArmHealth = data.LeftArmHealth;
            RightLegHealth = data.RightLegHealth;
            LeftLegHealth = data.LeftLegHealth;
            IsInjured = data.IsInjured;
            DeathDate = data.DeathDate;
            IsHUDEnabled = data.IsHUDEnabled;
            HUDEffectID = data.HUDEffectID;
        }

        public void SetBaseHealth(float newValue)
        {
            BaseHealth = newValue;
            EventManager.FCallBaseHealthUpdated(PlayerId, newValue);
        }

        public void SetHeadHealth(float newValue)
        {
            HeadHealth = newValue;
            EventManager.FCallHeadHealthUpdated(PlayerId, newValue);
        }

        public void SetBodyHealth(float newValue)
        {
            BodyHealth = newValue;
            EventManager.FCallBodyHealthUpdated(PlayerId, newValue);
        }

        public void SetRightArmHealth(float newValue)
        {
            RightArmHealth = newValue;
            EventManager.FCallRightArmHealthUpdated(PlayerId, newValue);
        }

        public void SetLeftArmHealth(float newValue)
        {
            LeftArmHealth = newValue;
            EventManager.FCallLeftArmHealthUpdated(PlayerId, newValue);
        }

        public void SetRightLegHealth(float newValue)
        {
            RightLegHealth = newValue;
            EventManager.FCallRightLegHealthUpdated(PlayerId, newValue);
        }

        public void SetLeftLegHealth(float newValue)
        {
            LeftLegHealth = newValue;
            EventManager.FCallLeftLegHealthUpdated(PlayerId, newValue);
        }

        public void SetInjured(bool isInjured)
        {
            IsInjured = isInjured;
            DeathDate = DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.InjuredDeathTimeSecs);
            EventManager.FCallInjuredStateUpdated(PlayerId, isInjured, DeathDate);
        }

        public void SetHUDEnabled(bool isHUDEnabled, ushort hudId)
        {
            IsHUDEnabled = isHUDEnabled;
            HUDEffectID = hudId;
        }

        public HealthData ToHealthData() => new HealthData
        {
            PlayerId = this.PlayerId,
            BaseHealth = this.BaseHealth,
            HeadHealth = this.HeadHealth,
            BodyHealth = this.BodyHealth,
            RightArmHealth = this.RightArmHealth,
            LeftArmHealth = this.LeftArmHealth,
            RightLegHealth = this.RightLegHealth,
            LeftLegHealth = this.LeftLegHealth,
            IsInjured = this.IsInjured,
            DeathDate = this.DeathDate,
            HUDEffectID = this.HUDEffectID,
        };
    }
}