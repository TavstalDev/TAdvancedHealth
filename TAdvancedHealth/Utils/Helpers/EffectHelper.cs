using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    public static class EffectHelper
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        /// <summary>
        /// Sends a UI effect with a progress bar to a specific player.
        /// </summary>
        /// <param name="key">The key of the UI effect.</param>
        /// <param name="steamID">The SteamID of the player to receive the UI effect.</param>
        /// <param name="reliable">Whether the network transmission should be reliable.</param>
        /// <param name="type">The type of progress bar.</param>
        /// <param name="percent">The current percentage value of the progress bar.</param>
        /// <param name="lastPercent">The previous percentage value of the progress bar.</param>
        public static async Task SendUIEffectProgressBarAsync(short key, CSteamID steamID, bool reliable, EProgressBar type, int percent, int lastPercent)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromCSteamID(steamID);
                HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
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
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleSimpleHealth = new List<string>();
                            }

                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", lastPercent.ToString()), false);
                            comp.ProgressBarData.VisibleSimpleHealth.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Food:
                        {
                            childName = "Food_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleFood.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleFood)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleFood = new List<string>();
                            }

                            comp.ProgressBarData.VisibleFood.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Water:
                        {
                            childName = "Water_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleWater.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleWater)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleWater = new List<string>();
                            }

                            comp.ProgressBarData.VisibleWater.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Radiation:
                        {
                            childName = "Radiation_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleVirus.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleVirus)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleVirus = new List<string>();
                            }

                            comp.ProgressBarData.VisibleVirus.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Oxygen:
                        {
                            childName = "Oxygen_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleOxygen.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleOxygen)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleOxygen = new List<string>();
                            }

                            comp.ProgressBarData.VisibleOxygen.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Stamina:
                        {
                            childName = "Stamina_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleStamina.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleStamina)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleStamina = new List<string>();
                            }

                            comp.ProgressBarData.VisibleStamina.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.HeadHealth:
                        {
                            childName = "Head_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleHead.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleHead)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleHead = new List<string>();
                            }

                            comp.ProgressBarData.VisibleHead.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.BodyHealth:
                        {
                            childName = "Body_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleBody.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleBody)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleBody = new List<string>();
                            }

                            comp.ProgressBarData.VisibleBody.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.LeftArmHealth:
                        {
                            childName = "LeftArm_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleLeftArm.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleLeftArm)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleLeftArm = new List<string>();
                            }

                            comp.ProgressBarData.VisibleLeftArm.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.LeftLegHealth:
                        {
                            childName = "LeftLeg_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleLeftLeg.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleLeftLeg)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleLeftLeg = new List<string>();
                            }

                            comp.ProgressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.RightArmHealth:
                        {
                            childName = "RightArm_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleRightArm.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleRightArm)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleRightArm = new List<string>();
                            }

                            comp.ProgressBarData.VisibleRightArm.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.RightLegHealth:
                        {
                            childName = "RightLeg_PB_Value#{index}";
                            if (comp.ProgressBarData.VisibleRightLeg.Count > 0)
                            {
                                foreach (string s in comp.ProgressBarData.VisibleRightLeg)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.ProgressBarData.VisibleRightLeg = new List<string>();
                            }

                            comp.ProgressBarData.VisibleRightLeg.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    default:
                        throw new Exception("How did we get here ?");
                }

                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastPercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", percent.ToString()), true);
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError("ProgressBar Error");
                AdvancedHealth.Logger.LogError(e);
            }
        }

        /// <summary>
        /// Asynchronously updates the health user interface (UI) for the specified Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose health UI is to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        private static async Task UpdateHealthUIAsync(UnturnedPlayer player)
        {
            string voidname = "UpdateHealthUI";
            try
            {
                HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);
                var transCon = player.SteamPlayer().transportConnection;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                //Base
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_Health", Math.Round(health.BaseHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.SimpleHealth, (int)((Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), (int)((comp.ProgressBarData.LastSimpleHealth / _config.HealthSystemSettings.BaseHealth) * 100));
                //Head
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_Head", Math.Round(health.HeadHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.HeadHealth, (int)((Math.Round(health.HeadHealth, 2) / _config.HealthSystemSettings.HeadHealth) * 100), (int)((comp.ProgressBarData.LastHealthHead / _config.HealthSystemSettings.HeadHealth) * 100));
                //Bpdy
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_Body", Math.Round(health.BodyHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.BodyHealth, (int)((Math.Round(health.BodyHealth, 2) / _config.HealthSystemSettings.BodyHealth) * 100), (int)((comp.ProgressBarData.LastHealthBody / _config.HealthSystemSettings.BodyHealth) * 100));
                //LeftArm
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_LeftArm", Math.Round(health.LeftArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftArmHealth, (int)((Math.Round(health.LeftArmHealth, 2) / _config.HealthSystemSettings.LeftArmHealth) * 100), (int)((comp.ProgressBarData.LastHealthLeftArm / _config.HealthSystemSettings.LeftArmHealth) * 100));
                //LeftLeg
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_LeftLeg", Math.Round(health.LeftLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftLegHealth, (int)((Math.Round(health.LeftLegHealth, 2) / _config.HealthSystemSettings.LeftLegHealth) * 100), (int)((comp.ProgressBarData.LastHealthLeftLeg / _config.HealthSystemSettings.LeftLegHealth) * 100));
                //RightArm
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_RightArm", Math.Round(health.RightArmHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightArmHealth, (int)((Math.Round(health.RightArmHealth, 2) / _config.HealthSystemSettings.RightArmHealth) * 100), (int)((comp.ProgressBarData.LastHealthRightArm / _config.HealthSystemSettings.RightArmHealth) * 100));
                //RightLeg
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_RightLeg", Math.Round(health.RightLegHealth, 2).ToString(CultureInfo.CurrentCulture));
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightLegHealth, (int)((Math.Round(health.RightLegHealth, 2) / _config.HealthSystemSettings.RightLegHealth) * 100), (int)((comp.ProgressBarData.LastHealthRightLeg / _config.HealthSystemSettings.RightLegHealth) * 100));
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.Log($"Error in {voidname}: {e}");
            }
        }

        /// <summary>
        /// Asynchronously updates the entire health user interface (UI) for the specified Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose entire health UI is to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task UpdateWholeHealthUIAsync(UnturnedPlayer player)
        {
            string voidname = "UpdateHealthUI";
            try
            {
                HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await UpdateHealthUIAsync(player);
                comp.ProgressBarData.LastHealthHead = health.HeadHealth;
                comp.ProgressBarData.LastHealthBody = health.BodyHealth;
                comp.ProgressBarData.LastHealthLeftArm = health.LeftArmHealth;
                comp.ProgressBarData.LastHealthLeftLeg = health.LeftLegHealth;
                comp.ProgressBarData.LastHealthRightArm = health.RightArmHealth;
                comp.ProgressBarData.LastHealthRightLeg = health.RightLegHealth;
                //Stats
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, player.Player.life.food);
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, player.Player.life.stamina);
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, player.Player.life.water);
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, player.Player.life.virus);
                await SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, player.Player.life.oxygen);

            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.Log($"Error in {voidname}: {e}");
            }
        }
    }
}
