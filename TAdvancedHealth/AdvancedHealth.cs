using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Text;
using Tavstal.TAdvancedHealth.Handlers;
using Tavstal.TAdvancedHealth.Handlers.Player;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
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
        private HarmonyLib.Harmony? HarmonyPatcher { get; set; }
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
            Level.onPostLevelLoaded += OnPostLevelLoaded;
            DatabaseManager = new DatabaseManager(Config);

            PlayerConnectionHandler.Attach();
            PlayerInventoryHandler.Attach();
            PlayerLifeHandler.Attach();
            PlayerMovementHandler.Attach();
            PlayerStatHandler.Attach();
            UIEventHandler.Attach();
            VehicleEventHandler.Attach();
            HealthSystemEventHandler.Attach();
            _hasFullMoon = LightingManager.isFullMoon;

            HarmonyPatcher = new HarmonyLib.Harmony("tavstal.tadvancedhealth.harmony");
            HarmonyPatcher.PatchAll();
            
            Logger.Info("# TAdvancedHealth has been loaded.");
        }

        private void OnPostLevelLoaded(int level)
        {
            if (!DatabaseManager.IsAuthenticationFailed)
                return;
            Logger.Warning($"# Unloading {GetPluginName()} due to database authentication error.");
            this.UnloadPlugin();
        }

        /// <summary>
        /// Called when the plugin is unloaded.
        /// </summary>
        public override void OnUnLoad()
        {
            Level.onPostLevelLoaded -= OnPostLevelLoaded;
            PlayerConnectionHandler.Detach();
            PlayerInventoryHandler.Detach();
            PlayerLifeHandler.Detach();
            PlayerMovementHandler.Detach();
            PlayerStatHandler.Detach();
            UIEventHandler.Detach();
            VehicleEventHandler.Detach();
            HealthSystemEventHandler.Detach();
            HarmonyPatcher?.UnpatchAll();
            
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                EffectManager.askEffectClearByID(Config.EffectId, steamPlayer.transportConnection);

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
            try
            {
                if (_nextUpdate > DateTime.Now)
                    return;

                // Update Moon State
                if (_hasFullMoon != LightingManager.isFullMoon)
                {
                    _hasFullMoon = LightingManager.isFullMoon;
                    PlayerStatHandler.OnMoonUpdated(LightingManager.isFullMoon);
                }

                _nextUpdate = DateTime.Now.AddSeconds(5);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error occured in {nameof(Update)}.", ex);
            }
        }

        public override Dictionary<string, string> LanguagePacks => new Dictionary<string, string>();

        public override Dictionary<string, string> DefaultLocalization =>
            new Dictionary<string, string>
            {
                // GENERAL
                
               { "prefix", "&e[TAdvHealth] " },
               
               // COMMANDS
               
               { "command_error_player_not_found", "&6Player not found." },
               { "command_cure_success","&aYou have successfully healed {0}"},
               { "command_sethealth_success", "&aYou have successfully set the health of {0}'s {1} to {2}" },
               { "command_sethealth_success_other", "&a{0} changed the health of your {1} to {2}" },
               { "command_hospital_error_not_found", "&cHospital was not found."},
               { "command_hospital_added", "&aThe hospital bed has been successfully added."},
               
               // Defibrillator
               
               { "defibrillator_error_cooldown", "&6This defibrillator is on cooldown for {0} second(s)." },
               { "defibrillator_error_fail", "&6Failed to revive {0}" },
               { "defibrillator_error_too_far", "&6You are too far from that player." },
               { "defibrillator_error_healthy", "&6The targeted player is healthy." },
               { "defibrillator_revive", "&6You have successfully revived {0}." }, 
               { "defibrillator_revive_other", "&6You have been successfully revived by {0}." }, 
               { "defibrillator_alert", "&6[Injured]: {0} needs a medic! (Their location has been marked on the map)" },
               
               // UI

               { "ui_bleeding_title", "Unconscious" },
               { "ui_bleeding_message", "You will bleed out in <color=#e74c3c>{0}</color> seconds" },
               { "ui_bleeding_suicide", "GIVE UP" },

               // Limbs
               
               { "limb_head", "head" },
               { "limb_body", "body" },
               { "limb_left_arm", "left arm" },
               { "limb_left_leg", "left leg" },
               { "limb_right_arm", "right arm" },
               { "limb_right_leg", "right leg" },
            };
    }
}
