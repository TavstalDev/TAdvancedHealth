using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Tavstal.TAdvancedHealth.Compatibility
{
    [Serializable]
    public class DatabaseData
    {
        public string DatabaseAddress { get; set; }
        public int DatabasePort { get; set; }
        public string DatabaseUser { get; set; }
        public string DatabasePassword { get; set; }
        public string DatabaseName { get; set; }
        public string DatabaseTable_PlayerData { get; set; }

        public DatabaseData(string address, int port, string user, string password, string database, string table_data)
        {
            DatabaseAddress = address;
            DatabasePort = port;
            DatabaseUser = user;
            DatabasePassword = password;
            DatabaseName = database;
            DatabaseTable_PlayerData = table_data;
        }

        public DatabaseData() { }
    }
}
