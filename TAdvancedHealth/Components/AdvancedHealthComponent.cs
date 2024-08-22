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
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Components
{
    public class AdvancedHealthComponent : UnturnedPlayerComponent
    {
        public ITransportConnection TranspConnection => Player.SteamPlayer().transportConnection;
        public bool HasHeavyBleeding;
        public ProgressBarDatas ProgressBarData = new ProgressBarDatas();
        public EDragState DragState = EDragState.NONE;
        public CSteamID DragPartnerId = CSteamID.Nil;
        public HealthData HealthData {  get; set; }

        public Dictionary<ushort, DateTime> LastDefibliratorUses { get; set; } = new Dictionary<ushort, DateTime>();
        private DateTime _nextHeadHealDate { get; set; }
        private DateTime _nextBodyHealDate { get; set; }
        private DateTime _nextArmHealDate { get; set; }
        private DateTime _nextLegHealDate { get; set; }


        public bool AllowDamage;
        public ushort LastEquipedItem;
        public ushort EffectID;
        public List<EPlayerState> States = new List<EPlayerState>();

        /// <summary>
        /// Tries to add the specified player state asynchronously.
        /// </summary>
        /// <param name="state">The player state to add.</param>
        /// <returns>A task representing the asynchronous operation. True if the state was added successfully; otherwise, false.</returns>
        public async Task TryAddStateAsync(EPlayerState state)
        {
            try
            {
                if (!States.Contains(state))
                {
                    var config = TAdvancedHealth.Instance.Config;

                    if (state == EPlayerState.NONE_TEMPERATURE)
                    {
                        StatusIcon icon2 = HealthHelper.GetStatusIcon(EPlayerState.WARM);
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
                            States.Add(state);
                            await RefreshStateUIAsync();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
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
        public async Task TryRemoveStateAsync(EPlayerState state, bool shouldUpdate = true)
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
            HealthData health =  await TAdvancedHealth.Database.GetPlayerHealthAsync(Player.Id);
            short effectID = (short)health.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (States.Count - 1 >= i)
                {
                    EPlayerState value = States.ElementAt(i);
                    StatusIcon icon = HealthHelper.GetStatusIcon(value);

                    EffectManager.sendUIEffectImageURL(effectID, Player.CSteamID, true, "Status#" + localuiname + "_img", icon.IconUrl);
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
            targetComp.DragPartnerId = Player.CSteamID;
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
            var chsettings = TAdvancedHealth.Instance.Config.HealthSystemSettings;

            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            AllowDamage = false;
            HasHeavyBleeding = false;
            
            HealthData health = await TAdvancedHealth.Database.GetPlayerHealthAsync(Player.Id);
            await TAdvancedHealth.Database.UpdateHealthAsync(Player.Id, new HealthData 
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

            EffectManager.sendUIEffectVisibility((short)EffectID, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }

        /// <summary>
        /// Asynchronously initiates the bleeding out process for the player.
        /// </summary>
        /// <param name="recievedFromPartner">A boolean indicating whether the command to bleed out was received from a partner. Default is false.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task BleedOutAsync(bool recievedFromPartner = false)
        {
            AllowDamage = true;
            Player.Player.life.askDamage(100, Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);
            var chsettings = TAdvancedHealth.Instance.Config.HealthSystemSettings;
            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            await TAdvancedHealth.Database.UpdateInjuredAsync(Player.Id, false, DateTime.Now);
            EffectManager.sendUIEffectVisibility((short)EffectID, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    
        private async void Update()
        {
            // TODO: Replace this with cache
            HealthData = await TAdvancedHealth.Database.GetPlayerHealthAsync(Player.Id);

            #region Injured
            if (HealthData.IsInjured)
            {
                Player.Bleeding = false;

                int secs = (int)(HealthData.DeathDate - DateTime.Now).TotalSeconds;
                EffectManager.sendUIEffectText((short)EffectID, TranspConnection, true, "tb_message", TAdvancedHealth.Instance.Localize("ui_bleeding", secs.ToString()));
                if (HealthData.DeathDate < DateTime.Now)
                {
                    await BleedOutAsync();
                    return;
                }
            }
            #endregion

            #region Dragging
            if (DragState == EDragState.DRAGGER && DragPartnerId != CSteamID.Nil)
            {
                UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(DragPartnerId);

                if (partner != null)
                    if (Vector3.Distance(partner.Position, Player.Position) > 3)
                        partner.Player.teleportToPlayer(Player.Player);
            }
            #endregion

            #region Regeneration
            bool shouldUpdateHealth = false;
            bool canRegenerate = Player.Player.life.food >= TAdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinFood && Player.Player.life.water >= TAdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinWater && Player.Player.life.virus >= TAdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinVirus;
            EDatabaseEvent databaseEvent = EDatabaseEvent.NONE;

            // Head
            if (_nextHeadHealDate <= DateTime.Now)
            {
                if (HealthData.HeadHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth && canRegenerate)
                {
                    HealthData.HeadHealth += 1;
                    shouldUpdateHealth = true;
                    databaseEvent = EDatabaseEvent.HEAD;
                }
                _nextHeadHealDate = DateTime.Now.AddSeconds(TAdvancedHealth.Instance.Config.HealthSystemSettings.HeadRegenTicks);
            }

            // Body
            if (_nextBodyHealDate <= DateTime.Now)
            {
                if (HealthData.BodyHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth && canRegenerate)
                {
                    HealthData.BodyHealth += 1;
                    shouldUpdateHealth = true;
                    if (databaseEvent == EDatabaseEvent.NONE)
                        databaseEvent = EDatabaseEvent.BODY;
                    else
                        databaseEvent |= EDatabaseEvent.BODY;
                }
                _nextBodyHealDate = DateTime.Now.AddSeconds(TAdvancedHealth.Instance.Config.HealthSystemSettings.BodyRegenTicks);
            }

            // Arm
            if (_nextArmHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                {
                    if (HealthData.LeftArmHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.LeftArmHealth)
                    {
                        HealthData.LeftArmHealth += 1;
                        shouldUpdateHealth = true;
                        if (databaseEvent == EDatabaseEvent.NONE)
                            databaseEvent = EDatabaseEvent.LEFT_ARM;
                        else
                            databaseEvent |= EDatabaseEvent.LEFT_ARM;
                    }
                    if (HealthData.RightArmHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth)
                    {
                        HealthData.RightArmHealth += 1;
                        shouldUpdateHealth = true;
                        if (databaseEvent == EDatabaseEvent.NONE)
                            databaseEvent = EDatabaseEvent.RIGHT_ARM;
                        else
                            databaseEvent |= EDatabaseEvent.RIGHT_ARM;
                    }
                }
                _nextArmHealDate = DateTime.Now.AddSeconds(TAdvancedHealth.Instance.Config.HealthSystemSettings.ArmRegenTicks);
            }

            // Leg
            if (_nextLegHealDate <= DateTime.Now)
            {
                if (canRegenerate)
                {
                    if (HealthData.LeftLegHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.LeftLegHealth)
                    {
                        HealthData.LeftLegHealth += 1;
                        shouldUpdateHealth = true;
                        if (databaseEvent == EDatabaseEvent.NONE)
                            databaseEvent = EDatabaseEvent.LEFT_LEG;
                        else
                            databaseEvent |= EDatabaseEvent.LEFT_LEG;
                    }
                    if (HealthData.RightLegHealth + 1 <= TAdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth)
                    {
                        HealthData.RightLegHealth += 1;
                        shouldUpdateHealth = true;
                        if (databaseEvent == EDatabaseEvent.NONE)
                            databaseEvent = EDatabaseEvent.RIGHT_LEG;
                        else
                            databaseEvent |= EDatabaseEvent.RIGHT_LEG;
                    }
                }
                _nextLegHealDate = DateTime.Now.AddSeconds(TAdvancedHealth.Instance.Config.HealthSystemSettings.LegRegenTicks);
            }

            if (shouldUpdateHealth)
                await TAdvancedHealth.Database.UpdateHealthAsync(Player.Id, HealthData, databaseEvent);
            #endregion
        }
    }
}
