using System;
using System.Data.Odbc;

namespace dissertation.ObjectModel
{
    public class User
    {
        private bool HaveDetails = false;
        private const string Fields = "USERID, FIRSTNAME, LASTNAME, EMAIL, USERPASSWORD ";

        #region DBFields

        public int ID { set; get; }

        public string FirstName
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return firstName;
            }
        }
        protected string firstName;

        public string LastName
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return lastName;
            }
        }
        protected string lastName;

        public string Email
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return email;
            }
        }
        protected string email;

        public string UserPassword
        {
            get
            {
                if (!HaveDetails) GetDetails();
                return userPassword;
            }
        }
        protected string userPassword;

        #endregion

        private User() { }

        public User(int id)
        {
            ID = id;
        }

        public static User Get(string email)
        {
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = $"SELECT {Fields} FROM USERDETAILS WHERE EMAIL = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("USERID", OdbcType.VarChar).Value = email;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                User u = new User();
                                u.FromReader(reader);
                                return u;
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

        public static bool Insert(string firstName, string lastName, string email, string password)
        {
            bool value = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertUserDetails ?,?,?,?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("USERID", OdbcType.VarChar).Value = firstName;
                        cmd.Parameters.Add("USERID", OdbcType.VarChar).Value = lastName;
                        cmd.Parameters.Add("USERID", OdbcType.VarChar).Value = email; 
                        cmd.Parameters.Add("USERID", OdbcType.VarChar).Value = password;
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
                    string query = $"SELECT {Fields} FROM USERDETAILS WHERE USERID = ?";
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("USERID", OdbcType.Int).Value = ID;
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

        private void FromReader(OdbcDataReader reader)
        {
            ID = reader.GetInt32(0);
            firstName = reader.GetString(1);
            lastName = reader.GetString(2);
            email = reader.GetString(3);
            userPassword = reader.GetString(4);
            HaveDetails = true;
        }
    }
}
