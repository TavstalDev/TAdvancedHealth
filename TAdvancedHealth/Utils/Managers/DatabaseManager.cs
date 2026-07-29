using System;
using System.Threading.Tasks;
using Tavstal.TAdvancedHealth.Models.Database;
using Tavstal.TLibrary.Extensions;
using Tavstal.TLibrary.Managers;
using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TAdvancedHealth.Utils.Managers
{
    public class DatabaseManager : DatabaseManagerBase
    {
        public MySqlRepository<string, HealthData> HealthData { get; }

        public DatabaseManager(AdvancedHealthConfig configuration) : base(AdvancedHealth.Instance,
            configuration.Database)
        {
            HealthData = new MySqlRepository<string, HealthData>(this, configuration.Database.TablePrefix);
        }

        /// <summary>
        /// Asynchronously checks the schema of the database.
        /// </summary>
        public override async Task CheckSchemaAsync()
        {
            try
            {
                await using var connection = CreateConnection();
                await HealthData.CheckSchemaAsync(connection);
            }
            catch (Exception ex)
            {
                AdvancedHealth.Logger.Error("Unexpected error occured in checkSchema:", ex);
            }
        }
    }
}
