using HarmonyLib;
using Newtonsoft.Json.Linq;
using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Unturned.Player;
using SDG.Framework.Utilities;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Helpers;
using Tavstal.TAdvancedHealth.Managers;
using Tavstal.TAdvancedHealth.Modules;
using UnityEngine;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;

namespace Tavstal.TAdvancedHealth.Compatibility.Harmony
{
    [HarmonyPatch(typeof(UseableMelee), "fire", new Type[] { })]
    public static class UseableMeleeHarmony
    {
        private static TAdvancedHealthConfig Config => TAdvancedHealthMain.Instance.Configuration.Instance;
        private static DatabaseManager Database => TAdvancedHealthMain.Database;

        [HarmonyPostfix]
        public static void Postfix(object __instance)
        {
            UseableMelee useableMelee = (UseableMelee)__instance;
            UnturnedPlayer userPlayer = UnturnedPlayer.FromPlayer(useableMelee.player);
            Defibrillator defibrillator = Config.DefibrillatorSettings.DefibrillatorItems.Find(x => x.ItemID == useableMelee.equippedMeleeAsset.id);
            if (!Config.DefibrillatorSettings.Enabled || (Config.DefibrillatorSettings.Enabled && !userPlayer.HasPermission(Config.DefibrillatorSettings.PermissionForUseDefiblirator)) || defibrillator == null)
                return;

            TAdvancedHealthComponent userComp = userPlayer.GetComponent<TAdvancedHealthComponent>();
            if (userComp.lastDefibliratorUses.TryGetValue(useableMelee.equippedMeleeAsset.id, out DateTime time))
                if (time > DateTime.Now)
                {
                    UnturnedHelper.SendChatMessage(userPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_cooldown", (time - DateTime.Now).TotalSeconds.ToString("0.00")));
                    return;
                }
                else
                    userComp.lastDefibliratorUses.Remove(useableMelee.equippedMeleeAsset.id);

            var userLook = userPlayer.Player.look;
            Player targetBasePlayer = null;
            if (Physics.Raycast(new Ray(userLook.aim.position, userLook.aim.forward), out RaycastHit hit, 2f, RayMasks.PLAYER))
                targetBasePlayer = hit.transform.GetComponent<Player>();

            if (targetBasePlayer != null)
            {
                UnturnedPlayer targetPlayer = UnturnedPlayer.FromPlayer(targetBasePlayer);
                TAdvancedHealthComponent targetComp = targetPlayer.GetComponent<TAdvancedHealthComponent>();

                PlayerHealth targetHealth = Database.GetPlayerHealth(targetPlayer.Id);
                if (targetHealth.isInjured)
                {
                    int chance = MathHelper.GenerateRandomNumber(1, 100);
                    if (chance != 0 && chance <= defibrillator.ReviveChance)
                    {
                        targetComp.Revive();
                        UnturnedHelper.SendChatMessage(userPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "success_defiblirator_revive", targetPlayer.CharacterName));
                        UnturnedHelper.SendChatMessage(targetPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "success_defiblirator_revive_other", userPlayer.CharacterName));
                    }
                    else
                    {
                        UnturnedHelper.SendChatMessage(userPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_revive_fail", targetPlayer.CharacterName));
                        //Helper.SendChatMessage(targetPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_revive_fail_other"));
                    }
                    userComp.lastDefibliratorUses.Add(useableMelee.equippedMeleeAsset.id, DateTime.Now.AddSeconds(defibrillator.RechargeTimeSecs));
                }
                else
                    UnturnedHelper.SendChatMessage(userPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_not_injured"));
            }
            else
                UnturnedHelper.SendChatMessage(userPlayer.SteamPlayer(), TAdvancedHealthMain.Instance.Translate(true, "error_defiblirator_no_player"));
        }
    }
}
