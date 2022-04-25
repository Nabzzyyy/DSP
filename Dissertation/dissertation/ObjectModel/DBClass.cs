using System.Data.Odbc;

namespace dissertation.ObjectModel
{
    public class DBClass
    {

        private static DBClass Instance = null;

        private DBClass() { }

        public static DBClass GetInstance()
        {
            if (Instance == null) Instance = new DBClass();
            return Instance;
        }

        // ref = pass by reference
        public bool OpenConn(ref OdbcConnection sql)
        {
            sql = new OdbcConnection(System.Configuration.ConfigurationManager.ConnectionStrings["SQLConnStr"].ConnectionString);
            sql.Open();
            return true;
        }

        public bool CloseConn(ref OdbcConnection sql)
        {
            sql.Close();
            sql.Dispose();
            return true;
        }
    }
}
