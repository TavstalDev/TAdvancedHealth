﻿using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models;

namespace Tavstal.TAdvancedHealth.Commands
{
    public class CommandSetHospitalBed : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sethospitalbed";
        public string Help => "Sets a respawn point";
        public string Syntax => "/sethospitalbed <hospitalname>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "tadvancedhealth.commands.sethospitalbed" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (args.Length == 1)
            {
                Hospital hospital = AdvancedHealth.Instance.Config.HospitalSettings.Hospitals.FirstOrDefault(x => x.Name.ToLower() == args[0].ToLower());
                if (hospital != null)
                {
                    hospital.Position.Add(new SerializableVector3(player.Position));
                    AdvancedHealth.Instance.Config.SaveConfig();
                    AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "success_command_hospital_added");
                }
                else
                {
                    AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_hospital_not_found");
                    //return;
                }
            }
            else
            {
                UChatHelper.SendPlainChatMessage(player.SteamPlayer(), Syntax);
                //return;
            }
        }
    }
}
