using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
// ReSharper disable UnusedType.Global

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHealth : CustomCommandBase
    {
        public override IPlugin Plugin => AdvancedHealth.Instance;
        public override bool UseBackgroundThread => false;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "sethealth";
        public override string Help => "Changes your health or somebody else's.";
        public override string Syntax => "/sethealth <player> [bodypart] [newHealth]";
        public override List<string> Aliases => new List<string>();
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.sethealth" };
        public override List<ISubcommand> SubCommands => new List<ISubcommand>();

        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            try
            {
                UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                if (args.Length == 3 || args.Length == 2)
                {
                    UnturnedPlayer targetPlayer = callerPlayer;
                    string bodyPart;
                    float newHealth;
                    if (args.Length == 3)
                    {
                        targetPlayer = UnturnedPlayer.FromName(args[0]);
                        if (targetPlayer == null)
                        {
                            AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_error_player_not_found", AdvancedHealth.Instance.Config.General.MessageIcon);
                            return true;
                        }

                        bodyPart = args[1].ToLower();
                        newHealth = Convert.ToSingle(args[2]);
                    }
                    else
                    {
                        bodyPart = args[0].ToLower();
                        newHealth = Convert.ToSingle(args[1]);
                    }

                    AdvancedHealthComponent targetComp = ComponentManager.Get(targetPlayer);
                    var health = targetComp.HealthData;
                    if (health == null)
                        return true;
                    
                    switch (bodyPart)
                    {
                        case "head":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetHeadHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_head"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_head"), newHealth);
                                break;
                            }
                        case "body":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetBodyHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_body"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_body"), newHealth);
                                break;
                            }
                        case "rightarm":
                        case "rarm":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetRightArmHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_right_arm"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_right_arm"), newHealth);
                                break;
                            }
                        case "leftarm":
                        case "larm":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.LeftArmHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetLeftArmHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_left_arm"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_left_arm"), newHealth);
                                break;
                            }
                        case "leftleg":
                        case "lleg":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.LeftLegHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetLeftLegHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_left_leg"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_left_leg"), newHealth);
                                break;
                            }
                        case "rightleg":
                        case "rleg":
                            {
                                float settingsHealth = AdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth;
                                if (newHealth > settingsHealth)
                                    newHealth = settingsHealth;
                                else if (newHealth < 0)
                                {
                                    if (newHealth * -1 > settingsHealth)
                                        newHealth = settingsHealth;
                                    else
                                        newHealth *= -1;
                                }
                                
                                health.SetRightLegHealth(newHealth);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealth_success", AdvancedHealth.Instance.Config.General.MessageIcon, targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_right_leg"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_success_other", AdvancedHealth.Instance.Config.General.MessageIcon, callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("limb_right_leg"), newHealth);
                                break;
                            }
                    }
                }
                else
                    UChatHelper.SendPlainChatMessage(callerPlayer.SteamPlayer(), Syntax);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error("Unexpected error occured in SetHealth command:", ex);
            }
            return true;
        }
    }
}