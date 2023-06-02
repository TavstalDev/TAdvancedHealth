using MySql.Data.MySqlClient;
using System;
using Logger = Tavstal.TAdvancedHealth.Helpers.LoggerHelper;
using Tavstal.TAdvancedHealth.Compatibility;
using Tavstal.TAdvancedHealth.Managers;
using Tavstal.TAdvancedHealth.Helpers;

namespace Tavstal.TAdvancedHealth
{
    public class DatabaseManager
    {
        public DatabaseManager()
        {
            new I18N.West.CP1250();
            CheckSchema();
        }

        public void CheckSchema()
        {
            try
            {
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();

                MySQLCommand.CommandText = "SHOW TABLES LIKE '" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "'";
                MySQLConnection.Open();

                object result = MySQLCommand.ExecuteScalar();

                if (result == null)
                {
                    MySQLCommand.CommandText = "CREATE TABLE " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData +
                    "(steamID VARCHAR(50) NOT NULL," +
                    "baseHealth FLOAT(6,2)," +
                    "headHealth FLOAT(6,2)," +
                    "bodyHealth FLOAT(6,2)," +
                    "rightArmHealth FLOAT(6,2)," +
                    "leftArmHealth FLOAT(6,2)," +
                    "rightLegHealth FLOAT(6,2)," +
                    "leftLegHealth FLOAT(6,2)," +
                    "isInjured BIT," +
                    "isHUDEnabled BIT," +
                    "hudEffectId SMALLINT UNSIGNED," +
                    "dieDate DATETIME," +
                    "PRIMARY KEY(steamID));";

                    MySQLCommand.ExecuteNonQuery();
                }
                
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public MySqlConnection CreateConnection()
        {
            MySqlConnection MySQLConnection = null;

            try
            {
                MySQLConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", new object[] {
                    TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseAddress,
                    TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseName,
                    TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseUser,
                    TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabasePassword,
                    TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabasePort
                }));
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
            return MySQLConnection;
        }
        public void Add(string id, PlayerHealth h)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.Parameters.AddWithValue("@SteamID", id);
                MySQLCommand.Parameters.AddWithValue("@BaseHealth", h.BaseHealth);
                MySQLCommand.Parameters.AddWithValue("@HeadHealth", h.HeadHealth);
                MySQLCommand.Parameters.AddWithValue("@BodyHealth", h.BodyHealth);
                MySQLCommand.Parameters.AddWithValue("@RightArmHealth", h.RightArmHealth);
                MySQLCommand.Parameters.AddWithValue("@LeftArmHealth", h.LeftArmHealth);
                MySQLCommand.Parameters.AddWithValue("@RightLegHealth", h.RightLegHealth);
                MySQLCommand.Parameters.AddWithValue("@LeftLegHealth", h.LeftLegHealth);
                MySQLCommand.Parameters.AddWithValue("@isInjured", h.isInjured);
                MySQLCommand.Parameters.AddWithValue("@isHUDEnabled", h.isHUDEnabled);
                MySQLCommand.Parameters.AddWithValue("@HUDEffectID", h.HUDEffectID);
                MySQLCommand.Parameters.AddWithValue("@dieDate", h.dieDate);

                MySQLCommand.CommandText = $"SELECT * FROM {tablename} WHERE steamID=@SteamID";
                object data = MySQLCommand.ExecuteScalar();

                if (data == null)
                {
                    MySQLCommand.CommandText = $"INSERT INTO {tablename} " +
                        $"(steamID,baseHealth,headHealth,bodyHealth,rightArmHealth,leftArmHealth,rightLegHealth,leftLegHealth,isInjured,isHUDEnabled,hudEffectId,dieDate) " +
                        $"VALUES (@SteamID,@BaseHealth,@HeadHealth,@BodyHealth,@RightArmHealth,@LeftArmHealth,@RightLegHealth,@LeftLegHealth,@isInjured,@isHUDEnabled,@HUDEffectID,@dieDate)";
                    MySQLCommand.ExecuteNonQuery();

                    EventManager.FCallBaseHealthUpdated(id, h.BaseHealth);
                    EventManager.FCallBodyHealthUpdated(id, h.BodyHealth);
                    EventManager.FCallHeadHealthUpdated(id, h.HeadHealth);
                    EventManager.FCallInjuredStateUpdated(id, h.isInjured, h.dieDate);
                    EventManager.FCallLeftArmHealthUpdated(id, h.LeftArmHealth);
                    EventManager.FCallRightArmHealthUpdated(id, h.RightArmHealth);
                    EventManager.FCallLeftLegHealthUpdated(id, h.LeftLegHealth);
                    EventManager.FCallRightLegHealthUpdated(id, h.RightLegHealth);
                }

                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void Update(string id, PlayerHealth h)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.Parameters.AddWithValue("@ID", id);
                MySQLCommand.CommandText = $"SELECT * FROM {tablename} WHERE steamID=@ID";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@HEI", h.HUDEffectID);
                    MySQLCommand.Parameters.AddWithValue("@BAH", h.BaseHealth);
                    MySQLCommand.Parameters.AddWithValue("@HH", h.HeadHealth);
                    MySQLCommand.Parameters.AddWithValue("@BH", h.BodyHealth);
                    MySQLCommand.Parameters.AddWithValue("@RAH", h.RightArmHealth);
                    MySQLCommand.Parameters.AddWithValue("@LAH", h.LeftArmHealth);
                    MySQLCommand.Parameters.AddWithValue("@RLH", h.RightLegHealth);
                    MySQLCommand.Parameters.AddWithValue("@LLH", h.LeftLegHealth);
                    MySQLCommand.Parameters.AddWithValue("@II", h.isInjured);
                    MySQLCommand.Parameters.AddWithValue("@IH", h.isHUDEnabled);
                    MySQLCommand.Parameters.AddWithValue("@DD", h.dieDate.ToString());
                    EventManager.FCallBaseHealthUpdated(id, h.BaseHealth);
                    EventManager.FCallBodyHealthUpdated(id, h.BodyHealth);
                    EventManager.FCallHeadHealthUpdated(id, h.HeadHealth);
                    EventManager.FCallInjuredStateUpdated(id, h.isInjured, h.dieDate);
                    EventManager.FCallLeftArmHealthUpdated(id, h.LeftArmHealth);
                    EventManager.FCallRightArmHealthUpdated(id, h.RightArmHealth);
                    EventManager.FCallLeftLegHealthUpdated(id, h.LeftLegHealth);
                    EventManager.FCallRightLegHealthUpdated(id, h.RightLegHealth);

                    MySQLCommand.CommandText = $"UPDATE {tablename} SET hudEffectId=@HEI,baseHealth=@BAH,headHealth=@HH,bodyHealth=@BH,rightArmHealth=@RAH,leftArmHealth=@LAH,rightLegHealth=@RLH,leftLegHealth=@LLH,isInjured=@II,isHUDEnabled=@IH,dieDate=@DD WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                }
                else
                {
                    var c = TAdvancedHealthMain.Instance.Configuration.Instance;
                    var s = c.CustomHealtSystemAndComponentSettings;
                    Add(id, h);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        public void UpdateHUDEffectId(string id, ushort effectid)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.Parameters.AddWithValue("@ID", id);
                MySQLCommand.CommandText = $"SELECT * FROM {tablename} WHERE steamID=@ID";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@HEI", effectid);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET hudEffectId=@HEI WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateHUDEnabled(string id, bool isEnabled)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@IH", isEnabled);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET isHUDEnabled=@IH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateInjured(string id, bool isInjured, DateTime bleedDate)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@II", isInjured);
                    MySQLCommand.Parameters.AddWithValue("@DD",bleedDate.ToString());

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET isInjured=@II,dieDate=@DD WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallInjuredStateUpdated(id, isInjured, bleedDate);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateBaseHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@BAH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET baseHealth=@BAH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallBaseHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateHeadHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@HH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET headHealth=@HH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallHeadHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateBodyHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@BH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET bodyHealth=@BH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallBodyHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateRightArmHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@RAH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET rightArmHealth=@RAH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallRightArmHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateLeftArmHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@LAH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET leftArmHealth=@LAH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallLeftArmHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateRightLegHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@RLH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET rightLegHealth=@RLH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallRightLegHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public void UpdateLeftLegHealth(string id, float newHealth)
        {
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();

                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                object data = MySQLCommand.ExecuteScalar();

                if (data != null)
                {
                    MySQLCommand.Parameters.AddWithValue("@ID", id);
                    MySQLCommand.Parameters.AddWithValue("@LLH", newHealth);

                    MySQLCommand.CommandText = "UPDATE `" + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + "` SET leftLegHealth=@LLH WHERE steamID=@ID;";
                    MySQLCommand.ExecuteNonQuery();
                    EventManager.FCallLeftLegHealthUpdated(id, newHealth);
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public PlayerHealth GetPlayerHealth(string id)
        {
            PlayerHealth h = null;
            try
            {
                string tablename = TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData;
                MySqlConnection MySQLConnection = CreateConnection();
                MySqlCommand MySQLCommand = MySQLConnection.CreateCommand();
                MySQLConnection.Open();
                MySQLCommand.CommandText = "SELECT * FROM " + TAdvancedHealthMain.Instance.Configuration.Instance.databaseData.DatabaseTable_PlayerData + " WHERE steamID = '" + id + "'";
                MySqlDataReader Reader = MySQLCommand.ExecuteReader();

                while (Reader.Read())
                {
                    string toreplace = ".";
                    string replacement = ",";
                    float baseh = Convert.ToSingle(Reader.GetString("baseHealth").Replace(toreplace, replacement));
                    float head = Convert.ToSingle(Reader.GetString("headHealth").Replace(toreplace, replacement));
                    float body = Convert.ToSingle(Reader.GetString("bodyHealth").Replace(toreplace, replacement));
                    float rarm = Convert.ToSingle(Reader.GetString("rightArmHealth").Replace(toreplace, replacement));
                    float larm = Convert.ToSingle(Reader.GetString("leftArmHealth").Replace(toreplace, replacement));
                    float lleg = Convert.ToSingle(Reader.GetString("leftLegHealth").Replace(toreplace, replacement));
                    float rleg = Convert.ToSingle(Reader.GetString("rightLegHealth").Replace(toreplace, replacement));
                    bool inj = Reader.GetBoolean("isInjured");
                    bool hud = Reader.GetBoolean("isHUDEnabled");
                    ushort hudid = Convert.ToUInt16(Reader.GetString("hudEffectId"));
                    DateTime date = Convert.ToDateTime(Reader.GetString("dieDate"));

                    h = new PlayerHealth
                    {
                      PlayerId = id,
                      BaseHealth = baseh,
                      HeadHealth = head,
                      BodyHealth = body,
                      LeftArmHealth = larm,
                      LeftLegHealth = lleg,
                      RightArmHealth = rarm,
                      RightLegHealth = rleg,
                      isInjured = inj,
                      isHUDEnabled = hud,
                      HUDEffectID = hudid,
                      dieDate = date,
                    };
                }
                MySQLConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogError("Error in mysql GetPlayerhealth():");
                Logger.LogError(ex);
            }
            return h;
        }
    }
}
