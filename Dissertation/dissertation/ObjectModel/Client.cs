using dissertation.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Web.Mvc.Ajax;

namespace dissertation.ObjectModel
{
    public class Client
    {
        private bool HaveDetails = false;
        private const string Fields = "CLIENTID, NICKNAME, COMPUTERNAME, CURRENTIP, OS, LASTSTATUS, CLIENTHASH ";

        #region DBFields

        public int ID { set; get; }

        public string Nickname
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return nickname;
            }
        }
        protected string nickname;

        public string ComputerName
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return computerName;
            }
        }
        protected string computerName;

        public string CurrentIP
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return currentIP;
            }
        }
        protected string currentIP;

        public string OS
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return os;
            }
        }
        protected string os;

        public DateTime LastStatus
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return lastStatus;
            }
        }
        protected DateTime lastStatus;

        public string ClientHash
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return clientHash;
            }
        }
        protected string clientHash;

        #endregion

        private Client() { }

        protected Client(int id)
        {
            ID = id;
        }

        public static Client Get(int id)
        {
            return new Client(id);
        }

        public static Client Get(string ClientName, User user)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT CLIENTID FROM VIEW_CLIENT WHERE USERID = ? AND NICKNAME = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("", OdbcType.Int).Value = user.ID;
                        cmd.Parameters.Add("", OdbcType.Int).Value = ClientName;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return new Client(reader.GetInt32(0));
                            }

                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return null;
        }

        public static bool InsertSetting(Client ClientID, int DnsTypeID, int? TosID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "INSERT INTO Settings(ClientID, DnsTypeID, TosID) VALUES(?,?,?)";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ClientID.ID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = DnsTypeID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = TosID;
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public IEnumerable<Models.SettingStruct> GetSettings(int ID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT * FROM VIEW_SETTING WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    yield return new Models.SettingStruct()
                                    {
                                        Name = reader.GetString(1),
                                        ComputerName = reader.GetString(2),
                                        CurrentIP = reader.GetString(3),
                                        OS = reader.GetString(4),
                                        Status = reader.GetDateTime(5),
                                        DnsName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                        PreferDns = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                        AlternativeDns = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                        Tos_Start_Minute = reader.IsDBNull(11) ? -1 : reader.GetInt32(11),
                                        Tos_End_Minute = reader.IsDBNull(12) ? -1 : reader.GetInt32(12),
                                        Tos_Day = reader.IsDBNull(7) ? null : (DayOfWeek?)reader.GetInt32(13)
                                    };
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static IEnumerable<Models.SettingStruct> getAllClientSettings(int ID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT * FROM VIEW_SETTING WHERE USERID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    yield return new Models.SettingStruct()
                                    {
                                        Name = reader.GetString(1),
                                        ComputerName = reader.GetString(2),
                                        CurrentIP = reader.GetString(3),
                                        OS = reader.GetString(4),
                                        Status = reader.GetDateTime(5),
                                        DnsName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                                        PreferDns = reader.IsDBNull(8) ? "" : reader.GetString(8),
                                        AlternativeDns = reader.IsDBNull(9) ? "" : reader.GetString(9),
                                        Tos_Start_Minute = reader.IsDBNull(11) ? -1 : reader.GetInt32(11),
                                        Tos_End_Minute = reader.IsDBNull(12) ? -1 : reader.GetInt32(12),
                                        Tos_Day = reader.IsDBNull(7) ? null : (DayOfWeek?)reader.GetInt32(13)
                                    };
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static bool UpdateDnsSettings(int DnsTypeID, int ClientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "UPDATE SETTINGS SET DNSTYPEID = ? WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = DnsTypeID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ClientID;
                        cmd.ExecuteNonQuery();
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static bool UpdateSettings(int DnsTypeID, int TosID, int ClientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "UPDATE SETTINGS SET DNSTYPEID = ?, TOSID = ? WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = DnsTypeID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = TosID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ClientID;
                        cmd.ExecuteNonQuery();
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public bool UpdateClientInformation(string computerName, string currentIP, string OS, DateTime dateTime)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "UPDATE CLIENTINFORMATION SET COMPUTERNAME = ?, CURRENTIP = ?, OS = ?, LASTSTATUS = ? WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = computerName;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = currentIP;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = OS;
                        cmd.Parameters.Add("CLIENTID", OdbcType.DateTime).Value = dateTime;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ID;
                        cmd.ExecuteNonQuery();
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static bool UpdateClientHash(string clientHash, int clientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "UPDATE CLIENTINFORMATION SET ClientHash = ? WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = clientHash;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = clientID;
                        cmd.ExecuteNonQuery();
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public bool StatusUpdate(DateTime dateTime)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "UPDATE CLIENTINFORMATION SET LASTSTATUS = ? WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {

                        cmd.Parameters.Add("CLIENTID", OdbcType.DateTime).Value = dateTime;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ID;
                        cmd.ExecuteNonQuery();
                    }
                    value = true;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static int UniqueClient(int userID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "Select TOP 1 ClientID from UserAndClient where UserID = ? ORDER BY ClientID DESC";
                    using (var cmd = new OdbcCommand(query, sql))
                    {

                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = userID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                return reader.GetInt32(0);
                            }
                        }

                    }

                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return -1;
        }

        

        public static IEnumerable<viewDevicesStruct> getUniqueClient(int userID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = @"SELECT C.ClientID, C.Nickname, C.LastStatus, COUNT(A.AlertID) AS totalViolations
                                    FROM ClientInformation AS C
                                    LEFT JOIN Alert AS A ON A.ClientID = C.ClientID
                                    INNER JOIN UserAndClient AS UAC ON UAC.ClientID = C.ClientID
                                    INNER JOIN UserDetails AS U ON U.UserID = UAC.UserID
                                    WHERE UAC.USERID = ?
                                    GROUP BY C.ClientID, C.NickName, C.LastStatus";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = userID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    yield return new Models.viewDevicesStruct()
                                    {
                                        Name = reader.GetString(1),
                                        LastStatus = reader.IsDBNull(2) ? new DateTime(2000, 01, 01) : reader.GetDateTime(2),
                                        TotalViolations = reader.GetInt32(3)
                                    };
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static IEnumerable<Models.ClientStruct> ViewClient(int userID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT Nickname, ComputerName, CurrentIP, OS, LastStatus, Screenshot, Keyword, Location, DateAndTime FROM VIEW_CLIENT WHERE USERID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = userID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read()) {
                                    yield return new Models.ClientStruct()
                                    {
                                        Name = reader.GetString(0),
                                        ComputerName = reader.GetString(1),
                                        CurrentIP = reader.GetString(2),
                                        OS = reader.GetString(3),
                                        LastStatus = reader.IsDBNull(4) ? new DateTime(2000, 01, 01) : reader.GetDateTime(4),
                                        Screenshot = reader.IsDBNull(5) ? "" : (string)reader.GetString(5),
                                        Keyword = reader.IsDBNull(6) ? "" : (string)reader.GetString(6),
                                        Location = reader.IsDBNull(7) ? "" : (string) reader.GetString(7),
                                        AlertTime = reader.IsDBNull(8) ? new DateTime(2000,01,01) : reader.GetDateTime(8)
                                    };
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static bool AlertNotification(string Screenshot, string Keyword, string Location, DateTime dateAndTime, int AlertTypeID, int clientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertAlert ?,?,?,?,?,?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("ALERTID", OdbcType.VarChar).Value = Screenshot;
                        cmd.Parameters.Add("ALERTID", OdbcType.VarChar).Value = Keyword;
                        cmd.Parameters.Add("ALERTID", OdbcType.VarChar).Value = Location;
                        cmd.Parameters.Add("ALERTID", OdbcType.DateTime).Value = dateAndTime;
                        cmd.Parameters.Add("ALERTID", OdbcType.Int).Value = AlertTypeID;
                        cmd.Parameters.Add("ALERTID", OdbcType.Int).Value = clientID;
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static bool Insert(string nickname, string computerName, string currentIP, string os, int userID, DateTime lastStatus, string clientHash)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertClientInformation ?,?,?,?,?,?,?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = nickname;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = computerName;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = currentIP;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = os;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = userID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.DateTime).Value = lastStatus;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = clientHash;
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public void GetDetails()
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = $"SELECT {Fields} FROM CLIENTINFORMATION WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                FromReader(reader);
                            }
                            else
                                throw new Exception("User does not exist.");
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static Client GetClientHash(String clientHash)
        {
            Client client = null;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = $"SELECT {Fields} FROM CLIENTINFORMATION WHERE CLIENTHASH = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = clientHash;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                client = new Client();
                                client.FromReader(reader);
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return client;
        }

        public static bool InsertTos(int startMinute, int endMinute, int day, int clientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertToS ?,?,?,?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = startMinute;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = endMinute;
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = day;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = clientID;
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static bool RemoveTosSettings(int clientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "DELETE FROM TOS WHERE CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.VarChar).Value = clientID;
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static bool InsertClientEmotions(string ToneName, decimal ToneScore, int ClientID, int UserID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertClientEmotions ?,?,?,?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.AddWithValue("@ToneName",ToneName);
                        cmd.Parameters.AddWithValue("@ToneScore", ToneScore);

                        cmd.Parameters.AddWithValue("@ClientID", ClientID);

                        cmd.Parameters.AddWithValue("@UserID", UserID);
                        cmd.ExecuteNonQuery();
                        value = true;
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        public static IEnumerable<ClientEmotions> GetClientEmotions(int UserID)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT * FROM ClientEmotions WHERE UserID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = UserID;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    yield return new ClientEmotions()
                                    {
                                        ToneName = reader.GetString(1),
                                        ToneScore = reader.GetDecimal(2),
                                        ClientID = reader.GetInt32(3),
                                        UserID = reader.GetInt32(4)

                                    };
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        public static bool CheckHash(int UserID, int ClientID)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM USERANDCLIENT WHERE USERID = ? AND CLIENTID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {

                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = UserID;
                        cmd.Parameters.Add("CLIENTID", OdbcType.Int).Value = ClientID;
                        
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                if (reader.GetInt32(0) > 0) {
                                    value = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return value;
        }

        private void FromReader(OdbcDataReader reader)
        {
            ID = reader.GetInt32(0);
            nickname = reader.GetString(1);
            computerName = reader.GetString(2);
            currentIP = reader.GetString(3);
            os = reader.GetString(4);
            lastStatus = reader.GetDateTime(5);
            clientHash = reader.GetString(6);
            HaveDetails = true;
        }
    }
}