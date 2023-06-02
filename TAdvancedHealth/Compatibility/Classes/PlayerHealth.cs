using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Managers;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    [SqlName("")]
    public class PlayerHealth
    {
        public string PlayerId { get; set; }
        public float BaseHealth { get; set; }
        private float _headHealth { get; set; }
        private float _bodyHealth { get; set; }
        private float _rightArmHealth { get; set; }
        private float _leftArmHealth { get; set; }
        private float _rightLegHealth { get; set; }
        private float _leftLegHealth { get; set; }
        public float HeadHealth
        {
            get
            {
                
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _headHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _headHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.HeadHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public float BodyHealth
        {
            get
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _bodyHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _bodyHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BodyHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public float RightArmHealth
        {
            get
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _rightArmHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _rightArmHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.RightArmHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public float LeftArmHealth
        {
            get
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _leftArmHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _leftArmHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.LeftArmHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public float RightLegHealth
        {
            get
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _rightLegHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _rightLegHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.RightLegHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public float LeftLegHealth
        {
            get
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                    return _leftLegHealth;
                else
                    return BaseHealth;
            }
            set
            {
                if (TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.EnableTarkovLikeHealth)
                {
                    _leftLegHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.LeftLegHealth);
                }
                else
                {
                    BaseHealth = MathHelper.Clamp(value, 0, TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings.BaseHealth);
                }
            }
        }
        public bool isInjured { get; set; }
        public bool isHUDEnabled { get; set; }
        public ushort HUDEffectID { get; set; }
        public DateTime dieDate { get; set; }

        public PlayerHealth() { }
    }
}
