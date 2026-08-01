using Rocket.Unturned.Player;
using System;
using System.Globalization;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    public static class EffectHelper
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        public static void SendUIEffectProgressBar(UnturnedPlayer player, short key, bool reliable, EProgressBar type, int percent, int lastPercent)
        {
            if (percent == lastPercent)
                return;
            
            try
            {
                var comp = ComponentManager.Get(player);
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                
                var transCon = player.SteamPlayer().transportConnection;
                string childName;

                bool isLimb = false;
                ProgressbarValue progressbar;
                switch (type)
                {
                    case EProgressBar.SimpleHealth:
                    {
                        childName = "Health_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Health;
                        break;
                    }
                    case EProgressBar.Food:
                    {
                        childName = "Food_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Food;
                        break;
                    }
                    case EProgressBar.Water:
                    {
                        childName = "Water_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Water;
                        break;
                    }
                    case EProgressBar.Radiation:
                    {
                        childName = "Radiation_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Virus;
                        break;
                    }
                    case EProgressBar.Oxygen:
                    {
                        childName = "Oxygen_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Oxygen;
                        break;
                    }
                    case EProgressBar.Stamina:
                    {
                        childName = "Stamina_PB_Value#{index}";
                        progressbar = comp.ProgressbarData.Stamina;
                        break;
                    }
                    case EProgressBar.HeadHealth:
                    {
                        childName = "Head_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.Head;
                        break;
                    }
                    case EProgressBar.BodyHealth:
                    {
                        childName = "Body_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.Body;
                        break;
                    }
                    case EProgressBar.LeftArmHealth:
                    {
                        childName = "LeftArm_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.LeftArm;
                        break;
                    }
                    case EProgressBar.LeftLegHealth:
                    {
                        childName = "LeftLeg_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.LeftLeg;
                        break;
                    }
                    case EProgressBar.RightArmHealth:
                    {
                        childName = "RightArm_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.RightArm;
                        break;
                    }
                    case EProgressBar.RightLegHealth:
                    {
                        childName = "RightLeg_PB_Value#{index}";
                        isLimb = true;
                        progressbar = comp.ProgressbarData.RightLeg;
                        break;
                    }
                    default:
                        throw new Exception("How did we get here ?");
                }

                if (!string.IsNullOrEmpty(progressbar.InterfaceValue))
                {
                    EffectManager.sendUIEffectVisibility(key, transCon, reliable, progressbar.InterfaceValue, false);
                    progressbar.InterfaceValue = null;
                }
                
                if (isLimb)
                {
                    if (percent >= 75)
                    {
                        progressbar.InterfaceValue = childName.Replace("{index}", "75");
                        EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", "75"), true);
                    }
                    else if (percent >= 50)
                    {
                        progressbar.InterfaceValue = childName.Replace("{index}", "50");
                        EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", "50"), true);
                    }
                    else if (percent >= 25)
                    {
                        progressbar.InterfaceValue = childName.Replace("{index}", "25");
                        EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", "25"), true);
                    }
                    else
                    {
                        progressbar.InterfaceValue = childName.Replace("{index}", "0");
                        EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", "0"), true);
                    }
                    return;
                }

                string currentPercentUiName = childName.Replace("{index}", percent.ToString());
                progressbar.InterfaceValue = currentPercentUiName;
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastPercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, currentPercentUiName, true);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(SendUIEffectProgressBar)}.", ex);
            }
        }
        
        private static void UpdateHealthUI(UnturnedPlayer player)
        {
            try
            {
                AdvancedHealthComponent comp = ComponentManager.Get(player);
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                var transCon = player.SteamPlayer().transportConnection;

                //Base
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Health", Math.Round(healthData.BaseHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.SimpleHealth, (int)((Math.Round(healthData.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), (int)((comp.ProgressbarData.Health.Value / _config.HealthSystemSettings.BaseHealth) * 100));
                //Head
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Head", Math.Round(healthData.HeadHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.HeadHealth, (int)((Math.Round(healthData.HeadHealth, 2) / _config.HealthSystemSettings.HeadHealth) * 100), (int)((comp.ProgressbarData.Head.Value / _config.HealthSystemSettings.HeadHealth) * 100));
                //Body
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Body", Math.Round(healthData.BodyHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.BodyHealth, (int)((Math.Round(healthData.BodyHealth, 2) / _config.HealthSystemSettings.BodyHealth) * 100), (int)((comp.ProgressbarData.Body.Value / _config.HealthSystemSettings.BodyHealth) * 100));
                //LeftArm
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_LeftArm", Math.Round(healthData.LeftArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.LeftArmHealth, (int)((Math.Round(healthData.LeftArmHealth, 2) / _config.HealthSystemSettings.LeftArmHealth) * 100), (int)((comp.ProgressbarData.LeftArm.Value / _config.HealthSystemSettings.LeftArmHealth) * 100));
                //LeftLeg
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_LeftLeg", Math.Round(healthData.LeftLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.LeftLegHealth, (int)((Math.Round(healthData.LeftLegHealth, 2) / _config.HealthSystemSettings.LeftLegHealth) * 100), (int)((comp.ProgressbarData.LeftLeg.Value / _config.HealthSystemSettings.LeftLegHealth) * 100));
                //RightArm
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_RightArm", Math.Round(healthData.RightArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.RightArmHealth, (int)((Math.Round(healthData.RightArmHealth, 2) / _config.HealthSystemSettings.RightArmHealth) * 100), (int)((comp.ProgressbarData.RightArm.Value / _config.HealthSystemSettings.RightArmHealth) * 100));
                //RightLeg
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_RightLeg", Math.Round(healthData.RightLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.RightLegHealth, (int)((Math.Round(healthData.RightLegHealth, 2) / _config.HealthSystemSettings.RightLegHealth) * 100), (int)((comp.ProgressbarData.RightLeg.Value / _config.HealthSystemSettings.RightLegHealth) * 100));
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(UpdateHealthUI)}.", ex);
            }
        }
        
        public static void UpdateWholeHealthUI(UnturnedPlayer player)
        {
            try
            {
                AdvancedHealthComponent comp = ComponentManager.Get(player);
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;

                var transCon = player.SteamPlayer().transportConnection;
                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, "LimbHealth", _config.HealthSystemSettings.EnableLimbHealthSystem);
                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, "BaseHealth", !_config.HealthSystemSettings.EnableLimbHealthSystem);
                
                UpdateHealthUI(player);
                comp.ProgressbarData.Health.Value = healthData.BaseHealth;
                comp.ProgressbarData.Head.Value = healthData.HeadHealth;
                comp.ProgressbarData.Body.Value = healthData.BodyHealth;
                comp.ProgressbarData.LeftArm.Value = healthData.LeftArmHealth;
                comp.ProgressbarData.LeftLeg.Value = healthData.LeftLegHealth;
                comp.ProgressbarData.RightArm.Value = healthData.RightArmHealth;
                comp.ProgressbarData.RightLeg.Value = healthData.RightLegHealth;
                //Stats
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Food, player.Player.life.food, player.Player.life.food);
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Stamina, player.Player.life.stamina, player.Player.life.stamina);
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Water, player.Player.life.water, player.Player.life.water);
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Radiation, player.Player.life.virus, player.Player.life.virus);
                SendUIEffectProgressBar(player, (short)_config.EffectId, true, EProgressBar.Oxygen, player.Player.life.oxygen, player.Player.life.oxygen);

            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(UpdateWholeHealthUI)}.", ex);
            }
        }

        public static EPlayerState GetPlayerState(EPlayerTemperature temperature)
        {
            switch (temperature)
            {
                case EPlayerTemperature.FREEZING:
                    return EPlayerState.TEMPERATURE_FREEZING;
                case EPlayerTemperature.COLD:
                    return EPlayerState.TEMPERATURE_DOWN;
                case EPlayerTemperature.WARM:
                    return EPlayerState.TEMPERATURE_UP;
                case EPlayerTemperature.BURNING:
                    return EPlayerState.TEMPERATURE_BURNING;
                case EPlayerTemperature.NONE:
                    return EPlayerState.TEMPERATURE_NONE;
                case EPlayerTemperature.COVERED:
                    return EPlayerState.TEMPERATURE_COVER;
                case  EPlayerTemperature.ACID:
                    return EPlayerState.ACID;
                default:
                    throw new Exception("Unknown temperature type.");
            }
        }
    }
}
