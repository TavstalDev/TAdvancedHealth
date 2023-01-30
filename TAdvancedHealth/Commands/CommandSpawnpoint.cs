using Rocket.Unturned.Player;
using System.Collections.Generic;
using Rocket.API;
using System.Linq;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Helpers;

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
            var TAdvancedHealthMain = TAdvancedHealth.TAdvancedHealthMain.Instance;
            var config = TAdvancedHealthMain.Configuration.Instance;
            if (args.Length == 1)
            {
                Hospital h = config.HospitalSettings.hospitals.FirstOrDefault(x => x.Name.ToLower() == args[0].ToLower());
                if (h != null)
                {
                    h.Position.Add(new SerializableVector3(player.Position));
                    TAdvancedHealthMain.Configuration.Save();
                }
                else
                {
                    UnturnedHelper.SendChatMessage(player.SteamPlayer(), TAdvancedHealthMain.Translate(true, "Hospital_isnot_exists"));
                    return;
                }
            }
            else
            {
                UnturnedHelper.SendChatMessage(player.SteamPlayer(), Syntax);
                return;
            }
        }
    }
}
