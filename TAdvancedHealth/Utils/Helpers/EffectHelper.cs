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
                switch (type)
                {
                    case EProgressBar.SimpleHealth:
                    {
                        childName = "Health_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleSimpleHealth.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleSimpleHealth)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleSimpleHealth = new List<string>();
                        }

                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", lastPercent.ToString()), false);
                        comp.ProgressBarData.VisibleSimpleHealth.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.Food:
                    {
                        childName = "Food_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleFood.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleFood)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleFood = new List<string>();
                        }

                        comp.ProgressBarData.VisibleFood.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.Water:
                    {
                        childName = "Water_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleWater.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleWater)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleWater = new List<string>();
                        }

                        comp.ProgressBarData.VisibleWater.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.Radiation:
                    {
                        childName = "Radiation_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleVirus.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleVirus)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleVirus = new List<string>();
                        }

                        comp.ProgressBarData.VisibleVirus.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.Oxygen:
                    {
                        childName = "Oxygen_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleOxygen.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleOxygen)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleOxygen = new List<string>();
                        }

                        comp.ProgressBarData.VisibleOxygen.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.Stamina:
                    {
                        childName = "Stamina_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleStamina.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleStamina)
                            {
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            }

                            comp.ProgressBarData.VisibleStamina = new List<string>();
                        }

                        comp.ProgressBarData.VisibleStamina.Add(childName.Replace("{index}", percent.ToString()));
                        EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                            childName.Replace("{index}", percent.ToString()), true);

                        break;
                    }
                    case EProgressBar.HeadHealth:
                    {
                        childName = "Head_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleHead.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleHead)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleHead.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleHead.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleHead.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleHead.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleHead.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    case EProgressBar.BodyHealth:
                    {
                        childName = "Body_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleBody.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleBody)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleBody.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleBody.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleBody.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleBody.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleBody.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    case EProgressBar.LeftArmHealth:
                    {
                        childName = "LeftArm_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleLeftArm.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleLeftArm)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleLeftArm.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleLeftArm.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleLeftArm.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleLeftArm.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleLeftArm.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    case EProgressBar.LeftLegHealth:
                    {
                        childName = "LeftLeg_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleLeftLeg.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleLeftLeg)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleLeftLeg.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    case EProgressBar.RightArmHealth:
                    {
                        childName = "RightArm_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleRightArm.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleRightArm)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleRightArm.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleRightArm.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleRightArm.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleRightArm.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleRightArm.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    case EProgressBar.RightLegHealth:
                    {
                        childName = "RightLeg_PB_Value#{index}";
                        if (comp.ProgressBarData.VisibleRightLeg.Count > 0)
                        {
                            foreach (string s in comp.ProgressBarData.VisibleRightLeg)
                                EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true, s, false);
                            comp.ProgressBarData.VisibleRightLeg.Clear();
                        }
                        
                        if (percent >= 75)
                        {
                            comp.ProgressBarData.VisibleRightLeg.Add(childName.Replace("{index}", "75"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "75"), true);
                        }
                        else if (percent >= 50)
                        {
                            comp.ProgressBarData.VisibleRightLeg.Add(childName.Replace("{index}", "50"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "50"), true);
                        }
                        else if (percent >= 25)
                        {
                            comp.ProgressBarData.VisibleRightLeg.Add(childName.Replace("{index}", "25"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "25"), true);
                        }
                        else
                        {
                            comp.ProgressBarData.VisibleRightLeg.Add(childName.Replace("{index}", "0"));
                            EffectManager.sendUIEffectVisibility((short)_config.EffectId, transCon, true,
                                childName.Replace("{index}", "0"), true);
                        }
                        break;
                    }
                    default:
                        throw new Exception("How did we get here ?");
                }

                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastPercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", percent.ToString()), true);
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
