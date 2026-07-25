using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Utils.Handlers;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Logging;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TAdvancedHealth
{
    /// <summary>
    /// Represents a plugin for advanced health management.
    /// </summary>
    public class AdvancedHealth : PluginBase<AdvancedHealthConfig>
    {
        public static AdvancedHealth Instance { get; private set; } = null!;
        public static DatabaseManager DatabaseManager { get; private set; } = null!;
        public static bool IsConnectionAuthFailed { get; set; }
        private HarmonyLib.Harmony HarmonyPatcher { get; set; }
        private bool _hasFullMoon;
        private DateTime _nextUpdate;

        public override void OnPreLoad()
        {
            Instance = this;
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("────────────────────────────────────────────────────────");
            sb.AppendLine();
            sb.AppendLine("████████╗███╗░░░███╗███████╗██████╗░░██████╗██╗░░░██╗░██████╗");
            sb.AppendLine("╚══██╔══╝████╗░████║██╔════╝██╔══██╗██╔════╝╚██╗░██╔╝██╔════╝");
            sb.AppendLine("░░░██║░░░██╔████╔██║█████╗░░██║░░██║╚█████╗░░╚████╔╝░╚█████╗░");
            sb.AppendLine("░░░██║░░░██║╚██╔╝██║██╔══╝░░██║░░██║░╚═══██╗░░╚██╔╝░░░╚═══██╗");
            sb.AppendLine("░░░██║░░░██║░╚═╝░██║███████╗██████╔╝██████╔╝░░░██║░░░██████╔╝");
            sb.AppendLine("░░░╚═╝░░░╚═╝░░░░░╚═╝╚══════╝╚═════╝░╚═════╝░░░░╚═╝░░░╚═════╝░");
            sb.AppendLine();
            sb.AppendLine("[ About ]");
            sb.AppendLine(" ▸ Developer : Tavstal");
            sb.AppendLine(" ▸ Discord   : @Tavstal");
            sb.AppendLine(" ▸ Website   : https://redstoneplugins.com");
            sb.AppendLine(" ▸ GitHub    : https://github.com/TavstalDev");
            sb.AppendLine();
            sb.AppendLine("[ Build ]");
            sb.AppendLine($" ▸ Version   : {Version}");
            sb.AppendLine($" ▸ Build Date: {BuildDate} UTC");
            sb.AppendLine($" ▸ TLibrary  : {LibraryVersion}");
            sb.AppendLine();
            sb.AppendLine("[ Support ]");
            sb.AppendLine(" ▸ Report issues or request features:");
            sb.AppendLine(" ▸ https://github.com/TavstalDev/TAdvancedHealth/issues");
            sb.AppendLine();
            sb.AppendLine("────────────────────────────────────────────────────────");
            Logger.Log(ELogLevel.COMMAND, sb.ToString(), includePrefixes: false, color:  ConsoleColor.Cyan);
        }
        
        /// <summary>
        /// Called when the plugin is loaded.
        /// </summary>
        public override void OnLoad()
        {
            DatabaseManager = new DatabaseManager(Config);

            UnturnedEventHandler.Attach();
            HealthSystemEventHandler.Attach();
            _hasFullMoon = LightingManager.isFullMoon;

            HarmonyPatcher = new HarmonyLib.Harmony("tavstal.tadvancedhealth.harmony");
            HarmonyPatcher.PatchAll();
            
            Logger.Info("# TAdvancedHealth has been loaded.");
        }

        /// <summary>
        /// Called when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            UnturnedEventHandler.Detach();
            HealthSystemEventHandler.Detach();
            HarmonyPatcher.UnpatchAll();
            
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                foreach (var hudStyle in Config.HUDStyles)
                    EffectManager.askEffectClearByID(hudStyle.EffectID, steamPlayer.transportConnection);

                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowFood, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowHealth, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowOxygen, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStamina, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowVirus, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowWater, true);
                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.ShowStatusIcons, true);
            }
            
            Logger.Info("# TAdvancedHealth has been successfully unloaded!");
        }

        /// <summary>
        /// Called every frame, if the MonoBehaviour is enabled.
        /// </summary>
        private void Update()
        {
            string methodName = "Update()";
            try
            {
                if (_nextUpdate > DateTime.Now)
                    return;

                // Update Moon State
                if (_hasFullMoon != LightingManager.isFullMoon)
                {
                    _hasFullMoon = LightingManager.isFullMoon;
                    UnturnedEventHandler.OnMoonUpdated(LightingManager.isFullMoon);
                }

                _nextUpdate = DateTime.Now.AddSeconds(5);
            }
            catch (Exception ex)
            {
                Logger.Error($"Error in {methodName}.", ex);
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
               { "success_command_hospital_added", "&aThe hospital bed has been successfully added." },
               { "error_hospital_not_found", "&cHospital was not found."  },
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
