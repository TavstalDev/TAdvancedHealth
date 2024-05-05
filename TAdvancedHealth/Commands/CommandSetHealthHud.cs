using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;
using Tavstal.TAdvancedHealth.Modules;

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
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            HealthData health = TAdvancedHealth.Database.GetPlayerHealth(player.Id);

            if (args.Length == 0)
            {
                UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "error_command_sethealthhud_args"));
                return;
            }

            if (args.Length == 1 && args[0].ToLower() != "list")
            {
                
                HUDStyle style = TAdvancedHealth.Instance.Configuration.Instance.HUDStyles.Find(x => x.Enabled && (x.Name == args[0] || x.Aliases.ContainsIgnoreCase(args[0])));
                if (style == null)
                {
                    UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "error_command_sethealthhud_style_invalid", args[0]));
                    return;
                }
                ushort oldId = health.HUDEffectID;
                comp.EffectID = style.EffectID;
                TAdvancedHealth.Database.UpdateHUDEffectIdAsync(player.Id, style.EffectID);
                
                if (health.IsHUDEnabled)
                {
                    SDG.Unturned.EffectManager.askEffectClearByID(oldId, player.SteamPlayer().transportConnection);
                    SDG.Unturned.EffectManager.sendUIEffect(style.EffectID, (short)style.EffectID, player.SteamPlayer().transportConnection, true);
                    HealthHelper.UpdateHealthAllUI(player);
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
                        UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "error_command_sethealthhud_args"));
                        return;
                    }
                }
                if (page <= 0)
                    page = 1;

                bool isEnd = false;
                List<HUDStyle> styles = TAdvancedHealth.Instance.Configuration.Instance.HUDStyles.FindAll(x => x.Enabled);
                for (int i = 0; i < 3; i++)
                {
                    int index = i + (page - 1) * 3;
                    if (styles.isValidIndex(index))
                    {
                        UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "command_sethealthhud_list_element", styles[index].Name));
                    }
                    else
                    {
                        
                        isEnd = true;
                        UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "command_sethealthhud_list_end"));
                        break;
                    }
                }

                if (!isEnd)
                    UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "command_sethealthhud_list_next", page + 1));
            }
            else
                UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealth.Instance.Translate(true, "error_command_sethealthhud_args"));
        }
    }
}
