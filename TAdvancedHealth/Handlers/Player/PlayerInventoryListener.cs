using System;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.RocketFlow.Attributes;
using Tavstal.RocketFlow.Core;
using Tavstal.RocketFlow.Events.Item;
using Tavstal.RocketFlow.Events.Player.Inventory;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Utils.Managers;
using Tavstal.TLibrary.Extensions;
// ReSharper disable UnusedMember.Local

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public class PlayerInventoryListener : EventListener
    {
        private AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;

        [EventHandler]
        private void OnPlayerEquip(PlayerEquipEvent e)
        {
            try
            {
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                    
                if (healthData.RightArmHealth > 0 && healthData.LeftArmHealth > 0)
                    return;

                ushort itemID = e.Item.item.id;
                var itemType = e.Asset.type;
                var equipment = e.Player.Player.equipment;
                bool isMedicine = _config.Medicines.Any(x => x.ItemID == itemID);
                if (isMedicine)
                    return;
                
                try
                {
                    if (healthData.RightArmHealth == 0 && healthData.LeftArmHealth == 0)
                    {
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldOneHandItemsWithBrokenArms)
                            if (_config.OneHandedItems.Items.Contains(itemID) || _config.OneHandedItems.ItemTypes.Contains(itemType))
                            {
                                e.ShouldAllow = false;
                                e.IsCancelled = true;
                                if (equipment.itemID != 0)
                                    equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldTwoHandItemsWithBrokenArms)
                            if (_config.TwoHandedItems.Items.Contains(itemID) || _config.TwoHandedItems.ItemTypes.Contains(itemType))
                            {
                                e.ShouldAllow = false;
                                e.IsCancelled = true;
                                if (equipment.itemID != 0)
                                    equipment.dequip();
                            }
                        return;
                    }
                    
                    if (!_config.HealthSystemSettings.Restrictions.CanHoldOneHandItemsWithOneBrokenArm)
                        if (_config.OneHandedItems.Items.Contains(itemID) || _config.OneHandedItems.ItemTypes.Contains(itemType))
                        {
                            e.ShouldAllow = false;
                            e.IsCancelled = true;
                            if (equipment.itemID != 0)
                                equipment.dequip();
                        }
                    if (!_config.HealthSystemSettings.Restrictions.CanHoldTwoHandItemsWithOneBrokenArm)
                        if (_config.TwoHandedItems.Items.Contains(itemID) || _config.TwoHandedItems.ItemTypes.Contains(itemType))
                        {
                            e.ShouldAllow = false;
                            e.IsCancelled = true;
                            if (equipment.itemID != 0)
                                equipment.dequip();
                        }
                }
                finally
                {
                    if (e.ShouldAllow)
                        comp.lastEquipedItem = e.Item.item.id;
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerEquip)}.", ex);
            }
        }

        [EventHandler(priority: EEventPriority.LOWEST, ignoreCancelled: true)]
        private void OnPlayerDequip(PlayerDequipEvent e)
        {
            try
            {
                if (!e.ShouldAllow)
                    return;
                    
                AdvancedHealthComponent? comp = ComponentManager.Get(e.Player);
                if (comp == null)
                    return;
                
                comp.lastEquipedItem = 0;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDequip)}.", ex);
            }
        }

        [EventHandler]
        private void OnConsume(ItemConsumeEvent e)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(e.InstigatingPlayer);
                AdvancedHealthComponent? comp = ComponentManager.Get(player);
                if (comp == null)
                    return;

                if (comp.lastEquipedItem == 0)
                    return;

                comp.lastEquipedItem = 0;
                Medicine? med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.lastEquipedItem);
                if (med == null)
                    return;

                var health = comp.HealthData;
                if (health == null)
                    return;

                health.SetHeadHealth(health.HeadHealth + med.HeadHp);
                health.SetBodyHealth(health.BodyHealth + med.BodyHp);
                health.SetLeftArmHealth(health.LeftArmHealth + med.LeftArmHp);
                health.SetRightArmHealth(health.RightArmHealth + med.RightArmHp);

                if (med.LeftLegHp > 0)
                {
                    health.SetLeftLegHealth(health.LeftLegHealth + med.LeftLegHp);
                    AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                    {
                        player.Broken = false;
                        player.Player.movement.sendPluginJumpMultiplier(1f);
                    });
                }

                if (med.RightLegHp > 0)
                {
                    health.SetRightLegHealth(health.RightLegHealth + med.RightLegHp);
                    AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                    {
                        player.Broken = false;
                        player.Player.movement.sendPluginJumpMultiplier(1f);
                    });
                }

                if (!med.CuresPain)
                    return;
                
                EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                    player.SteamPlayer().transportConnection);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnConsume)}.", ex);
            }
        }
    }
}