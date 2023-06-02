using Rocket.API;
using Rocket.API.Collections;
using Rocket.API.Serialisation;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Modules;
using UnityEngine;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;

namespace Tavstal.TAdvancedHealth.Helpers
{
    public static class HealthHelper
    {
        private static IAsset<TAdvancedHealthConfig> Configuration => TAdvancedHealthMain.Instance.Configuration;

        public static void UpdateHealthUI(UnturnedPlayer p)
        {
            string voidname = "UpdateHealthUI";
            try
            {
                PlayerHealth health = GetPlayerHealth(p.Id);
                var config = Configuration.Instance.CustomHealtSystemAndComponentSettings;
                var transCon = p.SteamPlayer().transportConnection;
                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();

                //Base
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Health", System.Math.Round(health.BaseHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_Simple, (int)((System.Math.Round(health.BaseHealth, 2) / config.BaseHealth) * 100), (int)((comp.progressBarData.LastSimpleHealth / config.BaseHealth) * 100));
                //Head
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Head", System.Math.Round(health.HeadHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_Head, (int)((System.Math.Round(health.HeadHealth, 2) / config.HeadHealth) * 100), (int)((comp.progressBarData.LastHealthHead / config.HeadHealth) * 100));
                //Bpdy
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_Body", System.Math.Round(health.BodyHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_Body, (int)((System.Math.Round(health.BodyHealth, 2) / config.BodyHealth) * 100), (int)((comp.progressBarData.LastHealthBody / config.BodyHealth) * 100));
                //LeftArm
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_LeftArm", System.Math.Round(health.LeftArmHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_LeftArm, (int)((System.Math.Round(health.LeftArmHealth, 2) / config.LeftArmHealth) * 100), (int)((comp.progressBarData.LastHealthLeftArm / config.LeftArmHealth) * 100));
                //LeftLeg
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_LeftLeg", System.Math.Round(health.LeftLegHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_LeftLeg, (int)((System.Math.Round(health.LeftLegHealth, 2) / config.LeftLegHealth) * 100), (int)((comp.progressBarData.LastHealthLeftLeg / config.LeftLegHealth) * 100));
                //RightArm
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_RightArm", System.Math.Round(health.RightArmHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_RightArm, (int)((System.Math.Round(health.RightArmHealth, 2) / config.RightArmHealth) * 100), (int)((comp.progressBarData.LastHealthRightArm / config.RightArmHealth) * 100));
                //RightLeg
                EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_RightLeg", System.Math.Round(health.RightLegHealth, 2).ToString());
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Health_RightLeg, (int)((System.Math.Round(health.RightLegHealth, 2) / config.RightLegHealth) * 100), (int)((comp.progressBarData.LastHealthRightLeg / config.RightLegHealth) * 100));
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        public static void UpdateHealthAllUI(UnturnedPlayer p)
        {
            string voidname = "UpdateHealthUI";
            try
            {
                PlayerHealth health = GetPlayerHealth(p.Id);
                var config = Configuration.Instance.CustomHealtSystemAndComponentSettings;

                TAdvancedHealthComponent comp = p.GetComponent<TAdvancedHealthComponent>();

                UpdateHealthUI(p);
                comp.progressBarData.LastHealthHead = health.HeadHealth;
                comp.progressBarData.LastHealthBody = health.BodyHealth;
                comp.progressBarData.LastHealthLeftArm = health.LeftArmHealth;
                comp.progressBarData.LastHealthLeftLeg = health.LeftLegHealth;
                comp.progressBarData.LastHealthRightArm = health.RightArmHealth;
                comp.progressBarData.LastHealthRightLeg = health.RightLegHealth;
                //Stats
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Food, p.Player.life.food, p.Player.life.food);
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Stamina, p.Player.life.stamina, p.Player.life.stamina);
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Water, p.Player.life.water, p.Player.life.water);
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Radiation, p.Player.life.virus, p.Player.life.virus);
                EffectHelper.sendUIEffectProgressBar((short)comp.EffectID, p.CSteamID, true, EProgressBarType.Oxygen, p.Player.life.oxygen, p.Player.life.oxygen);

            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        public static PlayerHealth GetPlayerHealth(string id)
        {
            string voidname = "GetPlayerHealth";
            try
            {
                return TAdvancedHealthMain.Database.GetPlayerHealth(id);
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
                return null;
            }
        }

        public static void SetPlayerDowned(UnturnedPlayer player)
        {
            string voidname = "Downing";
            try
            {
                TAdvancedHealthComponent cp = player.GetComponent<TAdvancedHealthComponent>();
                var transCon = player.SteamPlayer().transportConnection;
                PlayerHealth health = GetPlayerHealth(player.Id);
                if (!health.isInjured)
                {
                    if (player.Dead) { return; }
                    player.Player.equipment.dequip();
                    if (player.Infection > 25)
                        player.Infection = 0;
                    player.Heal(100, false, false);
                    player.Bleeding = false;
                    player.Broken = true;
                    if (player.Hunger < 50)
                        player.Hunger = 50;
                    if (player.Thirst < 50)
                        player.Thirst = 50;
                    player.Player.movement.sendPluginSpeedMultiplier(0f);
                    player.Player.movement.sendPluginJumpMultiplier(0f);

                    health.dieDate = DateTime.Now.AddSeconds(Configuration.Instance.CustomHealtSystemAndComponentSettings.InjuredDeathTimeSecs);
                    health.isInjured = true;
                    health.HeadHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.HeadHealth;
                    health.BodyHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.BodyHealth;
                    health.LeftArmHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.LeftArmHealth;
                    health.RightArmHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.RightArmHealth;
                    health.LeftLegHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.LeftLegHealth;
                    health.RightLegHealth = Configuration.Instance.CustomHealtSystemAndComponentSettings.RightLegHealth;
                    TAdvancedHealthMain.Database.Update(player.Id, health, EDatabaseEventType.ALL);


                    player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

                    EffectManager.sendUIEffectVisibility((short)cp.EffectID, transCon, true, "bt_suicide", true);
                    EffectManager.sendUIEffectText((short)cp.EffectID, transCon, true, "tb_message", TAdvancedHealthMain.Instance.Translate(true, "ui_bleeding", (int)(health.dieDate - DateTime.Now).TotalSeconds));
                    EffectManager.sendUIEffectVisibility((short)cp.EffectID, transCon, true, "RevivePanel", true);
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer tmpPlayer = UnturnedPlayer.FromSteamPlayer(sp);
                        if ((tmpPlayer.HasPermission(Configuration.Instance.DefibrillatorSettings.PermissionForUseDefiblirator) || tmpPlayer.CSteamID == player.CSteamID) && !player.IsAdmin)
                        {
                            var teleportLocation = new Vector3(player.Position.x, player.Position.y, player.Position.z);
                            tmpPlayer.Player.quests.sendSetMarker(true, teleportLocation);
                            UnturnedHelper.SendChatMessage(sp, TAdvancedHealthMain.Instance.Translate(true, "player_injured", player.CharacterName));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        public static bool CanBleed(float health, float damage)
        {
            bool can = false;

            if (health != 0 && damage != 0 && Configuration.Instance.CustomHealtSystemAndComponentSettings.CanStartBleeding)
            {
                if (health / 100 * 20 <= damage)
                    can = true;
            }

            return can;
        }

        public static StatusIcon GetStatusIcon(EPlayerStates state)
        {
            StatusIcon icon = new StatusIcon();

            icon = Configuration.Instance.CustomHealtSystemAndComponentSettings.statusIcons.Find(x => x.Status == state);

            return icon;
        }
    }
}
