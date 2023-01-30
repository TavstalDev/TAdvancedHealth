using Rocket.API;
using Rocket.Unturned.Player;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Collections;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Modules;
using Tavstal.TAdvancedHealth.Managers;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHealthHUD : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sethealthhud";
        public string Help => "Sets/Lists the hud style.";
        public string Syntax => "list <page> | [name]";
        public List<string> Aliases => new List<string> { "sethhud", "shealthhud", "sethealthh" };
        public List<string> Permissions => new List<string> { "TAdvancedHealth.command.sethealthhud" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            TAdvancedHealthComponent comp = player.GetComponent<TAdvancedHealthComponent>();
            PlayerHealth health = TAdvancedHealthMain.Database.GetPlayerHealth(player.Id);

            if (args.Length == 0)
            {
                UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_command_sethealthhud_args"));
                return;
            }

            if (args.Length == 1 && args[0].ToLower() != "list")
            {
                
                HUDStyle style = TAdvancedHealthMain.Instance.Configuration.Instance.HUDStyles.Find(x => x.Enabled && (x.Name == args[0] || x.Aliases.ContainsIgnoreCase(args[0])));
                if (style == null)
                {
                    UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_command_sethealthhud_style_invalid", args[0]));
                    return;
                }
                ushort oldId = health.HUDEffectID;
                comp.EffectID = style.EffectID;
                TAdvancedHealthMain.Database.UpdateHUDEffectId(player.Id, style.EffectID);
                
                if (health.isHUDEnabled)
                {
                    SDG.Unturned.EffectManager.askEffectClearByID(oldId, player.SteamPlayer().transportConnection);
                    SDG.Unturned.EffectManager.sendUIEffect(style.EffectID, (short)style.EffectID, player.SteamPlayer().transportConnection, true);
                    UnturnedHelper.UpdateHealthAllUI(player);
                }
            }
            else if (args[0].ToLower() == "list")
            {
                int page = 1;

                if (args.Length >= 2)
                {
                    try
                    {
                        int.TryParse(args[1], out page);
                    }
                    catch
                    {
                        UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_command_sethealthhud_args"));
                        return;
                    }
                }
                if (page <= 0)
                    page = 1;

                bool isEnd = false;
                List<HUDStyle> styles = TAdvancedHealthMain.Instance.Configuration.Instance.HUDStyles.FindAll(x => x.Enabled);
                for (int i = 0; i < 3; i++)
                {
                    int index = i + (page - 1) * 3;
                    if (styles.isValidIndex(index))
                    {
                        UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "command_sethealthhud_list_element", styles[index].Name));
                    }
                    else
                    {
                        
                        isEnd = true;
                        UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "command_sethealthhud_list_end"));
                        break;
                    }
                }

                if (!isEnd)
                    UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "command_sethealthhud_list_next", page + 1));
            }
            else
                UnturnedHelper.SendMessage(player.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_command_sethealthhud_args"));
        }
    }
}
