using System;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Utils.Managers;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Models
{
    public class Health
    {
        public string PlayerId { get;  }
        
        public float BaseHealth { get; private set; }

        public float HeadHealth { get; private set; }

        public float BodyHealth { get; private set; }

        public float RightArmHealth { get; private set; }

        public float LeftArmHealth { get; private set; }

        public float RightLegHealth { get; private set; }

        public float LeftLegHealth { get; private set; }

        public bool IsInjured { get; private set; }

        public bool IsHUDEnabled { get; private set; }

        public ushort HUDEffectID { get; private set; }

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

        public Health(string playerId, float baseHealth, float headHealth, float bodyHealth, float rightArmHealth, float leftArmHealth, float rightLegHealth, float leftLegHealth, bool isInjured, bool isHUDEnabled, ushort hudEffectID)
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
            HUDEffectID = hudEffectID;
        }

        public void SetBaseHealth(float newValue)
        {
            if (Mathf.Approximately(BaseHealth, newValue) && BaseHealth > 0.0f)
                return;
            BaseHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.BaseHealth);
            EventManager.FCallBaseHealthUpdated(PlayerId, newValue);
        }

        public void SetHeadHealth(float newValue)
        {
            if (Mathf.Approximately(HeadHealth, newValue) && HeadHealth > 0.0f)
                return;
            HeadHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth);
            EventManager.FCallHeadHealthUpdated(PlayerId, newValue);
        }

        public void SetBodyHealth(float newValue)
        {
            if (Mathf.Approximately(BodyHealth, newValue) && BodyHealth > 0.0f)
                return;
            BodyHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth);
            EventManager.FCallBodyHealthUpdated(PlayerId, newValue);
        }

        public void SetRightArmHealth(float newValue)
        {
            if (Mathf.Approximately(RightArmHealth, newValue) && RightArmHealth > 0.0f)
                return;
            RightArmHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth);
            EventManager.FCallRightArmHealthUpdated(PlayerId, newValue);
        }

        public void SetLeftArmHealth(float newValue)
        {
            if (Mathf.Approximately(LeftArmHealth, newValue) && LeftArmHealth > 0.0f)
                return;
            LeftArmHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.LeftArmHealth);
            EventManager.FCallLeftArmHealthUpdated(PlayerId, newValue);
        }

        public void SetRightLegHealth(float newValue)
        {
            if (Mathf.Approximately(RightLegHealth, newValue) && RightLegHealth > 0.0f)
                return;
            RightLegHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth);
            EventManager.FCallRightLegHealthUpdated(PlayerId, newValue);
        }

        public void SetLeftLegHealth(float newValue)
        {
            if (Mathf.Approximately(LeftLegHealth, newValue) && LeftLegHealth > 0.0f)
                return;
            LeftLegHealth = Mathf.Clamp(newValue, 0, AdvancedHealth.Instance.Config.HealthSystemSettings.LeftLegHealth);
            EventManager.FCallLeftLegHealthUpdated(PlayerId, newValue);
        }

        public void SetInjured(bool isInjured)
        {
            if (isInjured == IsInjured)
                return;
            IsInjured = isInjured;
            DeathDate = isInjured ? DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.Combat.InjuredDeathTimeSecs) : DateTime.Now;
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