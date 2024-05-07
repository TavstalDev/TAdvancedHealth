using Rocket.API;
using Rocket.Unturned.Player;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;

namespace Tavstal.TAdvancedHealth
{
    public class CommandSetHospitalBed : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "sethospitalbed";
        public string Help => "";
        public string Syntax => "/sethospitalbed <hospitalname>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "TAdvancedHealth.command.sethospitalbed" };

        public void Execute(IRocketPlayer caller, string[] args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (args.Length == 1)
            {
                Hospital hospital = TAdvancedHealth.Instance.Config.HospitalSettings.Hospitals.FirstOrDefault(x => x.Name.ToLower() == args[0].ToLower());
                if (hospital != null)
                {
                    hospital.Position.Add(new TLibrary.Compatibility.SerializableVector3(player.Position));
                    TAdvancedHealth.Instance.Config.SaveConfig();
                }
                else
                {
                    TAdvancedHealth.Instance.SendChatMessage(player.SteamPlayer(), "Hospital_isnot_exists");
                    return;
                }
            }
            else
            {
                UChatHelper.SendPlainChatMessage(player.SteamPlayer(), Syntax);
                return;
            }
        }
    }
}
