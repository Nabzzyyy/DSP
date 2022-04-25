using System.ComponentModel.DataAnnotations;

namespace dissertation.Models
{
    public class Login
    {
        [Required]
        public string email { set; get; }

        [Required]
        public string password { set; get; }
    }
}
