using System;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Player.Movement;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
using UnityEngine;
// ReSharper disable UnusedMember.Global
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerMovementListener : EventListener
    {
        [EventHandler]
        private void OnGesture(PlayerGestureEvent e)
        {
            try
            {
                switch (e.Gesture)
                {
                    case UnturnedPlayerEvents.PlayerGesture.SurrenderStart:
                    {
                        PlayerLook look = e.Player.Player.look;
                        SDG.Unturned.Player? victimPlayer = null;
                        if (Physics.Raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, 2f,
                                RayMasks.PLAYER))
                        {
                            var victimPlayer2 = hit.transform.GetComponent<SDG.Unturned.Player>();
                            if (victimPlayer2 != null &&
                                Vector3.Distance(victimPlayer2.transform.position, e.Player.Position) <= 5f)
                                victimPlayer = victimPlayer2;
                        }

                        if (victimPlayer != null)
                        {
                            UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(victimPlayer);
                            AdvancedHealthComponent? targetComp = ComponentManager.Get(targetPlayer);
                            targetComp?.Drag(targetPlayer);
                        }

                        break;
                    }
                    case UnturnedPlayerEvents.PlayerGesture.SurrenderStop:
                    {
                        AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                        if (comp == null)
                            break;
                        if (comp.dragState == EDragState.Dragger)
                            comp.UnDrag();
                        break;
                    }
                    case UnturnedPlayerEvents.PlayerGesture.Arrest_Start:
                    {
                        AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                        comp?.TryAddState(EPlayerState.HANDCUFFED);
                        break;
                    }
                    case UnturnedPlayerEvents.PlayerGesture.Arrest_Stop:
                    {
                        AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                        comp?.TryRemoveState(EPlayerState.HANDCUFFED);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnGesture)}.", ex);
            }
        }
    }
}