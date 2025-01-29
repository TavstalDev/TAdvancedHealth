using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Globalization;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Helpers.Unturned;

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
            EventManager.OnBaseHealthUpdated += OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated += OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated += OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated += OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated += OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated += OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated += OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated += OnInjuredStateUpdated;
        }

        /// <summary>
        /// Detaches event listeners from the health system to disable event handling.
        /// </summary>
        internal static void Detach()
        {
            EventManager.OnBaseHealthUpdated -= OnBaseHealthUpdated;
            EventManager.OnHeadHealthUpdated -= OnHeadHealthUpdated;
            EventManager.OnBodyHealthUpdated -= OnBodyHealthUpdated;
            EventManager.OnRightArmHealthUpdated -= OnRightArmUpdated;
            EventManager.OnRightLegHealthUpdated -= OnRightLegUpdated;
            EventManager.OnLeftArmHealthUpdated -= OnLeftArmUpdated;
            EventManager.OnLeftLegHealthUpdated -= OnLeftLegUpdated;
            EventManager.OnInjuredStateUpdated -= OnInjuredStateUpdated;
        }

        /// <summary>
        /// Event handler for updates to the injured state of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="isInjured">A boolean indicating whether the player is currently injured.</param>
        /// <param name="bleedDate">The date and time when the player started bleeding (if applicable).</param>
        private static void OnInjuredStateUpdated(string id, bool isInjured, DateTime bleedDate)
        {
            
        }

        /// <summary>
        /// Event handler for updates to the health of the left leg of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the left leg.</param>
        private static void OnLeftLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () => await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftLegHealth, 
                (int)(newHealth / Config.HealthSystemSettings.LeftLegHealth * 100), (int)comp.ProgressBarData.LastHealthLeftLeg));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_LeftLeg", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the left arm of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the left arm.</param>
        private static void OnLeftArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () => await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.LeftArmHealth,
                (int)(newHealth / Config.HealthSystemSettings.LeftArmHealth * 100), (int)comp.ProgressBarData.LastHealthLeftArm));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_LeftArm", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the right leg of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the right leg.</param>
        private static void OnRightLegUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () => await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightLegHealth,
                (int)(newHealth / Config.HealthSystemSettings.RightLegHealth * 100), (int)comp.ProgressBarData.LastHealthRightLeg));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_RightLeg", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the right arm of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the right arm.</param>
        private static void OnRightArmUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () =>await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.RightArmHealth,
                (int)(newHealth / Config.HealthSystemSettings.RightArmHealth * 100), (int)comp.ProgressBarData.LastHealthRightArm));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_RightArm", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the body of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the body.</param>
        private static void OnBodyHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () =>await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.BodyHealth,
                (int)(newHealth / Config.HealthSystemSettings.BodyHealth * 100), (int)comp.ProgressBarData.LastHealthBody));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Body", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the head of a player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value of the head.</param>
        private static void OnHeadHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () =>await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.HeadHealth,
                (int)(newHealth / Config.HealthSystemSettings.HeadHealth * 100), (int)comp.ProgressBarData.LastHealthHead));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Head", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Event handler for updates to the health of the player.
        /// </summary>
        /// <param name="id">The unique identifier of the player.</param>
        /// <param name="newHealth">The new health value.</param>
        private static void OnBaseHealthUpdated(string id, float newHealth)
        {
            UnturnedPlayer player = UnturnedPlayer.FromCSteamID((CSteamID)Convert.ToUInt64(id));
            AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
            Task.Run(async () =>await EffectHelper.SendUIEffectProgressBarAsync((short)comp.effectId, player.CSteamID, true, EProgressBar.SimpleHealth,
                (int)(newHealth / Config.HealthSystemSettings.BaseHealth * 100), (int)comp.ProgressBarData.LastSimpleHealth));
            UEffectHelper.SendUIEffectText((short)comp.effectId, comp.TranspConnection, true, "tb_Health", Math.Round(newHealth, 0).ToString(CultureInfo.CurrentCulture));
        }
    }
}
