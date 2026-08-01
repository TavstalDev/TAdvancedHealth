using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Handlers.Player;

namespace Tavstal.TAdvancedHealth.Models
{
    public class PlayerStatSubscriptions
    {
        public OxygenUpdated OxygenCallback { get; private set; }
        public TemperatureUpdated TemperatureCallback { get; private set; }
        public SafetyUpdated SafetyCallback { get; private set; }
        public RadiationUpdated RadiationCallback { get; private set; }
        public VirusUpdated VirusCallback { get; private set; }

        public PlayerStatSubscriptions(UnturnedPlayer player)
        {
            OxygenCallback = oxygen => PlayerStatHandler.OnPlayerOxygenUpdate(player, oxygen);
            TemperatureCallback = newTemperature => PlayerStatHandler.OnPlayerTemperatureUpdate(player, newTemperature);
            SafetyCallback = isSafe => PlayerStatHandler.OnPlayerSafezoneUpdated(player, isSafe);
            RadiationCallback = isRadio => PlayerStatHandler.OnPlayerDeadzoneUpdated(player, isRadio);
            VirusCallback = virus => PlayerStatHandler.OnPlayerVirusUpdate(player, virus);
        }
    }
}