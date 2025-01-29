using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Utils.Helpers
{
    /// <summary>
    /// A static class providing helper methods related to health management.
    /// </summary>
    public static class HealthHelper
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        /// <summary>
        /// Asynchronously sets the specified player as downed in the game.
        /// </summary>
        /// <param name="player">The Unturned player to be set as downed.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public static async Task SetPlayerDownedAsync(UnturnedPlayer player)
        {
            string methodName = "SetPlayerDownedAsync";
            try
            {
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                var transCon = player.SteamPlayer().transportConnection;
                HealthData healthData = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(player.Id);
                if (!healthData.IsInjured)
                {
                    if (player.Dead) { return; }
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
                    await AdvancedHealth.DatabaseManager.UpdateHealthAsync(player.Id, healthData);


                    player.Player.stance.checkStance(EPlayerStance.PRONE, true);
                    player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, true);

                    UEffectHelper.SendUIEffectVisibility((short)comp.effectId, transCon, true, "bt_suicide", true);
                    UEffectHelper.SendUIEffectText((short)comp.effectId, transCon, true, "tb_message", AdvancedHealth.Instance.Localize("ui_bleeding", (int)(healthData.DeathDate - DateTime.Now).TotalSeconds));
                    UEffectHelper.SendUIEffectVisibility((short)comp.effectId, transCon, true, "RevivePanel", true);
                    foreach (SteamPlayer sp in Provider.clients)
                    {
                        UnturnedPlayer tmpPlayer = UnturnedPlayer.FromSteamPlayer(sp);
                        if ((tmpPlayer.HasPermission(_config.DefibrillatorSettings.PermissionForUseDefiblirator) || tmpPlayer.CSteamID == player.CSteamID) && !player.IsAdmin)
                        {
                            var teleportLocation = new Vector3(player.Position.x, player.Position.y, player.Position.z);
                            tmpPlayer.Player.quests.sendSetMarker(true, teleportLocation);
                            AdvancedHealth.Instance.SendChatMessage(sp, "player_injured", player.CharacterName);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.Log($"Error in {methodName}: {e}");
            }
        }

        /// <summary>
        /// Determines whether an entity with the given healthData can bleed after receiving the specified amount of damage.
        /// </summary>
        /// <param name="health">The current healthData of the entity.</param>
        /// <param name="damage">The amount of damage the entity is about to receive.</param>
        /// <returns>True if the entity can bleed after taking the damage; otherwise, false.</returns>
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

        /// <summary>
        /// Retrieves the status icon associated with the specified player state.
        /// </summary>
        /// <param name="state">The player state for which to retrieve the status icon.</param>
        /// <returns>The status icon corresponding to the specified player state.</returns>
        public static StatusIcon GetStatusIcon(EPlayerState state)
        {
            return _config.HealthSystemSettings.StatusIcons.Find(x => x.Status == state);
        }
    }
}
