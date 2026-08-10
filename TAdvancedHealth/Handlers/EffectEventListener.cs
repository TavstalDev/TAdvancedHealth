using System;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Effect;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
// ReSharper disable UnusedMember.Global

namespace Tavstal.TAdvancedHealth.Handlers
{
    public class EffectEventListener : EventListener
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        [EventHandler]
        public void OnButtonClicked(EffectButtonEvent e)
        {
            try
            {
                if (e.ButtonName != "bt_revive_suicide")
                    return;

                e.IsCancelled = true;

                UnturnedPlayer uPlayer = UnturnedPlayer.FromPlayer(e.Player);
                AdvancedHealthComponent? comp = ComponentManager.Get(uPlayer);
                if (comp == null)
                    return;

                var health = comp.HealthData;
                if (health == null)
                    return;

                if (!health.IsInjured)
                    return;

                comp.allowDamage = true;
                uPlayer.Player.life.askDamage(100, uPlayer.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL,
                    CSteamID.Nil, out _);

                if (uPlayer.Player.movement.pluginSpeedMultiplier == 0)
                    uPlayer.Player.movement.sendPluginSpeedMultiplier(1);
                health.SetInjured(false);
                if (comp.dragState != EDragState.None)
                    comp.UnDrag();

                uPlayer.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
                EffectManager.sendUIEffectVisibility((short)_config.EffectId, comp.TranspConnection, true,
                    "RevivePanel", false);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnButtonClicked)}.", ex);
            }
        }
    }
}