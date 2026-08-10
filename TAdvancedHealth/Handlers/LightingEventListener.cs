using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Lighting;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers
{
    public class LightingEventListener : EventListener
    {
        [EventHandler]
        private void OnMoonUpdate(LightMoonEvent e)
        {
            foreach (SteamPlayer steamPlayer in Provider.clients)
            {
                UnturnedPlayer uPlayer = UnturnedPlayer.FromSteamPlayer(steamPlayer);
                if (uPlayer == null)
                    continue;
                
                AdvancedHealthComponent? comp = ComponentManager.Get(uPlayer);
                if (comp == null)
                    continue;

                if (e.IsFullMoon)
                    comp.TryAddState(EPlayerState.FULL_MOON);
                else
                    comp.TryRemoveState(EPlayerState.FULL_MOON);
            }
        }
    }
}