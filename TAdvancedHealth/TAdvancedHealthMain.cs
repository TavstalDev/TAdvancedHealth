using Rocket.API.Collections;
using Rocket.Core.Plugins;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using Rocket.API;
using SDG.Unturned;
using UnityEngine;
using System.Collections.Generic;
using System;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using Steamworks;
using SDG.Framework.Utilities;
using System.Linq;
using Rocket.Unturned;
using Rocket.API.Serialisation;
using Tavstal.TAdvancedHealth.Modules;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Managers;
using System.Reflection;
using Tavstal.TAdvancedHealth.Handlers;
using Tavstal.TAdvancedHealth.Helpers;
using HarmonyLib;

namespace Tavstal.TAdvancedHealth
{
    public class TAdvancedHealthMain : RocketPlugin<TAdvancedHealthConfig>
    {
        public static TAdvancedHealthMain Instance { get; private set; }
        internal bool hasFullMoon = false;
        public static DatabaseManager Database { get; private set; }
        public CustomHealtSystemAndComponentSettings CHSCSettings => Configuration.Instance.CustomHealtSystemAndComponentSettings;
        private Harmony HarmonyPatcher { get; set; }
        internal System.Version _Version => Assembly.GetExecutingAssembly().GetName().Version;
        internal DateTime _BuildDate => new DateTime(2000, 1, 1).AddDays(_Version.Build).AddSeconds(_Version.Revision * 2);

        protected override void Load()
        {
            Instance = this;
            Database = new DatabaseManager();

            UnturnedEventHandler.OnLoad();
            HealthSystemEventHandler.OnLoad();
            hasFullMoon = LightingManager.isFullMoon;

            HarmonyPatcher = new Harmony("tavstal.tadvancedhealth.harmony");
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
            Logger.Log(string.Format("# Build Version: {0}", _Version));
            Logger.Log(string.Format("# Build Date: {0}", _BuildDate));
            Logger.Log("#########################################");
            Logger.Log("# TAdvancedHealth has been loaded.");
        }

        protected override void Unload()
        {
            UnturnedEventHandler.OnUnload();
            HealthSystemEventHandler.OnUnload();
            HarmonyPatcher.UnpatchAll();
            Logger.Log("# TAdvancedHealth has been successfully unloaded!");

            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer p = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                PlayerHealth health = Database.GetPlayerHealth(p.Id);
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

        internal System.Collections.IEnumerator DelayedInvoke(float time, System.Action action)
        {
            yield return new WaitForSeconds(time);
            action();
        }

        [Obsolete]
        public new string Translate(string translationKey, params object[] placeholder)
        {
            Logger.LogError($"OLD TRANSLATION METHOD WAS USED FOR '{translationKey}'");
            return Translations.Instance.Translate(translationKey, placeholder);
        }

        public string Translate(bool AddPrefix, string translationKey, params object[] placeholder)
        {
            if (AddPrefix)
                return Translations.Instance.Translate("prefix") + Translations.Instance.Translate(translationKey, placeholder);
            else
                return Translations.Instance.Translate(translationKey, placeholder);
        }

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList(){
                    { "prefix", "&e[TAH] " },
                    {"error_healtbar_disabled", "&aYou should enable the custom healthbar to use this command." },
                    {"error_command_sethealthhud_args", "&aUsage: /sethealthhud list <page> | [name]" },
                    {"error_command_sethealthhud_list_args", "&aUsage: /sethealthhud list <page>" },
                    {"command_cure_success","&aYou have successfully healed {0}"},
                    {"command_succcess_sethealth", "&aYou have successfully set the health of your {0} to {1}" },
                    {"command_succcess_sethealth_other", "&aYou have successfully changed the health of {0}'s {1} to {2}" },
                    {"command_sethealth_other", "&a{0} changed the health of your {1} to {2}" },
                    {"command_sethealthhud_list_element", "&aStyle name: {0}" },
                    {"command_sethealthhud_list_next", "&aNext page: {0}" },
                    {"command_sethealthhud_list_end", "&aYou have reached the end of the styles" },
                    {"error_command_sethealthhud_style_invalid", "&aThe {0} style does not exist." },
                    {"success_revive_other","&a{0} healed you"},
                    {"error_revive_failed","&6Failed to cure someone because you turned away"},
                    {"error_defiblirator_cooldown","&6This defiblirator is on cooldown for {0} second(s)."},
                    {"error_playet_not_found", "&6Player not found" },
                    {"revive_start_other", "&6Reviving {0} in {1} secs" },
                    {"player_injured", "&6[Injured]: {0} needs a medic! (It has been marked on the map)" },
                    { "success_defiblirator_revive", "&6You have successfully revived {0}." },
                    { "success_defiblirator_revive_other", "&6You have been successfully revived by {0}." },
                    { "error_defiblirator_revive_fail", "&6Failed to revive {0}." },
                    { "error_defiblirator_no_player", "&6You need to be closer to a player." },
                    { "error_defiblirator_not_injured", "&6This player is not injured." },
                    {"ui_revive_progress", "You will revive <color=green>{0}</color> in <color=yellow>{1}</color> seconds" },
                    {"ui_bleeding", "You will bleed in <color=red>{0}</color> seconds" },
                    {"ui_revive_start_other", "<color=green>{0}</color> has began to revive you" },
                    {"head", "head" },
                    {"body", "body" },
                    {"rightarm", "rightarm" },
                    {"leftarm", "leftarm" },
                    {"rightleg", "rightleg" },
                    {"leftleg", "leftleg" }
                };
            }
        }

        internal void Update()
        {
            string voidname = "Update()";
            try
            {
                if (hasFullMoon != LightingManager.isFullMoon)
                    UnturnedEventHandler.Event_OnMoonUpdated(LightingManager.isFullMoon);

                foreach (SteamPlayer steamPlayer in Provider.clients)
                {
                    UnturnedPlayer player = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                    var transCon = steamPlayer.transportConnection;
                    TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();

                    PlayerHealth h = UnturnedHelper.GetPlayerHealth(player.Id);
                    if (h.isInjured)
                    {
                        player.Bleeding = false;

                        int i = (int)(h.dieDate - DateTime.Now).TotalSeconds;
                        EffectManager.sendUIEffectText((short)comp.EffectID, transCon, true, "tb_message", Translate(true, "ui_bleeding", i.ToString()));
                        if (h.dieDate < DateTime.Now)
                            comp.BleedOut();
                    }

                    #region Regeneration
                    if (comp.nextHeadHealDate <= DateTime.Now)
                    {
                        if (h.HeadHealth + 1 <= CHSCSettings.HeadHealth && player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                            Database.UpdateHeadHealth(player.Id, h.HeadHealth + 1);
                        comp.nextHeadHealDate = DateTime.Now.AddSeconds(CHSCSettings.HeadRegenTicks);
                    }

                    if (comp.nextBodyHealDate <= DateTime.Now)
                    {
                        if (h.BodyHealth + 1 <= CHSCSettings.BodyHealth && player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                            Database.UpdateBodyHealth(player.Id, h.BodyHealth + 1);
                        comp.nextBodyHealDate = DateTime.Now.AddSeconds(CHSCSettings.BodyRegenTicks);
                    }

                    if (comp.nextArmHealDate <= DateTime.Now)
                    {
                        if (player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                        {
                            if (h.LeftArmHealth + 1 <= CHSCSettings.LeftArmHealth)
                                Database.UpdateLeftArmHealth(player.Id, h.LeftArmHealth + 1);
                            if (h.RightArmHealth + 1 <= CHSCSettings.RightArmHealth)
                                Database.UpdateRightArmHealth(player.Id, h.RightArmHealth + 1);
                        }
                        comp.nextArmHealDate = DateTime.Now.AddSeconds(CHSCSettings.ArmRegenTicks);
                    }

                    if (comp.nextLegHealDate <= DateTime.Now)
                    {
                        if (player.Player.life.food >= CHSCSettings.HealthRegenMinFood && player.Player.life.water >= CHSCSettings.HealthRegenMinWater && player.Player.life.virus >= CHSCSettings.HealthRegenMinVirus)
                        {
                            if (h.LeftLegHealth + 1 <= CHSCSettings.LeftLegHealth)
                                Database.UpdateLeftLegHealth(player.Id, h.LeftLegHealth + 1);
                            if (h.RightLegHealth + 1 <= CHSCSettings.RightLegHealth)
                                Database.UpdateRightLegHealth(player.Id, h.RightLegHealth + 1);
                        }
                        comp.nextLegHealDate = DateTime.Now.AddSeconds(CHSCSettings.LegRegenTicks);
                    }
                    #endregion

                    if (comp.DragState == EDragState.DRAGGER && comp.DragPartnerId != CSteamID.Nil)
                    {
                        UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(comp.DragPartnerId);

                        if (partner != null)
                            if (Vector3.Distance(partner.Position, player.Position) > 3)
                                partner.Player.teleportToPlayer(player.Player);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Log(string.Format("Error in {0}: {1}", voidname, e));
            }
        }
    }
}
