using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enums;
using Tavstal.TAdvancedHealth.Utils.Helpers;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection TranspConnection => Player.SteamPlayer().transportConnection;
        public bool hasHeavyBleeding = false;
        public ProgressBarDatas progressBarData = new ProgressBarDatas();
        public EDragState DragState = EDragState.NONE;
        public CSteamID DragPartnerId = CSteamID.Nil;

        public Dictionary<ushort, DateTime> LastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        public DateTime NextHeadHealDate { get; set; }
        public DateTime NextBodyHealDate { get; set; }
        public DateTime NextArmHealDate { get; set; }
        public DateTime NextLegHealDate { get; set; }


        public bool AllowDamage = false;
        public ushort LastEquipedItem = 0;
        public ushort EffectID = 0;
        public List<EPlayerStates> States = new List<EPlayerStates>();

        /// <summary>
        /// Tries to add the specified player state asynchronously.
        /// </summary>
        /// <param name="state">The player state to add.</param>
        /// <returns>A task representing the asynchronous operation. True if the state was added successfully; otherwise, false.</returns>
        public async Task TryAddStateAsync(EPlayerStates state)
        {
            try
            {
                if (!States.Contains(state))
                {
                    var config = TAdvancedHealth.Instance.Config;

                    if (state == EPlayerStates.NONE_TEMPERATURE)
                    {
                        StatusIcon icon2 = HealthHelper.GetStatusIcon(EPlayerStates.WARM);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.statusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                await  TryRemoveStateAsync(ic.Status, false);
                            }
                            await RefreshStateUIAsync();
                        }
                        return;
                    }

                    StatusIcon icon = HealthHelper.GetStatusIcon(state);

                    if (icon != null)
                    {
                        if (icon.GroupIndex == -1)
                        {
                            States.Add(state);
                            await RefreshStateUIAsync();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.statusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                await TryRemoveStateAsync(ic.Status, false);
                            }

                            States.Add(state);
                            await RefreshStateUIAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Tries to remove the specified player state asynchronously.
        /// </summary>
        /// <param name="state">The player state to remove.</param>
        /// <param name="shouldUpdate">A boolean indicating whether to update after removing the state. Default is true.</param>
        /// <returns>A task representing the asynchronous operation. True if the state was removed successfully; otherwise, false.</returns>
        public async Task TryRemoveStateAsync(EPlayerStates state, bool shouldUpdate = true)
        {
            try
            {
                if (States.Contains(state))
                {
                    States.RemoveAt(States.FindIndex(x => x == state));
                    if (shouldUpdate)
                        await RefreshStateUIAsync();
                }
            }
            catch (Exception e)
            {
                TAdvancedHealth.Logger.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Asynchronously refreshes the user interface (UI) to reflect the current player state.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task RefreshStateUIAsync()
        {
            HealthData health =  await TAdvancedHealth.Database.GetPlayerHealthAsync(this.Player.Id);
            short effectID = (short)health.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (States.Count - 1 >= i)
                {
                    EPlayerStates value = States.ElementAt(i);
                    StatusIcon icon = HealthHelper.GetStatusIcon(value);

                    EffectManager.sendUIEffectImageURL(effectID, this.Player.CSteamID, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    TAdvancedHealth.Instance.InvokeAction(0.1f, () => {
                        if (States.Count >= localuiname)
                            EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, true);
                    });
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, false);
                }
            }
        }

        /// <summary>
        /// Asynchronously drags the specified target Unturned player.
        /// </summary>
        /// <param name="target">The Unturned player to be dragged.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task DragAsync(UnturnedPlayer target)
        {
            AdvancedHealthComponent targetComp = target.GetComponent<AdvancedHealthComponent>();
            HealthData healthData = await TAdvancedHealth.Database.GetPlayerHealthAsync(target.Id);

            if (healthData.IsInjured || targetComp.DragState != EDragState.NONE || !healthData.IsInjured || DragState != EDragState.NONE)
                return;

            DragPartnerId = target.CSteamID;
            targetComp.DragPartnerId = this.Player.CSteamID;
            DragState = EDragState.DRAGGER;
            targetComp.DragState = EDragState.DRAGGED;
        }

        /// <summary>
        /// Stops dragging the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the command to stop dragging was received from a partner. Default is false.</param>
        public void UnDrag(bool recievedFromPartner = false)
        {
            if (recievedFromPartner)
            {
                UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(DragPartnerId);
                if (partner != null)
                {
                    AdvancedHealthComponent partnerComp = partner.GetComponent<AdvancedHealthComponent>();
                    if (partnerComp != null)
                        partnerComp.UnDrag();
                }
            }

            DragState = EDragState.NONE;
            DragPartnerId = CSteamID.Nil;
        }

        /// <summary>
        /// Asynchronously revives the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the revival command was received from a partner. Default is false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReviveAsync(bool recievedFromPartner = false)
        {
            var chsettings = TAdvancedHealth.Instance.CHSCSettings;

            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            AllowDamage = false;
            hasHeavyBleeding = false;
            
            HealthData health = await TAdvancedHealth.Database.GetPlayerHealthAsync(this.Player.Id);
            await TAdvancedHealth.Database.UpdateHealthAsync(this.Player.Id, new HealthData 
            {
                BaseHealth = chsettings.BaseHealth,
                BodyHealth = chsettings.BodyHealth,
                HeadHealth = chsettings.HeadHealth,
                RightArmHealth = chsettings.RightArmHealth,
                LeftArmHealth = chsettings.LeftArmHealth,
                LeftLegHealth = chsettings.RightLegHealth,
                RightLegHealth = chsettings.LeftLegHealth,
                DeathDate = health.DeathDate,
                HUDEffectID = health.HUDEffectID,
                IsHUDEnabled = health.IsHUDEnabled,
                IsInjured = false,
                PlayerId = health.PlayerId
            }, EDatabaseEvent.ALL);
            this.Player.Broken = false;
            this.Player.Bleeding = false;
            this.Player.Hunger = 0;
            this.Player.Thirst = 0;
            this.Player.Infection = 0;
            this.Player.Heal(100);

            EffectManager.sendUIEffectVisibility((short)EffectID, TranspConnection, true, "RevivePanel", false);
            this.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }

        /// <summary>
        /// Asynchronously initiates the bleeding out process for the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the command to bleed out was received from a partner. Default is false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task BleedOutAsync(bool recievedFromPartner = false)
        {
            AllowDamage = true;
            this.Player.Player.life.askDamage(100, this.Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out EPlayerKill outKill);
            var chsettings = TAdvancedHealth.Instance.CHSCSettings;
            if (this.Player.Player.movement.pluginSpeedMultiplier != chsettings.DefaultWalkSpeed)
                this.Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            this.Player.Player.movement.sendPluginJumpMultiplier(1f);
            await TAdvancedHealth.Database.UpdateInjuredAsync(this.Player.Id, false, DateTime.Now);
            EffectManager.sendUIEffectVisibility((short)EffectID, TranspConnection, true, "RevivePanel", false);
            this.Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    }
}
