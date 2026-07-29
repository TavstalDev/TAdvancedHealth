using Rocket.Unturned.Player;
using SDG.NetTransport;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TAdvancedHealth.Utils.Helpers;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Threading;
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
        public Health? HealthData {  get; set; }

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

        protected override void Load()
        {
            BackgroundThreadDispatcher.Run(async () =>
            {
                try
                {
                    HealthData = await AdvancedHealth.DatabaseManager.HealthData.GetAsync(Player.Id);
                    if (HealthData == null)
                    {
                        var cshSettings = AdvancedHealth.Instance.Config.HealthSystemSettings;
                        HUDStyle style = AdvancedHealth.Instance.Config.HUDStyles.FirstOrDefault(x => x.Enabled) ??
                                          AdvancedHealth.Instance.Config.HUDStyles[0];

                        HealthData = new HealthData
                        {
                            PlayerId = Player.Id,
                            BaseHealth = cshSettings.BaseHealth,
                            HeadHealth = cshSettings.HeadHealth,
                            BodyHealth = cshSettings.BodyHealth,
                            LeftArmHealth = cshSettings.LeftArmHealth,
                            RightArmHealth = cshSettings.RightArmHealth,
                            LeftLegHealth = cshSettings.LeftLegHealth,
                            RightLegHealth = cshSettings.RightLegHealth,
                            DeathDate = DateTime.Now,
                            IsInjured = false,
                            IsHUDEnabled = true,
                            HUDEffectID = style.EffectID
                        };
                    }
                }
                catch (Exception ex)
                {
                    AdvancedHealth.Logger.Error("Failed to get player health data.", ex);
                }
            });
        }

        protected override void Unload()
        {
            BackgroundThreadDispatcher.Run(async () =>
            {
                try
                {
                    HealthData = await AdvancedHealth.DatabaseManager.HealthData.GetAsync(Player.Id);
                }
                catch (Exception ex)
                {
                    AdvancedHealth.Logger.Error("Failed to get player health data.", ex);
                }
            });
        }

        public void TryAddState(EPlayerState state)
        {
            try
            {
                if (!states.Contains(state))
                {
                    var config = AdvancedHealth.Instance.Config;

                    if (state == EPlayerState.NoneTemperature)
                    {
                        StatusIcon? icon2 = HealthHelper.GetStatusIcon(EPlayerState.Warm);
                        if (icon2 != null)
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon2.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                                TryRemoveState(ic.Status, false);
                            RefreshStateUI();
                        }
                        return;
                    }

                    StatusIcon? icon = HealthHelper.GetStatusIcon(state);

                    if (icon != null)
                    {
                        if (icon.GroupIndex == -1)
                        {
                            states.Add(state);
                            RefreshStateUI();
                        }
                        else
                        {
                            List<StatusIcon> icons = config.HealthSystemSettings.StatusIcons.FindAll(x => x.GroupIndex == icon.GroupIndex && x.Status != state);
                            foreach (StatusIcon ic in icons)
                                TryRemoveState(ic.Status, false);

                            states.Add(state);
                            RefreshStateUI();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                AdvancedHealth.Logger.Error(e.ToString());
            }
        }

        public void TryRemoveState(EPlayerState state, bool shouldUpdate = true)
        {
            try
            {
                if (states.Contains(state))
                {
                    states.RemoveAt(states.FindIndex(x => x == state));
                    if (shouldUpdate)
                        RefreshStateUI();
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error("Failed to remove state.", ex);
            }
        }
        
        private void RefreshStateUI()
        {
            if (HealthData == null)
                return;
            
            short effectID = (short)HealthData.HUDEffectID;

            for (int i = 0; i < 12; i++)
            {
                int localuiname = i + 1;

                if (states.Count - 1 >= i)
                {
                    EPlayerState value = states.ElementAt(i);
                    StatusIcon? icon = HealthHelper.GetStatusIcon(value);
                    if (icon == null)
                        continue;

                    EffectManager.sendUIEffectImageURL(effectID, TranspConnection, true, "Status#" + localuiname + "_img", icon.IconUrl);
                    AdvancedHealth.Instance.InvokeAction(0.1f, () => {
                        if (states.Count >= localuiname)
                            EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, true);
                    });
                }
                else
                {
                    EffectManager.sendUIEffectVisibility(effectID, TranspConnection, true, "Status#" + localuiname, false);
                }
            }
        }

        public void Drag(UnturnedPlayer target)
        {
            AdvancedHealthComponent targetComp = target.GetComponent<AdvancedHealthComponent>();
            var targetHealth = targetComp.HealthData;
            if (targetHealth == null)
                return;
            if (targetHealth.IsInjured || !targetHealth.IsInjured || targetComp.dragState != EDragState.None || dragState != EDragState.None)
                return;

            dragPartnerId = target.CSteamID;
            targetComp.dragPartnerId = Player.CSteamID;
            dragState = EDragState.Dragger;
            targetComp.dragState = EDragState.Dragged;
        }
        
        public void UnDrag(bool receivedFromPartner = false)
        {
            dragState = EDragState.None;
            dragPartnerId = CSteamID.Nil;

            if (receivedFromPartner)
                return;
            UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);
            if (partner == null)
                return;
            AdvancedHealthComponent partnerComp = partner.GetComponent<AdvancedHealthComponent>();
            if (partnerComp == null)
                return;
            partnerComp.UnDrag(true);
        }
        
        public void Revive(bool receivedFromPartner = false)
        {
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;

            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);
            allowDamage = false;
            hasHeavyBleeding = false;

            if (HealthData != null)
            {
                HealthData.BaseHealth = chsettings.BaseHealth;
                HealthData.HeadHealth = chsettings.HeadHealth;
                HealthData.BodyHealth = chsettings.BodyHealth;
                HealthData.LeftArmHealth = chsettings.LeftArmHealth;
                HealthData.RightArmHealth = chsettings.RightArmHealth;
                HealthData.LeftLegHealth = chsettings.LeftLegHealth;
                HealthData.RightLegHealth = chsettings.RightLegHealth;
                HealthData.IsInjured = false;
            }

            Player.Broken = false;
            Player.Bleeding = false;
            Player.Hunger = 0;
            Player.Thirst = 0;
            Player.Infection = 0;
            Player.Heal(100);

            EffectManager.sendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
        
        public void BleedOut(bool receivedFromPartner = false)
        {
            allowDamage = true;
            Player.Player.life.askDamage(100, Player.Position.normalized, EDeathCause.BLEEDING, ELimb.SKULL, CSteamID.Nil, out _);
            var chsettings = AdvancedHealth.Instance.Config.HealthSystemSettings;
            if (!Mathf.Approximately(Player.Player.movement.pluginSpeedMultiplier, chsettings.DefaultWalkSpeed))
                Player.Player.movement.sendPluginSpeedMultiplier(chsettings.DefaultWalkSpeed);
            Player.Player.movement.sendPluginJumpMultiplier(1f);

            if (HealthData != null)
            {
                HealthData.IsInjured = false;
                HealthData.DeathDate = DateTime.Now;
            }

            EffectManager.sendUIEffectVisibility((short)effectId, TranspConnection, true, "RevivePanel", false);
            Player.Player.setPluginWidgetFlag(EPluginWidgetFlags.Modal, false);
        }
    
        private void Update()
        {
            if (dragState == EDragState.Dragger && dragPartnerId != CSteamID.Nil)
            {
                UnturnedPlayer partner = UnturnedPlayer.FromCSteamID(dragPartnerId);

                if (partner != null)
                    if (Vector3.Distance(partner.Position, Player.Position) > 3)
                        partner.Player.teleportToPlayer(Player.Player);
            }
            
            if (HealthData == null)
                return;
            
            #region Injured
            if (HealthData.IsInjured)
            {
                Player.Bleeding = false;

                int secs = (int)(HealthData.DeathDate - DateTime.Now).TotalSeconds;
                EffectManager.sendUIEffectText((short)effectId, TranspConnection, true, "tb_message",
                    AdvancedHealth.Instance.Localize("ui_bleeding", secs.ToString()));
                if (HealthData.DeathDate < DateTime.Now)
                {
                    BleedOut();
                    return;
                }
            }
            #endregion
            
            #region Regeneration
            
                bool canRegenerate =
                    Player.Player.life.food >= AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinFood &&
                    Player.Player.life.water >=
                    AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinWater &&
                    Player.Player.life.virus >= AdvancedHealth.Instance.Config.HealthSystemSettings.HealthRegenMinVirus;

                // Head
                if (_nextHeadHealDate <= DateTime.Now)
                {
                    if (HealthData.HeadHealth + 1 <= AdvancedHealth.Instance.Config.HealthSystemSettings.HeadHealth &&
                        canRegenerate)
                        HealthData.HeadHealth += 1;

                    _nextHeadHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.HeadRegenTicks);
                }

                // Body
                if (_nextBodyHealDate <= DateTime.Now)
                {
                    if (HealthData.BodyHealth + 1 <= AdvancedHealth.Instance.Config.HealthSystemSettings.BodyHealth &&
                        canRegenerate)
                        HealthData.BodyHealth += 1;

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
                            HealthData.LeftArmHealth += 1;

                        if (HealthData.RightArmHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.RightArmHealth)
                            HealthData.RightArmHealth += 1;
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
                            HealthData.LeftLegHealth += 1;

                        if (HealthData.RightLegHealth + 1 <=
                            AdvancedHealth.Instance.Config.HealthSystemSettings.RightLegHealth)
                            HealthData.RightLegHealth += 1;
                    }

                    _nextLegHealDate =
                        DateTime.Now.AddSeconds(AdvancedHealth.Instance.Config.HealthSystemSettings.LegRegenTicks);
                }
            #endregion
        }
    }
}
