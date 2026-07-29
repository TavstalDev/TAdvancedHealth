using System;
using Rocket.Unturned.Events;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Extensions;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerMovementHandler
    {
        internal static void Attach()
        {
            UnturnedPlayerEvents.OnPlayerUpdateGesture += OnPlayerGestureUpdated;
        }

        internal static void Detach()
        {
            UnturnedPlayerEvents.OnPlayerUpdateGesture -= OnPlayerGestureUpdated;
        }
        
        internal static void OnPlayerGestureUpdated(UnturnedPlayer player, UnturnedPlayerEvents.PlayerGesture gesture)
        {
            try
            {
                if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStart)
                {
                    PlayerLook look = player.Player.look;

                    SDG.Unturned.Player? victimPlayer = null;
                    if (Physics.Raycast(new Ray(look.aim.position, look.aim.forward), out RaycastHit hit, 2f, RayMasks.PLAYER))
                    {
                        var victimPlayer2 = hit.transform.GetComponent<SDG.Unturned.Player>();
                        if (victimPlayer2 != null && Vector3.Distance(victimPlayer2.transform.position, player.Position) <= 5f)
                            victimPlayer = victimPlayer2;
                    }

                    if (victimPlayer != null)
                    {
                        UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(victimPlayer);
                        AdvancedHealthComponent playerComp = player.GetComponent<AdvancedHealthComponent>();
                        playerComp.Drag(targetPlayer);
                    }
                }
                else if (gesture == UnturnedPlayerEvents.PlayerGesture.SurrenderStop)
                {
                    AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();

                    if (comp.dragState == EDragState.Dragger)
                        comp.UnDrag();
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerGestureUpdated)}.", ex);
            }
        }
    }
}