using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHealthHUD : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sethealthhud";
        public string Help => "Sets/Lists the hud style.";
        public string Syntax => "list <page> | [name]";
        public List<string> Aliases => new List<string> { "sethhud", "shealthhud", "sethealthh" };
        public List<string> Permissions => new List<string> { "tadvancedhealth.commands.sethealthhud" };

        public async void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);

            if (args.Length == 0)
            {
                AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_args");
                return;
            }

            if (args.Length == 1 && args[0].ToLower() != "list")
            {
                HUDStyle style = AdvancedHealth.Instance.Config.HUDStyles.Find(x => x.Enabled && (x.Name == args[0] || x.Aliases.ContainsIgnoreCase(args[0])));
                if (style == null)
                {
                    AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_style_invalid", args[0]);
                    return;
                }
                ushort oldId = health.HUDEffectID;
                comp.effectId = style.EffectID;
                await AdvancedHealth.DatabaseManager.UpdateHUDEffectIdAsync(player.Id, style.EffectID);
                
                if (health.IsHUDEnabled)
                {
                    SDG.Unturned.EffectManager.askEffectClearByID(oldId, player.SteamPlayer().transportConnection);
                    SDG.Unturned.EffectManager.sendUIEffect(style.EffectID, (short)style.EffectID, player.SteamPlayer().transportConnection, true);
                    await EffectHelper.UpdateWholeHealthUIAsync(player);
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
                        AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_args");
                        return;
                    }
                }
                if (page <= 0)
                    page = 1;

                bool isEnd = false;
                List<HUDStyle> styles = AdvancedHealth.Instance.Config.HUDStyles.FindAll(x => x.Enabled);
                for (int i = 0; i < 3; i++)
                {
                    int index = i + (page - 1) * 3;
                    if (styles.IsValidIndex(index))
                    {
                        AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "command_sethealthhud_list_element", styles[index].Name);
                    }
                    else
                    {
                        
                        isEnd = true;
                        AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "command_sethealthhud_list_end");
                        break;
                    }
                }

                if (!isEnd)
                    AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "command_sethealthhud_list_next", page + 1);
            }
            else
                AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_args");
        }
    }
}
