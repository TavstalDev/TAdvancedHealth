using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    public static class HealthHelper
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        public static void SetPlayerDowned(UnturnedPlayer player)
        {
            string methodName = "SetPlayerDownedAsync";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var transCon = player.SteamPlayer().transportConnection;
                HealthData? healthData = comp.HealthData;
                if (healthData == null)
                    return;
                
                if (!healthData.IsInjured)
                    return;

                if (player.Dead)
                    return;

                player.Player.equipment.dequip();
                if (player.Infection > 25)
                    player.Infection = 0;
                player.Heal(100, false, false);
                player.Bleeding = false;
                player.Broken = true;
                if (player.Hunger < 50)
                    player.Hunger = 50;
                if (player.Thirst < 50)
                    player.Thirst = 50;
                player.Player.movement.sendPluginSpeedMultiplier(0f);
                player.Player.movement.sendPluginJumpMultiplier(0f);

                healthData.DeathDate = DateTime.Now.AddSeconds(_config.HealthSystemSettings.InjuredDeathTimeSecs);
                healthData.IsInjured = true;
                healthData.HeadHealth = _config.HealthSystemSettings.HeadHealth;
                healthData.BodyHealth = _config.HealthSystemSettings.BodyHealth;
                healthData.LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth;
                healthData.RightArmHealth = _config.HealthSystemSettings.RightArmHealth;
                healthData.LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth;
                healthData.RightLegHealth = _config.HealthSystemSettings.RightLegHealth;

                player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

                EffectManager.sendUIEffectVisibility((short)comp.effectId, transCon, true, "bt_suicide", true);
                EffectManager.sendUIEffectText((short)comp.effectId, transCon, true, "tb_message",
                    AdvancedHealth.Instance.Localize("ui_bleeding",
                        (int)(healthData.DeathDate - DateTime.Now).TotalSeconds));
                EffectManager.sendUIEffectVisibility((short)comp.effectId, transCon, true, "RevivePanel", true);
                foreach (SteamPlayer sp in Provider.clients)
                {
                    UnturnedPlayer tmpPlayer = UnturnedPlayer.FromSteamPlayer(sp);
                    if ((tmpPlayer.HasPermission(_config.DefibrillatorSettings.PermissionForUseDefiblirator) ||
                         tmpPlayer.CSteamID == player.CSteamID) && !player.IsAdmin)
                    {
                        var teleportLocation = new Vector3(player.Position.x, player.Position.y, player.Position.z);
                        tmpPlayer.Player.quests.sendSetMarker(true, teleportLocation);
                        AdvancedHealth.Instance.SendChatMessage(sp, "player_injured", player.CharacterName);
                    }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {methodName}.", ex);
            }
        }
        
        public static bool CanBleed(float health, float damage)
        {
            bool can = false;

            if (health != 0 && damage != 0 && _config.HealthSystemSettings.CanStartBleeding)
            {
                if (health / 100 * 20 <= damage)
                    can = true;
            }

            return can;
        }
        
        public static StatusIcon? GetStatusIcon(EPlayerState state) =>
            _config.HealthSystemSettings.StatusIcons.Find(x => x.Status == state);
    }
}
