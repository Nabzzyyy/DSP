using System;
using System.Text;

namespace dissertation.Models
{
    public class Base64Handler
    {

        public static string Encoder(string encode)
        {
            var bytes = Encoding.UTF8.GetBytes(encode);
            return Convert.ToBase64String(bytes);
        }

        public static string Decoder(string decode)
        {
            var base64EncodedBytes = Convert.FromBase64String(decode);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
