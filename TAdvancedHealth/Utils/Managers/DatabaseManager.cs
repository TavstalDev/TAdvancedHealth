using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TAdvancedHealth.Models.Enumerators;
using Tavstal.TLibrary.Models.Plugin;
using Tavstal.TLibrary.Models.Database;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Helpers.General;
using Tavstal.TLibrary.Managers;

namespace Tavstal.TAdvancedHealth.Utils.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
        // ReSharper disable once InconsistentNaming
        private static AdvancedHealthConfig _pluginConfig => AdvancedHealth.Instance.Config;

        public DatabaseManager(IConfigurationBase configuration) : base(AdvancedHealth.Instance, configuration) { }

        /// <summary>
        /// Asynchronously checks the schema of the database.
        /// </summary>
        public override async Task CheckSchemaAsync()
        {
            try
            {
                using (var connection = CreateConnection())
                {
                    if (!await connection.OpenSafeAsync())
                        AdvancedHealth.IsConnectionAuthFailed = true;
                    if (connection.State != System.Data.ConnectionState.Open)
                        throw new Exception("# Failed to connect to the database. Please check the plugin's config file.");

                    if (await connection.DoesTableExistAsync<HealthData>(_pluginConfig.Database.PlayerDataTable))
                        await connection.CheckTableAsync<HealthData>(_pluginConfig.Database.PlayerDataTable);
                    else
                        await connection.CreateTableAsync<HealthData>(_pluginConfig.Database.PlayerDataTable);

                    if (connection.State != System.Data.ConnectionState.Closed)
                        await connection.CloseAsync();
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in checkSchema:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously adds health data to the database.
        /// </summary>
        /// <param name="id">The ID of the health data.</param>
        /// <param name="healthData">The health data to add.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task AddHealthDataAsync(string id, HealthData healthData)
        {
            try
            {
                await CreateConnection().AddTableRowAsync(tableName: _pluginConfig.Database.PlayerDataTable, healthData);

                EventManager.FCallBaseHealthUpdated(id, healthData.BaseHealth);
                EventManager.FCallBodyHealthUpdated(id, healthData.BodyHealth);
                EventManager.FCallHeadHealthUpdated(id, healthData.HeadHealth);
                EventManager.FCallInjuredStateUpdated(id, healthData.IsInjured, healthData.DeathDate);
                EventManager.FCallLeftArmHealthUpdated(id, healthData.LeftArmHealth);
                EventManager.FCallRightArmHealthUpdated(id, healthData.RightArmHealth);
                EventManager.FCallLeftLegHealthUpdated(id, healthData.LeftLegHealth);
                EventManager.FCallRightLegHealthUpdated(id, healthData.RightLegHealth);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in addHealthAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously updates health data in the database.
        /// </summary>
        /// <param name="id">The ID of the health data.</param>
        /// <param name="healthData">The updated health data.</param>
        /// <param name="eventtype">The type of database event to trigger.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateHealthAsync(string id, HealthData healthData, EDatabaseEvent eventtype = EDatabaseEvent.All)
        {
            try
            {
                healthData.EnsureValueCap();
                await CreateConnection().UpdateTableRowAsync(tableName: _pluginConfig.Database.PlayerDataTable, healthData, $"PlayerId={id}", null);

                switch (eventtype)
                {
                    default:
                    case EDatabaseEvent.None:
                        {
                            break;
                        }
                    case EDatabaseEvent.All:
                        {
                            EventManager.FCallBaseHealthUpdated(id, healthData.BaseHealth);
                            EventManager.FCallBodyHealthUpdated(id, healthData.BodyHealth);
                            EventManager.FCallHeadHealthUpdated(id, healthData.HeadHealth);
                            EventManager.FCallInjuredStateUpdated(id, healthData.IsInjured, healthData.DeathDate);
                            EventManager.FCallLeftArmHealthUpdated(id, healthData.LeftArmHealth);
                            EventManager.FCallRightArmHealthUpdated(id, healthData.RightArmHealth);
                            EventManager.FCallLeftLegHealthUpdated(id, healthData.LeftLegHealth);
                            EventManager.FCallRightLegHealthUpdated(id, healthData.RightLegHealth);
                            break;
                        }
                    case EDatabaseEvent.Base:
                        {
                            EventManager.FCallBaseHealthUpdated(id, healthData.BaseHealth);
                            break;
                        }
                    case EDatabaseEvent.Head:
                        {
                            EventManager.FCallHeadHealthUpdated(id, healthData.HeadHealth);
                            break;
                        }
                    case EDatabaseEvent.Body:
                        {
                            EventManager.FCallBodyHealthUpdated(id, healthData.BodyHealth);
                            break;
                        }
                    case EDatabaseEvent.LeftARM:
                        {
                            EventManager.FCallLeftArmHealthUpdated(id, healthData.LeftArmHealth);
                            break;
                        }
                    case EDatabaseEvent.RightARM:
                        {
                            EventManager.FCallRightArmHealthUpdated(id, healthData.RightArmHealth);
                            break;
                        }
                    case EDatabaseEvent.LeftLeg:
                        {
                            EventManager.FCallLeftLegHealthUpdated(id, healthData.LeftLegHealth);
                            break;
                        }
                    case EDatabaseEvent.RightLeg:
                        {
                            EventManager.FCallRightLegHealthUpdated(id, healthData.RightLegHealth);
                            break;
                        }
                    case EDatabaseEvent.Injured:
                        {
                            EventManager.FCallInjuredStateUpdated(id, healthData.IsInjured, healthData.DeathDate);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in updateHealthAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously updates the HUD effect ID in the database.
        /// </summary>
        /// <param name="id">The ID of the data to update.</param>
        /// <param name="effectid">The new HUD effect ID.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateHUDEffectIdAsync(string id, ushort effectid)
        {
            try
            {
                await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.HUDEffectID, effectid));
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in updateEffectIdAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously updates the HUD enabled status in the database.
        /// </summary>
        /// <param name="id">The ID of the data to update.</param>
        /// <param name="isEnabled">The new HUD enabled status.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateHUDEnabledAsync(string id, bool isEnabled)
        {
            try
            {
                await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.IsHUDEnabled, isEnabled));
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in updateEffectStateAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously updates the injured status and bleed date in the database.
        /// </summary>
        /// <param name="id">The ID of the data to update.</param>
        /// <param name="isInjured">The new injured status.</param>
        /// <param name="bleedDate">The new bleed date.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateInjuredAsync(string id, bool isInjured, DateTime bleedDate)
        {
            try
            {
                await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", new List<SqlParameter>
                {
                    SqlParameter.Get<HealthData>(x => x.IsInjured, isInjured),
                    SqlParameter.Get<HealthData>(x => x.DeathDate, bleedDate)
                });
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in updateInjuredAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously updates the health value in the database.
        /// </summary>
        /// <param name="id">The ID of the data to update.</param>
        /// <param name="newHealth">The new health value.</param>
        /// <param name="type">The type of health value to update (e.g., base health).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task UpdateHealthAsync(string id, float newHealth, EHealth type = EHealth.Base)
        {
            try
            {
                switch (type)
                {
                    default:
                    case EHealth.Base:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.BaseHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.BaseHealth, newHealth));
                            EventManager.FCallBaseHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.Head:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.HeadHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.HeadHealth, newHealth));
                            EventManager.FCallHeadHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.Body:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.BodyHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.BodyHealth, newHealth));
                            EventManager.FCallBodyHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.LeftARM:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.LeftArmHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.LeftArmHealth, newHealth));
                            EventManager.FCallLeftArmHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.RightARM:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.RightArmHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.RightArmHealth, newHealth));
                            EventManager.FCallRightArmHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.LeftLeg:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.LeftLegHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.LeftLegHealth, newHealth));
                            EventManager.FCallLeftLegHealthUpdated(id, newHealth);
                            break;
                        }
                    case EHealth.RightLeg:
                        {
                            newHealth = MathHelper.Clamp(newHealth, 0, _pluginConfig.HealthSystemSettings.RightLegHealth);

                            await CreateConnection().UpdateTableRowAsync<HealthData>(tableName: _pluginConfig.Database.PlayerDataTable, $"PlayerId='{id}'", SqlParameter.Get<HealthData>(x => x.RightLegHealth, newHealth));
                            EventManager.FCallRightLegHealthUpdated(id, newHealth);
                            break;
                        }
                }
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in updateBaseHealthAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
        }

        /// <summary>
        /// Asynchronously retrieves the health data of a player from the database.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the health data of the player.</returns>
        public async Task<HealthData> GetPlayerHealthAsync(string id)
        {
            HealthData healthData = null;
            try
            {
                healthData = await CreateConnection().GetTableRowAsync<HealthData>($"PlayerId={id}", null);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in getHealthAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
            return healthData;
        }

        /// <summary>
        /// Asynchronously retrieves the health data of a player from the database.
        /// </summary>
        /// <param name="id">The ID of the player.</param>
        /// <returns>A task representing the asynchronous operation. The task result contains the health data of the player.</returns>
        public HealthData GetPlayerHealth(string id)
        {
            HealthData healthData = null;
            try
            {
                healthData = CreateConnection().GetTableRowAsync<HealthData>($"PlayerId={id}", null).Result;
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.LogException("Error in getHealthAsync:");
                AdvancedHealth.Logger.LogError(ex);
            }
            return healthData;
        }
    }
}
