using System;
using System.Collections.Concurrent;
using Rocket.Unturned.Player;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Models.Logging;

namespace Tavstal.TAdvancedHealth.Utils.Managers
{
    public static class ComponentManager
    {
        private static readonly ConcurrentDictionary<string, AdvancedHealthComponent> _healthComponents = new ConcurrentDictionary<string, AdvancedHealthComponent>();
        private static TLogger Logger => AdvancedHealth.Logger;

        public static AdvancedHealthComponent Get(UnturnedPlayer player) => _healthComponents.GetOrAdd(player.Id, player.GetComponent<AdvancedHealthComponent>());

        public static void Invalidate(string id)
        {
            try
            {
                _healthComponents.TryRemove(id, out _);
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected occured while invalidating {id}'s component.", ex);
            }
        }
    }
}