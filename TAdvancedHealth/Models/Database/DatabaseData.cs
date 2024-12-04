using Newtonsoft.Json;
using System;
using Tavstal.TLibrary.Models.Database;

namespace Tavstal.TAdvancedHealth.Models.Database
{
    [Serializable]
    public class DatabaseData : DatabaseSettingsBase
    {
        [JsonProperty(Order = 7)]
        public string PlayerDataTable { get; set; }

        public DatabaseData() { }

        public DatabaseData(string playerDataTable)
        {
            PlayerDataTable = playerDataTable;
        }
    }
}
