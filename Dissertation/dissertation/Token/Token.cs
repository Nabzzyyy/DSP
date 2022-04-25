using dissertation.Models;
using System;
using System.Data.Odbc;
namespace dissertation.ObjectModel
{
    public class Token
    {

        private static Token token = null;


        private Token() { }


        /// <summary>
        /// Grab the instance of the token object.
        /// </summary>
        /// <returns></returns>
        public static Token getInstance()
        {
            if (token == null)
            {
                token = new Token();
                return token;
            }
            else
            {
                return token;
            }
        }

        /// <summary>
        /// Generate a token based.
        /// </summary>
        /// <param name="userId">Id of the user account</param> 
        /// <param name="procedure">Procedure of the token for example: CE for Confirm Email</param>
        /// <returns></returns>
        public string GenerateToken(int userId, string procedure) 
        {
            string token = "";
            Random rnd = new Random();
            // stage 1
            string num = rnd.Next(1000000000, int.MaxValue).ToString();

            // stage 2
            num = Base64Handler.Encoder(num);
            token = $"NABZY, {procedure}, {userId}, {num}";

            //stage 3 (token)
            token = Base64Handler.Encoder(token);

            return token;
        }

        /// <summary>
        /// Delete token from the database
        /// </summary>
        /// <param name="token"></param>
        public void deleteToken(int userID)
        {
            string query = "DELETE FROM TOKENS WHERE USERID = ?"; 
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("TOKEN", OdbcType.Int).Value = userID;
                        cmd.ExecuteNonQuery();
                    }
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
        }

        /// <summary>
        /// Decode the token and returned in parts but serparated by comma ','
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string DecodeToken(string token)
        {
            if (token == null || token.Trim() == "") return null;
            var decoded = Base64Handler.Decoder(token);
            var separate = decoded.Split(',');
            decoded = separate[0] + "," + separate[1] + "," + separate[2] + "," + Base64Handler.Decoder(separate[3]);

            return decoded;
        }

        
        /// <summary>
        /// Add token to the database
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public Boolean AddToken(long userId, string token, DateTime expiry)
        {
            var retVal = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "EXEC SP_InsertTokens ?,?,?;"; 
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("TOKEN", OdbcType.VarChar).Value = token;
                        cmd.Parameters.Add("EXPIRYDATETIME", OdbcType.DateTime).Value = expiry;
                        cmd.Parameters["EXPIRYDATETIME"].Scale = 7;
                        cmd.Parameters.Add("USERID", OdbcType.BigInt).Value = userId;
                        cmd.ExecuteNonQuery();
                        retVal = true;
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("SQL", "AddToken() Error: " + ex.ToString());
                    retVal = false;
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return retVal;
        }


        public bool ValidateLoggingin(string token, int userId)
        {
            var retVal = false;
            OdbcConnection sql = null;
            if (DBClass.GetInstance().OpenConn(ref sql))
            {
                try
                {
                    string query = "SELECT COUNT(*) FROM TOKENS WHERE USERID = ? AND TOKEN = ? AND EXPIRYDATETIME > GETDATE()"; //CHANGE TO CORRECT DB VARIABLE NAME & TABLE NAME
                    using (var cmd = new OdbcCommand(query, sql))
                    {
                        cmd.Parameters.Add("USERID", OdbcType.Int).Value = userId;
                        cmd.Parameters.Add("TOKEN", OdbcType.VarChar).Value = token;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.HasRows)
                            {
                                retVal = reader.GetInt32(0) > 0 ? true : false;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logging.Log.Error("SQL", "ValidateLoggingin() Error: " + ex.ToString());
                }
                finally
                {
                    DBClass.GetInstance().CloseConn(ref sql);
                }
            }
            return retVal;
        }
    }
}
