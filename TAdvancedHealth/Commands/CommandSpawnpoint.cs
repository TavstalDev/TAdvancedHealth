using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TLibrary.Models;
// ReSharper disable UnusedType.Global

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
            if (args.Length != 1)
            {
                UChatHelper.SendPlainChatMessage(player.SteamPlayer(), Syntax);
                return;
            }

            Hospital? hospital =
                AdvancedHealth.Instance.Config.HospitalSettings.Hospitals.FirstOrDefault(x =>
                    x.Name.ToLower() == args[0].ToLower());
            if (hospital == null)
            {
                AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "error_hospital_not_found", AdvancedHealth.Instance.Config.General.MessageIcon);
                return;
            }

            hospital.Position ??= new List<SerializableVector3>();
            hospital.Position.Add(new SerializableVector3(player.Position));
            AdvancedHealth.Instance.Config.Save();
            AdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "success_command_hospital_added", AdvancedHealth.Instance.Config.General.MessageIcon);
        }
    }
}
