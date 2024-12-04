using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Globalization;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;

namespace Tavstal.TAdvancedHealth.Utils.Handlers
{
    /// <summary>
    /// Provides event handling functionality for the health system.
    /// </summary>
    public static class HealthSystemEventHandler
    {
        private static AdvancedHealthConfig Config => AdvancedHealth.Instance.Config;

        /// <summary>
        /// Attaches event listeners to the health system to enable event handling.
        /// </summary>
        internal static void Attach()
        {
            EventManager.OnBaseHealthUpdated += Event_OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated += Event_OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated += Event_OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated += Event_OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated += Event_OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated += Event_OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated += Event_OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated += Event_OnInjuredStateUpdated;
        }

        /// <summary>
        /// Detaches event listeners from the health system to disable event handling.
        /// </summary>
        internal static void Dettach()
        {
            EventManager.OnBaseHealthUpdated -= Event_OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated -= Event_OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated -= Event_OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated -= Event_OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated -= Event_OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated -= Event_OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated -= Event_OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated -= Event_OnInjuredStateUpdated;
        }

        /// <summary>
        /// Event handler for updates to the injured state of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="isInjured">A boolean indicating whether the player is currently injured.</param>
        /// <param name="bleedDate">The date and time when the player started bleeding (if applicable).</param>
        private static void Event_OnInjuredStateUpdated(string id, bool isInjured, DateTime bleedDate)
        {
            
        }

        /// <summary>
        /// Event handler for updates to the health of the left leg of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the left leg.</param>
        private static async void Event_OnLeftLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftLegHealth, 
                (int)(newHealth / Config.HealthSystemSettings.LeftLegHealth * 100), (int)comp.ProgressBarData.LastHealthLeftLeg);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_LeftLeg", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the left arm of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the left arm.</param>
        private static async void Event_OnLeftArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftArmHealth,
                (int)(newHealth / Config.HealthSystemSettings.LeftArmHealth * 100), (int)comp.ProgressBarData.LastHealthLeftArm);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_LeftArm", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the right leg of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the right leg.</param>
        private static async void Event_OnRightLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightLegHealth,
                (int)(newHealth / Config.HealthSystemSettings.RightLegHealth * 100), (int)comp.ProgressBarData.LastHealthRightLeg);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_RightLeg", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the right arm of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the right arm.</param>
        private static async void Event_OnRightArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightArmHealth,
                (int)(newHealth / Config.HealthSystemSettings.RightArmHealth * 100), (int)comp.ProgressBarData.LastHealthRightArm);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_RightArm", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the body of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the body.</param>
        private static async void Event_OnBodyHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.BodyHealth,
                (int)(newHealth / Config.HealthSystemSettings.BodyHealth * 100), (int)comp.ProgressBarData.LastHealthBody);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Body", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the head of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the head.</param>
        private static async void Event_OnHeadHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.HeadHealth,
                (int)(newHealth / Config.HealthSystemSettings.HeadHealth * 100), (int)comp.ProgressBarData.LastHealthHead);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Head", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value.</param>
        private static async void Event_OnBaseHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.SimpleHealth,
                (int)(newHealth / Config.HealthSystemSettings.BaseHealth * 100), (int)comp.ProgressBarData.LastSimpleHealth);
            EffectManager.sendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Health", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }
    }
}
