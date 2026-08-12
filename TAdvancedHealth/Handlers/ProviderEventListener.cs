using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Provider;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers
{
    public class ProviderEventListener : EventListener
    {
        [EventHandler]
        private void OnShutdown(ProviderShutdownEvent e)
        {
            AdvancedHealth.IsShuttingDown =  true;
        }
    }
}