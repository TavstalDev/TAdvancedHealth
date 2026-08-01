using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using HarmonyLib;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace Tavstal.TAdvancedHealth.Harmony
{
    [HarmonyPatch(typeof(UseableMelee), "fire", new Type[] { })]
    public static class UseableMeleeHarmony
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        [HarmonyPostfix]
        // ReSharper disable once InconsistentNaming
        public static void Postfix(object __instance)
        {
            UseableMelee useableMelee = (UseableMelee)__instance;
            UnturnedPlayer userPlayer = UnturnedPlayer.FromPlayer(useableMelee.player);
            Defibrillator defibrillator = _config.DefibrillatorSettings.Items.Find(x => x.ItemID == useableMelee.equippedMeleeAsset.id);
            if (!_config.DefibrillatorSettings.Enable || (_config.DefibrillatorSettings.Enable && !userPlayer.HasPermission(_config.DefibrillatorSettings.Permission)) || defibrillator == null)
                return;

            AdvancedHealthComponent userComp = ComponentManager.Get(userPlayer);
            if (userComp.LastDefibliratorUses.TryGetValue(useableMelee.equippedMeleeAsset.id, out DateTime time))
                if (time > DateTime.Now)
                {
                    AdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "defibrillator_error_cooldown", (time - DateTime.Now).TotalSeconds.ToString("0.00"));
                    return;
                }
                else
                    userComp.LastDefibliratorUses.Remove(useableMelee.equippedMeleeAsset.id);

            var userLook = userPlayer.Player.look;
            Player? targetBasePlayer = null;
            if (Physics.Raycast(new Ray(userLook.aim.position, userLook.aim.forward), out RaycastHit hit, 2.5f, RayMasks.PLAYER))
                targetBasePlayer = hit.transform.GetComponent<Player>();

            if (targetBasePlayer != null)
            {
                AdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "defibrillator_error_too_far");
                return;
            }

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(targetBasePlayer);
            AdvancedHealthComponent targetComp = ComponentManager.Get(targetPlayer);

            var targetHealth = targetComp.HealthData;
            if (targetHealth is { IsInjured: false })
            {
                AdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "defibrillator_error_healthy");
                return;
            }

            int chance = MathHelper.Next(1, 100);
            if (chance != 0 && chance <= defibrillator.ReviveChance)
            { 
                targetComp.Revive();
                AdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "defibrillator_revive",
                    targetPlayer.CharacterName);
                AdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(),
                    "defibrillator_revive_other", userPlayer.CharacterName);
            }
            else
            {
                AdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "defibrillator_error_fail",
                    targetPlayer.CharacterName);
            }

            userComp.LastDefibliratorUses.Add(useableMelee.equippedMeleeAsset.id,
                DateTime.Now.AddSeconds(defibrillator.RechargeTimeSecs));
        }
    }
}
