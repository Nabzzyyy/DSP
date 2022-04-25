using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace dissertation.Models
{
    /// <summary>
    /// Register model generates fields for neccessary information
    /// </summary>
    /// <returns></returns>
    public class RegisterModel
    {
        [Required(ErrorMessage = "First name is required")]
        public string FirstName { set; get; }

        [Required(ErrorMessage = "Last name is required")]
        public string LastName { set; get; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { set; get; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { set; get; }

        [Required(ErrorMessage = "Password confirmination is required")]
        [Compare("Password", ErrorMessage = "Password Must Match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { set; get; }
    }
}