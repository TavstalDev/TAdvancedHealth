#region References
using System;
using System.Collections.Generic;
using Rocket.Unturned.Player;
using Steamworks;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;
using SDG.Unturned;
#endregion

namespace Tavstal.TAdvancedHealth.Modules
{
    public class EffectHelper
    {
        public static void sendUIEffectProgressBar(short key, CSteamID steamID, bool reliable, EProgressBarType type, int percent, int lastpercent)
        {
            try
            {
                UnturnedPlayer p = UnturnedPlayer.FromCSteamID(steamID);
                PlayerHealth health = TAdvancedHealthMain.Database.GetPlayerHealth(p.Id);
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();
                var transCon = p.SteamPlayer().transportConnection;
                var config = TAdvancedHealthMain.Instance.Configuration.Instance;
                string childName = null;
                switch (type)
                {
                    case EProgressBarType.Health_Simple:
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

                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", lastpercent.ToString()), false);
                            comp.progressBarData.VisibleSimpleHealth.Add(childName.Replace("{index}", percent.ToString()));
                            EffectManager.sendUIEffectVisibility((short)health.HUDEffectID, transCon, true, childName.Replace("{index}", percent.ToString()), true);

                            break;
                        }
                    case EProgressBarType.Food:
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
                    case EProgressBarType.Water:
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
                    case EProgressBarType.Radiation:
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
                    case EProgressBarType.Oxygen:
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
                    case EProgressBarType.Stamina:
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
                    case EProgressBarType.Health_Head:
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
                    case EProgressBarType.Health_Body:
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
                    case EProgressBarType.Health_LeftArm:
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
                    case EProgressBarType.Health_LeftLeg:
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
                    case EProgressBarType.Health_RightArm:
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
                    case EProgressBarType.Health_RightLeg:
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

                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", lastpercent.ToString()), false);
                EffectManager.sendUIEffectVisibility(key, transCon, reliable, childName.Replace("{index}", percent.ToString()), true);
            }
            catch (Exception e)
            {
                LoggerHelper.LogError("ProgressBar Error");
                LoggerHelper.LogError(e);
            }
        }
    }
}
