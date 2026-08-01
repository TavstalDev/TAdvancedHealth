using Rocket.Unturned.Player;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    public static class EffectHelper
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        public static void SendUIEffectProgressBar(short key, CSteamID steamID, bool reliable, EProgressBar type, int percent, int lastPercent)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(steamID);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                
                var transCon = player.SteamPlayer().transportConnection;
                string childName;

                bool isLimb = false;
                List<string> progressBars;
                switch (type)
                {
                    case EProgressBar.SimpleHealth:
                    {
                        childName = "Health_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleSimpleHealth;
                        break;
                    }
                    case EProgressBar.Food:
                    {
                        childName = "Food_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleFood;
                        break;
                    }
                    case EProgressBar.Water:
                    {
                        childName = "Water_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleWater;
                        break;
                    }
                    case EProgressBar.Radiation:
                    {
                        childName = "Radiation_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleVirus;
                        break;
                    }
                    case EProgressBar.Oxygen:
                    {
                        childName = "Oxygen_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleOxygen;
                        break;
                    }
                    case EProgressBar.Stamina:
                    {
                        childName = "Stamina_PB_Value#{index}";
                        progressBars = comp.ProgressBarData.VisibleStamina;
                        break;
                    }
                    case EProgressBar.HeadHealth:
                    {
                        childName = "Head_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleHead;
                        break;
                    }
                    case EProgressBar.BodyHealth:
                    {
                        childName = "Body_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleBody;
                        break;
                    }
                    case EProgressBar.LeftArmHealth:
                    {
                        childName = "LeftArm_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleLeftArm;
                        break;
                    }
                    case EProgressBar.LeftLegHealth:
                    {
                        childName = "LeftLeg_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleLeftLeg;
                        break;
                    }
                    case EProgressBar.RightArmHealth:
                    {
                        childName = "RightArm_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleRightArm;
                        break;
                    }
                    case EProgressBar.RightLegHealth:
                    {
                        childName = "RightLeg_PB_Value#{index}";
                        isLimb = true;
                        progressBars = comp.ProgressBarData.VisibleRightLeg;
                        break;
                    }
                    default:
                        throw new Exception("How did we get here ?");
                }

                if (progressBars.Count > 0)
                {
                    foreach (string s in progressBars)
                        EffectManager.sendUIEffectVisibility(key, transCon, true, s, false);
                    progressBars.Clear();
                }
                
                if (isLimb)
                {
                    if (percent >= 75)
                    {
                        progressBars.Add(childName.Replace("{index}", "75"));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", "75"), true);
                    }
                    else if (percent >= 50)
                    {
                        progressBars.Add(childName.Replace("{index}", "50"));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", "50"), true);
                    }
                    else if (percent >= 25)
                    {
                        progressBars.Add(childName.Replace("{index}", "25"));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", "25"), true);
                    }
                    else
                    {
                        progressBars.Add(childName.Replace("{index}", "0"));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", "0"), true);
                    }
                    return;
                }

                string currentPercentUiName = childName.Replace("{index}", percent.ToString());
                progressBars.Add(currentPercentUiName);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastPercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, true, currentPercentUiName, true);
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                var transCon = player.SteamPlayer().transportConnection;

                //Base
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Health", Math.Round(healthData.BaseHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.SimpleHealth, (int)((Math.Round(healthData.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), (int)((comp.ProgressBarData.LastSimpleHealth / _config.HealthSystemSettings.BaseHealth) * 100));
                //Head
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Head", Math.Round(healthData.HeadHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.HeadHealth, (int)((Math.Round(healthData.HeadHealth, 2) / _config.HealthSystemSettings.HeadHealth) * 100), (int)((comp.ProgressBarData.LastHealthHead / _config.HealthSystemSettings.HeadHealth) * 100));
                //Body
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_Body", Math.Round(healthData.BodyHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.BodyHealth, (int)((Math.Round(healthData.BodyHealth, 2) / _config.HealthSystemSettings.BodyHealth) * 100), (int)((comp.ProgressBarData.LastHealthBody / _config.HealthSystemSettings.BodyHealth) * 100));
                //LeftArm
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_LeftArm", Math.Round(healthData.LeftArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.LeftArmHealth, (int)((Math.Round(healthData.LeftArmHealth, 2) / _config.HealthSystemSettings.LeftArmHealth) * 100), (int)((comp.ProgressBarData.LastHealthLeftArm / _config.HealthSystemSettings.LeftArmHealth) * 100));
                //LeftLeg
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_LeftLeg", Math.Round(healthData.LeftLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.LeftLegHealth, (int)((Math.Round(healthData.LeftLegHealth, 2) / _config.HealthSystemSettings.LeftLegHealth) * 100), (int)((comp.ProgressBarData.LastHealthLeftLeg / _config.HealthSystemSettings.LeftLegHealth) * 100));
                //RightArm
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_RightArm", Math.Round(healthData.RightArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.RightArmHealth, (int)((Math.Round(healthData.RightArmHealth, 2) / _config.HealthSystemSettings.RightArmHealth) * 100), (int)((comp.ProgressBarData.LastHealthRightArm / _config.HealthSystemSettings.RightArmHealth) * 100));
                //RightLeg
                EffectManager.sendUIEffectText((short)_config.EffectId, transCon, true, "tb_RightLeg", Math.Round(healthData.RightLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.RightLegHealth, (int)((Math.Round(healthData.RightLegHealth, 2) / _config.HealthSystemSettings.RightLegHealth) * 100), (int)((comp.ProgressBarData.LastHealthRightLeg / _config.HealthSystemSettings.RightLegHealth) * 100));
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
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;

                var transCon = player.SteamPlayer().transportConnection;
                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, "LimbHealth", _config.HealthSystemSettings.EnableLimbHealthSystem);
                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, "BaseHealth", !_config.HealthSystemSettings.EnableLimbHealthSystem);
                
                UpdateHealthUI(player);
                comp.ProgressBarData.LastHealthHead = healthData.HeadHealth;
                comp.ProgressBarData.LastHealthBody = healthData.BodyHealth;
                comp.ProgressBarData.LastHealthLeftArm = healthData.LeftArmHealth;
                comp.ProgressBarData.LastHealthLeftLeg = healthData.LeftLegHealth;
                comp.ProgressBarData.LastHealthRightArm = healthData.RightArmHealth;
                comp.ProgressBarData.LastHealthRightLeg = healthData.RightLegHealth;
                //Stats
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, player.Player.life.food);
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, player.Player.life.stamina);
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, player.Player.life.water);
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, player.Player.life.virus);
                SendUIEffectProgressBar((short)_config.EffectId, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, player.Player.life.oxygen);

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
