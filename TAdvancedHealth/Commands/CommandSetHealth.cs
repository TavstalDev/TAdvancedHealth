using System;
using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth
{
    public class CommandSetHealth : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sethealth";
        public string Help => "Changes your health or somebody else's.";
        public string Syntax => "/sethealth <player> [bodypart] [newhealth]";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "TAdvancedHealth.command.sethealth" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            try
            {
                UnturnedPlayer callerPlayer = (UnturnedPlayer)caller;
                var hsettings = TAdvancedHealthMain.Instance.Configuration.Instance.CustomHealtSystemAndComponentSettings;
                var config = TAdvancedHealthMain.Instance.Configuration.Instance;
                var main = TAdvancedHealthMain.Instance;
                if (args.Length == 2)
                {
                    if (args[0].ToLower() == "head")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.HeadHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateHeadHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "head"), newhealth));
                    }
                    else if (args[0].ToLower() == "body")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.BodyHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateBodyHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "body"), newhealth));
                    }
                    else if (args[0].ToLower() == "rightarm")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.RightArmHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateRightArmHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "rightarm"), newhealth));
                    }
                    else if (args[0].ToLower() == "leftarm")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.LeftArmHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateLeftArmHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "leftarm"), newhealth));
                    }
                    else if (args[0].ToLower() == "leftleg")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.LeftLegHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateLeftLegHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "leftleg"), newhealth));
                    }
                    else if (args[0].ToLower() == "rightleg")
                    {
                        float newhealth = Convert.ToSingle(args[1]);
                        float settingsHealth = hsettings.RightLegHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateRightLegHealth(callerPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", main.Translate(true, "rightleg"), newhealth));
                    }

                }
                else if (args.Length == 3)
                {
                    UnturnedPlayer targetPlayer = UnturnedPlayer.FromName(args[0]);
                    if (targetPlayer == null)
                    {
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), "error_playet_not_found");
                        return;
                    }

                    if (args[1].ToLower() == "head")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.HeadHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateHeadHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "head"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "head"), newhealth));
                    }
                    else if (args[1].ToLower() == "body")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.BodyHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateBodyHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "body"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "body"), newhealth));
                    }
                    else if (args[1].ToLower() == "rightarm")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.RightArmHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateRightArmHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "rightarm"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "rightarm"), newhealth));
                    }
                    else if (args[1].ToLower() == "leftarm")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.LeftArmHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateLeftArmHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "leftarm"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "leftarm"), newhealth));
                    }
                    else if (args[1].ToLower() == "leftleg")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.LeftLegHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateLeftLegHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "leftleg"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "leftleg"), newhealth));
                    }
                    else if (args[1].ToLower() == "rightleg")
                    {
                        float newhealth = Convert.ToSingle(args[2]);
                        float settingsHealth = hsettings.RightLegHealth;
                        if (newhealth > settingsHealth)
                            newhealth = settingsHealth;
                        else if (newhealth < 0)
                        {
                            if (newhealth * -1 > settingsHealth)
                                newhealth = settingsHealth;
                            else
                                newhealth *= -1;
                        }

                        TAdvancedHealthMain.Database.UpdateRightLegHealth(targetPlayer.Id, newhealth);
                        UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), main.Translate(true, "command_succcess_sethealth", targetPlayer.CharacterName, main.Translate(true, "rightleg"), newhealth));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), main.Translate(true, "command_sethealth_other", callerPlayer.CharacterName, main.Translate(true, "rightleg"), newhealth));
                    }
                }
                else
                    UnturnedHelper.SendChatMessage(callerPlayer.SteamPlayer(), Syntax);
            }
            catch (Exception e)
            {
                Console.Write(e);
            }
        }
    }
}