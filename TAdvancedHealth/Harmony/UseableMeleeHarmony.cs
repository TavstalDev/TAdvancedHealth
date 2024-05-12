using HarmonyLib;
using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Helpers.Unturned;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Utils.Managers;
using UnityEngine;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Components;

namespace Tavstal.TAdvancedHealth.Harmony
{
    [HarmonyPatch(typeof(UseableMelee), "fire", new Type[] { })]
    public static class UseableMeleeHarmony
    {
        private static TAdvancedHealthConfig _config => TAdvancedHealth.Instance.Config;
        private static DatabaseManager _database => TAdvancedHealth.Database;

        [HarmonyPostfix]
        public static async void Postfix(object __instance)
        {
            UseableMelee useableMelee = (UseableMelee)__instance;
            UnturnedPlayer userPlayer = UnturnedPlayer.FromPlayer(useableMelee.player);
            Defibrillator defibrillator = _config.DefibrillatorSettings.DefibrillatorItems.Find(x => x.ItemID == useableMelee.equippedMeleeAsset.id);
            if (!_config.DefibrillatorSettings.Enabled || (_config.DefibrillatorSettings.Enabled && !userPlayer.HasPermission(_config.DefibrillatorSettings.PermissionForUseDefiblirator)) || defibrillator == null)
                return;

            AdvancedHealthComponent userComp = userPlayer.GetComponent<AdvancedHealthComponent>();
            if (userComp.LastDefibliratorUses.TryGetValue(useableMelee.equippedMeleeAsset.id, out DateTime time))
                if (time > DateTime.Now)
                {
                    TAdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "error_defiblirator_cooldown", (time - DateTime.Now).TotalSeconds.ToString("0.00"));
                    return;
                }
                else
                    userComp.LastDefibliratorUses.Remove(useableMelee.equippedMeleeAsset.id);

            var userLook = userPlayer.Player.look;
            Player targetBasePlayer = null;
            if (Physics.Raycast(new Ray(userLook.aim.position, userLook.aim.forward), out RaycastHit hit, 2.5f, RayMasks.PLAYER))
                targetBasePlayer = hit.transform.GetComponent<Player>();

            if (targetBasePlayer != null)
            {
                TAdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "error_defiblirator_no_player");
                return;
            }

            UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(targetBasePlayer);
            AdvancedHealthComponent targetComp = targetPlayer.GetComponent<AdvancedHealthComponent>();

            HealthData targetHealth = await _database.GetPlayerHealthAsync(targetPlayer.Id);
            if (targetHealth.IsInjured)
            {
                int chance = MathHelper.Next(1, 100);
                if (chance != 0 && chance <= defibrillator.ReviveChance)
                {
                    await targetComp.ReviveAsync();
                    TAdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "success_defiblirator_revive", targetPlayer.CharacterName);
                    TAdvancedHealth.Instance.SendChatMessage(targetPlayer.SteamPlayer(), "success_defiblirator_revive_other", userPlayer.CharacterName);
                }
                else
                {
                    TAdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "error_defiblirator_revive_fail", targetPlayer.CharacterName);
                }
                userComp.LastDefibliratorUses.Add(useableMelee.equippedMeleeAsset.id, DateTime.Now.AddSeconds(defibrillator.RechargeTimeSecs));
            }
            else
                TAdvancedHealth.Instance.SendChatMessage(userPlayer.SteamPlayer(), "error_defiblirator_not_injured");

        }
    }
}
