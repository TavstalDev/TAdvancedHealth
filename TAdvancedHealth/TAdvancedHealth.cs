﻿using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Handlers;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enums;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Compatibility;
using UnityEngine;

namespace Tavstal.TAdvancedHealth
{
    /// <summary>
    /// Represents a plugin for advanced health management.
    /// </summary>
    /// <typeparam name="TAdvancedHealthConfig">The type of configuration used by the plugin.</typeparam>
    public class TAdvancedHealth : PluginBase<TAdvancedHealthConfig>
    {
        public new static TAdvancedHealth Instance { get; private set; }
        public static DatabaseManager Database { get; private set; }
        private HarmonyLib.Harmony HarmonyPatcher { get; set; }
        public static bool IsConnectionAuthFailed { get; set; }
        private bool _hasFullMoon = false;
        public HealthSystemSettings CHSCSettings => Config.HealthSystemSettings;

        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            Instance = this;
            Database = new DatabaseManager(Config);

            UnturnedEventHandler.Attach();
            HealthSystemEventHandler.Attach();
            _hasFullMoon = LightingManager.isFullMoon;

            HarmonyPatcher = new HarmonyLib.Harmony("tavstal.tadvancedhealth.harmony");
            HarmonyPatcher.PatchAll();

            Logger.LogWarning("████████╗███╗░░░███╗███████╗██████╗░░██████╗██╗░░░██╗░██████╗");
            Logger.LogWarning("╚══██╔══╝████╗░████║██╔════╝██╔══██╗██╔════╝╚██╗░██╔╝██╔════╝");
            Logger.LogWarning("░░░██║░░░██╔████╔██║█████╗░░██║░░██║╚█████╗░░╚████╔╝░╚█████╗░");
            Logger.LogWarning("░░░██║░░░██║╚██╔╝██║██╔══╝░░██║░░██║░╚═══██╗░░╚██╔╝░░░╚═══██╗");
            Logger.LogWarning("░░░██║░░░██║░╚═╝░██║███████╗██████╔╝██████╔╝░░░██║░░░██████╔╝");
            Logger.LogWarning("░░░╚═╝░░░╚═╝░░░░░╚═╝╚══════╝╚═════╝░╚═════╝░░░░╚═╝░░░╚═════╝░");
            Logger.Log("#########################################");
            Logger.Log("# Thanks for using my plugin");
            Logger.Log("# Plugin Created By Tavstal");
            Logger.Log("# Discord: Tavstal#6189");
            Logger.Log("# Website: https://redstoneplugins.com");
            Logger.Log("#########################################");
            Logger.Log(string.Format("# Build Version: {0}", Version));
            Logger.Log(string.Format("# Build Date: {0}", BuildDate));
            Logger.Log("#########################################");
            Logger.Log("# TAdvancedHealth has been loaded.");
        }

        /// <summary>
        /// Called when the plugin is unloaded.
        /// </summary>
        public override async void OnUnLoad()
        {
            UnturnedEventHandler.Dettach();
            HealthSystemEventHandler.Dettach();
            HarmonyPatcher.UnpatchAll();
            Logger.Log("# TAdvancedHealth has been successfully unloaded!");

            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                HealthData health = await Database.GetPlayerHealthAsync(p.Id);
                EffectManager.askEffectClearByID(health.HUDEffectID, steamPlayer.transportConnection);

                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, true);
                p.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, true);
            }
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        internal async void Update()
        {
            string voidname = "UpdateHealthAsync()";
            try
            {
                // Update Moon State
                if (_hasFullMoon != LightingManager.isFullMoon)
                {
                    _hasFullMoon = LightingManager.isFullMoon;
                    UnturnedEventHandler.Event_OnMoonUpdated(LightingManager.isFullMoon);
                }

                // Should be rechecked
                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    var transCon = steamPlayer.transportConnection;
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    HealthData healthData = await Database.GetPlayerHealthAsync(player.Id);
                    #region Injured
                    if (healthData.IsInjured)
                    {
                        player.Bleeding = false;

                        int secs = (int)(healthData.DeathDate - DateTime.Now).TotalSeconds;
                        EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_message", Localize("ui_bleeding", secs.ToString()));
                        if (healthData.DeathDate < DateTime.Now)
                            await comp.BleedOutAsync();
                    }
                    #endregion

                    #region Regeneration
                    // Head
                    if (comp.NextHeadHealDate <= DateTime.Now)
                    {
                        if (healthData.HeadHealth + 1 <= CHSCSettings.HeadHealth && player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                            await Database.UpdateHealthAsync(player.Id, healthData.HeadHealth + 1, EHealth.HEAD);
                        comp.NextHeadHealDate = DateTime.Now.AddSeconds(CHSCSettings.HeadRegenTicks);
                    }

                    // Body
                    if (comp.NextBodyHealDate <= DateTime.Now)
                    {
                        if (healthData.BodyHealth + 1 <= CHSCSettings.BodyHealth && player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                            await Database.UpdateHealthAsync(player.Id, healthData.BodyHealth + 1, EHealth.BODY);
                        comp.NextBodyHealDate = DateTime.Now.AddSeconds(CHSCSettings.BodyRegenTicks);
                    }

                    // Arm
                    if (comp.NextArmHealDate <= DateTime.Now)
                    {
                        if (player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                        {
                            if (healthData.LeftArmHealth + 1 <= CHSCSettings.LeftArmHealth)
                                await Database.UpdateHealthAsync(player.Id, healthData.LeftArmHealth + 1, EHealth.LEFT_ARM);
                            if (healthData.RightArmHealth + 1 <= CHSCSettings.RightArmHealth)
                                await Database.UpdateHealthAsync(player.Id, healthData.RightArmHealth + 1, EHealth.RIGHT_ARM);
                        }
                        comp.NextArmHealDate = DateTime.Now.AddSeconds(CHSCSettings.ArmRegenTicks);
                    }

                    // Leg
                    if (comp.NextLegHealDate <= DateTime.Now)
                    {
                        if (player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                        {
                            if (healthData.LeftLegHealth + 1 <= CHSCSettings.LeftLegHealth)
                                await Database.UpdateHealthAsync(player.Id, healthData.LeftLegHealth + 1, EHealth.LEFT_LEG);
                            if (healthData.RightLegHealth + 1 <= CHSCSettings.RightLegHealth)
                                await Database.UpdateHealthAsync(player.Id, healthData.RightLegHealth + 1, EHealth.RIGHT_LEG);
                        }
                        comp.NextLegHealDate = DateTime.Now.AddSeconds(CHSCSettings.LegRegenTicks);
                    }
                    #endregion

                    #region Dragging
                    if (comp.DragState == EDragState.DRAGGER && comp.DragPartnerId != CSteamID.Nil)
                    {
                        UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(comp.DragPartnerId);

                        if (partner != null)
                            if (Vector3.Distance(partner.Position, player.Position) > 3)
                                partner.Player.teleportToPlayer(player.Player);
                    }
                    #endregion
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }

        public override Dictionary<string, string> LanguagePacks => new Dictionary<string, string>();

        public override Dictionary<string, string> DefaultLocalization =>
            new Dictionary<string, string>
            {
                // TODO, check translations
               { "prefix", "&e[TAH] " },
               { "error_healtbar_disabled", "&aYou should enable the custom healthbar to use this command." },
               { "error_command_sethealthhud_args", "&aUsage: /sethealthhud list <page> | [name]" },
               { "error_command_sethealthhud_list_args", "&aUsage: /sethealthhud list <page>" },
               { "command_cure_success","&aYou have successfully healed {0}"},
               { "command_succcess_sethealth", "&aYou have successfully set the health of your {0} to {1}" },
               { "command_succcess_sethealth_other", "&aYou have successfully changed the health of {0}'s {1} to {2}" },
               { "command_sethealth_other", "&a{0} changed the health of your {1} to {2}" },
               { "command_sethealthhud_list_element", "&aStyle name: {0}" },
               { "command_sethealthhud_list_next", "&aNext page: {0}" },
               { "command_sethealthhud_list_end", "&aYou have reached the end of the styles" },
               { "error_command_sethealthhud_style_invalid", "&aThe {0} style does not exist." },
               { "success_revive_other","&a{0} healed you"},
               { "error_revive_failed","&6Failed to cure someone because you turned away"},
               { "error_defiblirator_cooldown","&6This defiblirator is on cooldown for {0} second(s)."},
               { "error_playet_not_found", "&6Player not found" },
               { "revive_start_other", "&6Reviving {0} in {1} secs" },
               { "player_injured", "&6[Injured]: {0} needs a medic! (It has been marked on the map)" },
               { "success_defiblirator_revive", "&6You have successfully revived {0}." },
               { "success_defiblirator_revive_other", "&6You have been successfully revived by {0}." },
               { "error_defiblirator_revive_fail", "&6Failed to revive {0}." },
               { "error_defiblirator_no_player", "&6You need to be closer to a player." },
               { "error_defiblirator_not_injured", "&6This player is not injured." },
               { "ui_revive_progress", "You will revive <color=green>{0}</color> in <color=yellow>{1}</color> seconds" },
               { "ui_bleeding", "You will bleed in <color=red>{0}</color> seconds" },
               { "ui_revive_start_other", "<color=green>{0}</color> has began to revive you" },
               { "head", "head" },
               { "body", "body" },
               { "rightarm", "rightarm" },
               { "leftarm", "leftarm" },
               { "rightleg", "rightleg" },
               { "leftleg", "leftleg" }
            };
    }
}