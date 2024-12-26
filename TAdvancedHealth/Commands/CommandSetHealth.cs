using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHealth : CommandBase
    {
        protected override IPlugin Plugin => AdvancedHealth.Instance;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "sethealth";
        public override string Help => "Changes your health or somebody else's.";
        public override string Syntax => "/sethealth <player> [bodypart] [newHealth]";
        public override List<string> Aliases => new List<string>();
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.sethealth" };
        protected override List<SubCommand> SubCommands => new List<SubCommand>();

        protected override async Task<bool> ExecutionRequested(IRocketPlayer caller, string[] args)
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
                            AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found");
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.Head);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("head"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("head"), newHealth);
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.Body);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("body"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("body"), newHealth);
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.RightARM);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("rightarm"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("rightarm"), newHealth);
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.LeftARM);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("leftarm"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("leftarm"), newHealth);
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.LeftLeg);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("leftleg"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("leftleg"), newHealth);
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

                                await AdvancedHealth.DatabaseManager.UpdateHealthAsync(callerPlayer.Id, newHealth, EHealth.RightLeg);
                                AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_succcess_sethealth", targetPlayer.CharacterName, AdvancedHealth.Instance.Localize("rightleg"), newHealth);
                                if (!targetPlayer.Equals(callerPlayer))
                                    AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "command_sethealth_other", callerPlayer.CharacterName, AdvancedHealth.Instance.Localize("rightleg"), newHealth);
                                break;
                            }
                    }
                }
                else
                    UChatHelper.SendPlainChatMessage(callerPlayer.SteamPlayer(), Syntax);
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogException("Error in SetHealth command:");
                AdvancedHealth.Logger.LogError(e);
            }
            return true;
        }
    }
}