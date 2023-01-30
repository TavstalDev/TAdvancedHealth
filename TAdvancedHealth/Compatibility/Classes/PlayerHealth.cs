using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
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
                    _headHealth = value;
                else
                    BaseHealth = value;
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
                    _bodyHealth = value;
                else
                    BaseHealth = value;
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
                    _rightArmHealth = value;
                else
                    BaseHealth = value;
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
                    _leftArmHealth = value;
                else
                    BaseHealth = value;
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
                    _rightLegHealth = value;
                else
                    BaseHealth = value;
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
                    _leftLegHealth = value;
                else
                    BaseHealth = value;
            }
        }
        public bool isInjured { get; set; }
        public bool isHUDEnabled { get; set; }
        public ushort HUDEffectID { get; set; }
        public DateTime dieDate { get; set; }

        public PlayerHealth() { }
    }
}
