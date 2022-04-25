using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    /// <summary>
    /// model that generates necessary fields which are email and password for user to login.
    /// </summary>
    /// <returns></returns>
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }
        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { set; get; }
    }
}