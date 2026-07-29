using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions.General;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models.Commands;
using Tavstal.TLibrary.Models.Plugin;
// ReSharper disable UnusedType.Global

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHealthHUD : CustomCommandBase
    {
        public override IPlugin Plugin => AdvancedHealth.Instance;
        public override bool UseBackgroundThread => false;
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "sethealthhud";
        public override string Help => "Sets/Lists the hud style.";
        public override string Syntax => "list <page> | [name]";
        public override List<string> Aliases => new List<string> { "sethhud", "shealthhud", "sethealthh" };
        public override List<string> Permissions => new List<string> { "tadvancedhealth.commands.sethealthhud" };

        public override List<ISubcommand> SubCommands => new List<ISubcommand>()
        {
            new SubCommand("list",  "Lists the hud styles.", "list <page>", new List<string>(), new List<string>(),
                AdvancedHealth.Instance, AllowedCaller,
                (player, args) => () =>
                {
                    if (player is ConsolePlayer)
                    {
                        
                        return;
                    }

                    var callerPlayer = (UnturnedPlayer)player;
                    int page = 1;
                    if (args.Length >= 1)
                    {
                        try
                        {
                            int.TryParse(args[0], out page);
                        }
                        catch
                        {
                            AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "error_command_sethealthhud_args", AdvancedHealth.Instance.Config.General.MessageIcon);
                            return;
                        }
                    }
                    if (page <= 0)
                        page = 1;

                    bool isEnd = false;
                    List<HUDStyle> styles = AdvancedHealth.Instance.Config.HUDStyles.FindAll(x => x.Enable);
                    for (int i = 0; i < 3; i++)
                    {
                        int index = i + (page - 1) * 3;
                        if (styles.IsValidIndex(index))
                        {
                            AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealthhud_list_element", AdvancedHealth.Instance.Config.General.MessageIcon, styles[index].Name);
                        }
                        else
                        {
                        
                            isEnd = true;
                            AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealthhud_list_end", AdvancedHealth.Instance.Config.General.MessageIcon);
                            break;
                        }
                    }

                    if (!isEnd)
                        AdvancedHealth.Instance.SendChatMessage(callerPlayer.SteamPlayer(), "command_sethealthhud_list_next", AdvancedHealth.Instance.Config.General.MessageIcon, page + 1);
                })
        };

        protected override bool HandleExecute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            var health = comp.HealthData;
            if (health == null)
                return true;
            
            if (args.Length == 0)
            {
                AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_args", AdvancedHealth.Instance.Config.General.MessageIcon);
                return true;
            }

            HUDStyle style = AdvancedHealth.Instance.Config.HUDStyles.Find(x => x.Enable && (x.Name == args[0] || x.Aliases.Contains(args[0])));
            if (style == null)
            {
                AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_command_sethealthhud_style_invalid", AdvancedHealth.Instance.Config.General.MessageIcon, args[0]);
                return true;
            }
            ushort oldId = health.HUDEffectID;
            comp.effectId = style.EffectID;
                
            if (health.IsHUDEnabled)
            {
                EffectManager.askEffectClearByID(oldId, player.SteamPlayer().transportConnection);
                UEffectHelper.SendUIEffect(style.EffectID, (short)style.EffectID, player.SteamPlayer().transportConnection, true);
                EffectHelper.UpdateWholeHealthUI(player);
            }
            return true;
        }
    }
}
