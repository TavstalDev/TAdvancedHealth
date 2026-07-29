using System;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TLibrary.Extensions;

namespace Tavstal.TAdvancedHealth.Handlers.Player
{
    public static class PlayerInventoryHandler
    {
        private static AdvancedHealthConfig _config => AdvancedHealth.Instance.Config;
        
        internal static void Attach()
        {
            UseableConsumeable.onConsumePerformed += OnPlayerUseMedicine;
        }

        internal static void Detach()
        {
            UseableConsumeable.onConsumePerformed -= OnPlayerUseMedicine;
        }
        
        internal static void OnPlayerEquipRequested(PlayerEquipment equipment, ItemJar jar, ItemAsset asset, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                bool isMedicine = false;
                if (_config.Medicines.FirstOrDefault(x => x.ItemID == jar.item.id) != null)
                {
                    comp.lastEquipedItem = jar.item.id;
                    isMedicine = true;
                }

                var healthData = comp.HealthData;
                if (healthData == null)
                    return;
                    
                if (healthData.RightArmHealth == 0 && healthData.LeftArmHealth == 0)
                {
                    if (!isMedicine)
                    {
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldOneHandItemsWithBrokenArms)
                            if (_config.OneHandedItems.Items.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldTwoHandItemsWithBrokenArms)
                            if (_config.TwoHandedItems.Items.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                    }
                }
                else if (healthData.RightArmHealth == 0 || healthData.LeftArmHealth == 0)
                    if (!isMedicine)
                    {
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldOneHandItemsWithOneBrokenArm)
                            if (_config.OneHandedItems.Items.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.Restrictions.CanHoldTwoHandItemsWithOneBrokenArm)
                            if (_config.TwoHandedItems.Items.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                    }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerEquipRequested)}.", ex);
            }
        }
        
        internal static void OnPlayerDequipRequested(PlayerEquipment equipment, ref bool shouldAllow)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(equipment.player);
                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                comp.lastEquipedItem = 0;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerDequipRequested)}.", ex);
            }
        }
        
        private static void OnPlayerUseMedicine(SDG.Unturned.Player instigatingPlayer, ItemConsumeableAsset consumableAsset)
        {
            try
            {
                UnturnedPlayer player = UnturnedPlayer.FromPlayer(instigatingPlayer);

                AdvancedHealthComponent comp = player.GetComponent<AdvancedHealthComponent>();
                if (comp.lastEquipedItem == 0)
                    return;
                
                Medicine? med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.lastEquipedItem);
                comp.lastEquipedItem = 0;
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

                if (med.CuresPain)
                    EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                        player.SteamPlayer().transportConnection);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerUseMedicine)}.", ex);
            }
        }
    }
}