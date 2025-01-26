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
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Helpers.Unturned;
using UnityEngine;
using UnityEngine.Serialization;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection TranspConnection => Player.SteamPlayer().transportConnection;
        [FormerlySerializedAs("HasHeavyBleeding")] 
        public bool hasHeavyBleeding;
        public readonly ProgressBarDatas ProgressBarData = new ProgressBarDatas();
        [FormerlySerializedAs("DragState")] 
        public EDragState dragState = EDragState.None;
        [FormerlySerializedAs("DragPartnerId")] 
        public CSteamID dragPartnerId = CSteamID.Nil;
        public HealthData HealthData {  get; set; }

        public Dictionary<ushort, DateTime> LastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        private DateTime _nextHeadHealDate;
        private DateTime _nextBodyHealDate;
        private DateTime _nextArmHealDate;
        private DateTime _nextLegHealDate;


        [FormerlySerializedAs("AllowDamage")] 
        public bool allowDamage;
        [FormerlySerializedAs("LastEquipedItem")] 
        public ushort lastEquipedItem;
        [FormerlySerializedAs("EffectID")] 
        public ushort effectId;
        [FormerlySerializedAs("States")] 
        public List<EPlayerState> states = new List<EPlayerState>();

        /// <summary>
        /// Tries to add the specified player state asynchronously.
        /// </summary>
        /// <param name="state">The player state to add.</param>
        /// <returns>A task representing the asynchronous operation. True if the state was added successfully; otherwise, false.</returns>
        public async Task TryAddStateAsync(EPlayerState state)
        {
            try
            {
                if (!states.Contains(state))
                {
                    var config = AdvancedHealth.Instance.Config;

                    if (state == EPlayerState.NoneTemperature)
                    {
                        StatusIcon icon2 = HealthHelper.GetStatusIcon(EPlayerState.Warm);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
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
                            states.Add(state);
                            await RefreshStateUIAsync();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                            {
                                await TryRemoveStateAsync(ic.Status, false);
                            }

                            states.Add(state);
                            await RefreshStateUIAsync();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Tries to remove the specified player state asynchronously.
        /// </summary>
        /// <param name="state">The player state to remove.</param>
        /// <param name="shouldUpdate">A boolean indicating whether to update after removing the state. Default is true.</param>
        /// <returns>A task representing the asynchronous operation. True if the state was removed successfully; otherwise, false.</returns>
        public async Task TryRemoveStateAsync(EPlayerState state, bool shouldUpdate = true)
        {
            try
            {
                if (states.Contains(state))
                {
                    states.RemoveAt(states.FindIndex(x => x == state));
                    if (shouldUpdate)
                        await RefreshStateUIAsync();
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.LogError(e.ToString());
            }
        }

        /// <summary>
        /// Asynchronously refreshes the user interface (UI) to reflect the current player state.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task RefreshStateUIAsync()
        {
            HealthData health =  await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(Player.Id);
            short effectID = (short)health.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (states.Count - 1 >= i)
                {
                    EPlayerState value = states.ElementAt(i);
                    StatusIcon icon = HealthHelper.GetStatusIcon(value);

                    UEffectHelper.SendUIEffectImageURL(effectID, TranspConnection, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    AdvancedHealth.Instance.InvokeAction(0.1f, () => {
                        if (states.Count >= localuiname)
                            UEffectHelper.SendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, true);
                    });
                }
                else
                {
                    UEffectHelper.SendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, false);
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
            HealthData healthData = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(target.Id);

            if (!healthData.IsInjured || targetComp.dragState != EDragState.None || dragState != EDragState.None)
                return;

            dragPartnerId = target.CSteamID;
            targetComp.dragPartnerId = Player.CSteamID;
            dragState = EDragState.Dragger;
            targetComp.dragState = EDragState.Dragged;
        }

        /// <summary>
        /// Stops dragging the player.
        /// </summary>
        /// <param name="receivedFromPartner">A boolean indicating whether the command to stop dragging was received from a partner. Default is false.</param>
        public void UnDrag(bool receivedFromPartner = false)
        {
            dragState = EDragState.None;
            dragPartnerId = CSteamID.Nil;

            if (!receivedFromPartner)
                return;
            UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);
            if (partner == null)
                return;
            AdvancedHealthComponent partnerComp = partner.GetComponent<AdvancedHealthComponent>();
            if (partnerComp == null)
                return;
            partnerComp.UnDrag();
        }

        /// <summary>
        /// Asynchronously revives the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the revival command was received from a partner. Default is false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task ReviveAsync(bool recievedFromPartner = false)
        {
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;

            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            allowDamage = false;
            hasHeavyBleeding = false;
            
            HealthData health = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(Player.Id);
            await AdvancedHealth.DatabaseManager.UpdateHealthAsync(Player.Id, new HealthData 
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
            });
            Player.Broken = false;
            Player.Bleeding = false;
            Player.Hunger = 0;
            Player.Thirst = 0;
            Player.Infection = 0;
            Player.Heal(100);

            UEffectHelper.SendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }

        /// <summary>
        /// Asynchronously initiates the bleeding out process for the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the command to bleed out was received from a partner. Default is false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task BleedOutAsync(bool recievedFromPartner = false)
        {
            allowDamage = true;
            Player.Player.life.askDamage(100, Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;
            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            await AdvancedHealth.DatabaseManager.UpdateInjuredAsync(Player.Id, false, DateTime.Now);
            UEffectHelper.SendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    
        private void Update()
        {
            // TODO: Replace this with cache
            Task.Run(async () =>
            {
                HealthData = await AdvancedHealth.DatabaseManager.GetPlayerHealthAsync(Player.Id);

                #region Injured

                if (HealthData.IsInjured)
                {
                    Player.Bleeding = false;

                    int secs = (int)(HealthData.DeathDate - DateTime.Now).TotalSeconds;
                    UEffectHelper.SendUIEffectText((short)effectId, TranspConnection, true, "tb_message",
                        AdvancedHealth.Instance.Localize("ui_bleeding", secs.ToString()));
                    if (HealthData.DeathDate < DateTime.Now)
                    {
                        await BleedOutAsync();
                        return;
                    }
                }

                #endregion

                #region Dragging

                if (dragState == EDragState.Dragger && dragPartnerId != CSteamID.Nil)
                {
                    UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);

                    if (partner != null)
                        if (Vector3.Distance(partner.Position, Player.Position) > 3)
                            partner.Player.teleportToPlayer(Player.Player);
                }

                #endregion

                #region Regeneration

                bool shouldUpdateHealth = false;
                bool canRegenerate =
                    Player.Player.life.food >= AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinFood &&
                    Player.Player.life.water >=
                    AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinWater &&
                    Player.Player.life.virus >= AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinVirus;
                EDatabaseEvent databaseEvent = EDatabaseEvent.None;

                // Head
                if (_nextHeadHealDate <= DateTime.Now)
                {
                    if (HealthData.HeadHealth + 1 <= AdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth &&
                        canRegenerate)
                    {
                        HealthData.HeadHealth += 1;
                        shouldUpdateHealth = true;
                        databaseEvent = EDatabaseEvent.Head;
                    }

                    _nextHeadHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.HeadRegenTicks);
                }

                // Body
                if (_nextBodyHealDate <= DateTime.Now)
                {
                    if (HealthData.BodyHealth + 1 <= AdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth &&
                        canRegenerate)
                    {
                        HealthData.BodyHealth += 1;
                        shouldUpdateHealth = true;
                        if (databaseEvent == EDatabaseEvent.None)
                            databaseEvent = EDatabaseEvent.Body;
                        else
                            databaseEvent |= EDatabaseEvent.Body;
                    }

                    _nextBodyHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.BodyRegenTicks);
                }

                // Arm
                if (_nextArmHealDate <= DateTime.Now)
                {
                    if (canRegenerate)
                    {
                        if (HealthData.LeftArmHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.LeftArmHealth)
                        {
                            HealthData.LeftArmHealth += 1;
                            shouldUpdateHealth = true;
                            if (databaseEvent == EDatabaseEvent.None)
                                databaseEvent = EDatabaseEvent.LeftARM;
                            else
                                databaseEvent |= EDatabaseEvent.LeftARM;
                        }

                        if (HealthData.RightArmHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth)
                        {
                            HealthData.RightArmHealth += 1;
                            shouldUpdateHealth = true;
                            if (databaseEvent == EDatabaseEvent.None)
                                databaseEvent = EDatabaseEvent.RightARM;
                            else
                                databaseEvent |= EDatabaseEvent.RightARM;
                        }
                    }

                    _nextArmHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.ArmRegenTicks);
                }

                // Leg
                if (_nextLegHealDate <= DateTime.Now)
                {
                    if (canRegenerate)
                    {
                        if (HealthData.LeftLegHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.LeftLegHealth)
                        {
                            HealthData.LeftLegHealth += 1;
                            shouldUpdateHealth = true;
                            if (databaseEvent == EDatabaseEvent.None)
                                databaseEvent = EDatabaseEvent.LeftLeg;
                            else
                                databaseEvent |= EDatabaseEvent.LeftLeg;
                        }

                        if (HealthData.RightLegHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth)
                        {
                            HealthData.RightLegHealth += 1;
                            shouldUpdateHealth = true;
                            if (databaseEvent == EDatabaseEvent.None)
                                databaseEvent = EDatabaseEvent.RightLeg;
                            else
                                databaseEvent |= EDatabaseEvent.RightLeg;
                        }
                    }

                    _nextLegHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.LegRegenTicks);
                }

                if (shouldUpdateHealth)
                    await AdvancedHealth.DatabaseManager.UpdateHealthAsync(Player.Id, HealthData, databaseEvent);

                #endregion
            });
        }
    }
}
