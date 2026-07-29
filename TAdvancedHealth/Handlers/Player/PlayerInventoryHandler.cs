using System;
using System.Linq;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Tavstal.TAdvancedHealth.Components;
using Tavstal.TAdvancedHealth.Models.Config;
using Tavstal.TAdvancedHealth.Models.Database;
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

                HealthData? healthData = comp.HealthData;
                if (healthData == null)
                    return;
                    
                if (healthData.RightArmHealth == 0 && healthData.LeftArmHealth == 0)
                {
                    if (!isMedicine)
                    {
                        if (!_config.HealthSystemSettings.CanHoldOneHandItemsWithBrokenArms)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.CanHoldTwoHandItemsWithBrokenArms)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
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
                        if (!_config.HealthSystemSettings.CanHoldOneHandItemsWithOneBrokenArm)
                            if (_config.OneHandedItems.ItemID.Contains(jar.item.id) || _config.OneHandedItems.ItemTypes.Contains(asset.type))
                            {
                                shouldAllow = false;
                                if (player.Player.equipment.itemID != 0)
                                    player.Player.equipment.dequip();
                            }
                        if (!_config.HealthSystemSettings.CanHoldTwoHandItemsWithOneBrokenArm)
                            if (_config.TwoHandedItems.ItemID.Contains(jar.item.id) || _config.TwoHandedItems.ItemTypes.Contains(asset.type))
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
                {
                    return;
                }
                
                Medicine? med = _config.Medicines.FirstOrDefault(x => x.ItemID == comp.lastEquipedItem);
                if (med != null)
                {
                    HealthData? health = comp.HealthData;
                    if (health != null)
                    {
                        if (health.BodyHealth + med.HealsBodyHp <= _config.HealthSystemSettings.BodyHealth)
                            health.BodyHealth += med.HealsBodyHp;
                        else
                            health.BodyHealth = _config.HealthSystemSettings.BodyHealth;

                        if (health.HeadHealth + med.HealsHeadHp <= _config.HealthSystemSettings.HeadHealth)
                            health.HeadHealth += med.HealsHeadHp;
                        else
                            health.HeadHealth = _config.HealthSystemSettings.HeadHealth;

                        if (med.CuresPain)
                            EffectManager.askEffectClearByID(_config.HealthSystemSettings.PainEffectID,
                                player.SteamPlayer().transportConnection);

                        if (health.LeftLegHealth + med.HealsLeftLegHp <= _config.HealthSystemSettings.LeftLegHealth)
                        {
                            health.LeftLegHealth += med.HealsLeftLegHp;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.LeftLegHealth = _config.HealthSystemSettings.LeftLegHealth;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.RightLegHealth + med.HealsRightLegHp <=
                            _config.HealthSystemSettings.RightLegHealth)
                        {
                            health.RightLegHealth += med.HealsRightLegHp;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }
                        else
                        {
                            health.RightLegHealth = _config.HealthSystemSettings.RightLegHealth;
                            AdvancedHealth.Instance.InvokeAction(0.5f, () =>
                            {
                                player.Broken = false;
                                player.Player.movement.sendPluginJumpMultiplier(1f);
                            });
                        }

                        if (health.LeftArmHealth + med.HealsLeftArmHp <= _config.HealthSystemSettings.LeftArmHealth)
                            health.LeftArmHealth += med.HealsLeftArmHp;
                        else
                            health.LeftArmHealth = _config.HealthSystemSettings.LeftArmHealth;

                        if (health.RightArmHealth + med.HealsRightArmHp <=
                            _config.HealthSystemSettings.RightArmHealth)
                            health.RightArmHealth += med.HealsRightArmHp;
                        else
                            health.RightArmHealth = _config.HealthSystemSettings.RightArmHealth;
                    }
                }

                comp.lastEquipedItem = 0;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error($"Unexpected error occured in {nameof(OnPlayerUseMedicine)}.", ex);
            }
        }
    }
}