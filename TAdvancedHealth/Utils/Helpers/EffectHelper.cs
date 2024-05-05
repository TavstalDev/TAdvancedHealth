using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enums;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    public static class EffectHelper
    {
        private static TAdvancedHealthConfig _config => TAdvancedHealth.Instance.Config;

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
                HealthData health = await TAdvancedHealth.Database.GetPlayerHealthAsync(player.Id);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var transCon = player.SteamPlayer().transportConnection;
                string childName = null;
                switch (type)
                {
                    case EProgressBar.Health_Simple:
                        {
                            childName = "Health_PB_Value#{index}";
                            if (comp.progressBarData.VisibleSimpleHealth.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleSimpleHealth)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleSimpleHealth = new List<string>();
                            }

                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", lastPercent.ToString()), false);
                            comp.progressBarData.VisibleSimpleHealth.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Food:
                        {
                            childName = "Food_PB_Value#{index}";
                            if (comp.progressBarData.VisibleFood.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleFood)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleFood = new List<string>();
                            }

                            comp.progressBarData.VisibleFood.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Water:
                        {
                            childName = "Water_PB_Value#{index}";
                            if (comp.progressBarData.VisibleWater.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleWater)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleWater = new List<string>();
                            }

                            comp.progressBarData.VisibleWater.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Radiation:
                        {
                            childName = "Radiation_PB_Value#{index}";
                            if (comp.progressBarData.VisibleVirus.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleVirus)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleVirus = new List<string>();
                            }

                            comp.progressBarData.VisibleVirus.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Oxygen:
                        {
                            childName = "Oxygen_PB_Value#{index}";
                            if (comp.progressBarData.VisibleOxygen.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleOxygen)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleOxygen = new List<string>();
                            }

                            comp.progressBarData.VisibleOxygen.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Stamina:
                        {
                            childName = "Stamina_PB_Value#{index}";
                            if (comp.progressBarData.VisibleStamina.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleStamina)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleStamina = new List<string>();
                            }

                            comp.progressBarData.VisibleStamina.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_Head:
                        {
                            childName = "Head_PB_Value#{index}";
                            if (comp.progressBarData.VisibleHead.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleHead)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleHead = new List<string>();
                            }

                            comp.progressBarData.VisibleHead.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_Body:
                        {
                            childName = "Body_PB_Value#{index}";
                            if (comp.progressBarData.VisibleBody.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleBody)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleBody = new List<string>();
                            }

                            comp.progressBarData.VisibleBody.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_LeftArm:
                        {
                            childName = "LeftArm_PB_Value#{index}";
                            if (comp.progressBarData.VisibleLeftArm.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleLeftArm)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleLeftArm = new List<string>();
                            }

                            comp.progressBarData.VisibleLeftArm.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_LeftLeg:
                        {
                            childName = "LeftLeg_PB_Value#{index}";
                            if (comp.progressBarData.VisibleLeftLeg.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleLeftLeg)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleLeftLeg = new List<string>();
                            }

                            comp.progressBarData.VisibleLeftLeg.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_RightArm:
                        {
                            childName = "RightArm_PB_Value#{index}";
                            if (comp.progressBarData.VisibleRightArm.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleRightArm)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleRightArm = new List<string>();
                            }

                            comp.progressBarData.VisibleRightArm.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBar.Health_RightLeg:
                        {
                            childName = "RightLeg_PB_Value#{index}";
                            if (comp.progressBarData.VisibleRightLeg.Count > 0)
                            {
                                foreach (string s in comp.progressBarData.VisibleRightLeg)
                                {
                                    EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, s, false);
                                }
                                comp.progressBarData.VisibleRightLeg = new List<string>();
                            }

                            comp.progressBarData.VisibleRightLeg.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                }

                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastPercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", percent.ToString()), true);
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError("ProgressBar Error");
                TAdvancedHealth.Logger.LogError(e);
            }
        }

        /// <summary>
        /// Asynchronously updates the health user interface (UI) for the specified Unturned player.
        /// </summary>
        /// <param name="player">The Unturned player whose health UI is to be updated.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task UpdateHealthUIAsync(UnturnedPlayer player)
        {
            string voidname = "UpdateHealthUI";
            try
            {
                HealthData health = await TAdvancedHealth.Database.GetPlayerHealthAsync(player.Id);
                var transCon = player.SteamPlayer().transportConnection;
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                //Base
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Health", System.Math.Round(health.BaseHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_Simple, (int)((System.Math.Round(health.BaseHealth, 2) / _config.HealthSystemSettings.BaseHealth) * 100), (int)((comp.progressBarData.LastSimpleHealth / _config.HealthSystemSettings.BaseHealth) * 100));
                //Head
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Head", System.Math.Round(health.HeadHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_Head, (int)((System.Math.Round(health.HeadHealth, 2) / _config.HealthSystemSettings.HeadHealth) * 100), (int)((comp.progressBarData.LastHealthHead / _config.HealthSystemSettings.HeadHealth) * 100));
                //Bpdy
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Body", System.Math.Round(health.BodyHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_Body, (int)((System.Math.Round(health.BodyHealth, 2) / _config.HealthSystemSettings.BodyHealth) * 100), (int)((comp.progressBarData.LastHealthBody / _config.HealthSystemSettings.BodyHealth) * 100));
                //LeftArm
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_LeftArm", System.Math.Round(health.LeftArmHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_LeftArm, (int)((System.Math.Round(health.LeftArmHealth, 2) / _config.HealthSystemSettings.LeftArmHealth) * 100), (int)((comp.progressBarData.LastHealthLeftArm / _config.HealthSystemSettings.LeftArmHealth) * 100));
                //LeftLeg
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_LeftLeg", System.Math.Round(health.LeftLegHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_LeftLeg, (int)((System.Math.Round(health.LeftLegHealth, 2) / _config.HealthSystemSettings.LeftLegHealth) * 100), (int)((comp.progressBarData.LastHealthLeftLeg / _config.HealthSystemSettings.LeftLegHealth) * 100));
                //RightArm
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_RightArm", System.Math.Round(health.RightArmHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_RightArm, (int)((System.Math.Round(health.RightArmHealth, 2) / _config.HealthSystemSettings.RightArmHealth) * 100), (int)((comp.progressBarData.LastHealthRightArm / _config.HealthSystemSettings.RightArmHealth) * 100));
                //RightLeg
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_RightLeg", System.Math.Round(health.RightLegHealth, 2).ToString());
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Health_RightLeg, (int)((System.Math.Round(health.RightLegHealth, 2) / _config.HealthSystemSettings.RightLegHealth) * 100), (int)((comp.progressBarData.LastHealthRightLeg / _config.HealthSystemSettings.RightLegHealth) * 100));
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
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
                HealthData health = await TAdvancedHealth.Database.GetPlayerHealthAsync(player.Id);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                await UpdateHealthUIAsync(player);
                comp.progressBarData.LastHealthHead = health.HeadHealth;
                comp.progressBarData.LastHealthBody = health.BodyHealth;
                comp.progressBarData.LastHealthLeftArm = health.LeftArmHealth;
                comp.progressBarData.LastHealthLeftLeg = health.LeftLegHealth;
                comp.progressBarData.LastHealthRightArm = health.RightArmHealth;
                comp.progressBarData.LastHealthRightLeg = health.RightLegHealth;
                //Stats
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Food, player.Player.life.food, player.Player.life.food);
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Stamina, player.Player.life.stamina, player.Player.life.stamina);
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Water, player.Player.life.water, player.Player.life.water);
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Radiation, player.Player.life.virus, player.Player.life.virus);
                await SendUIEffectProgressBarAsync((short)comp.EffectID, player.CSteamID, true, EProgressBar.Oxygen, player.Player.life.oxygen, player.Player.life.oxygen);

            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }
    }
}
