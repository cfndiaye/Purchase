using System;
using System.ComponentModel.DataAnnotations;

namespace PurchaseShared.Models
{
    public class AuthenticationUserModel
    {
        [Required(ErrorMessage = "Email adresse est requis")]
        public string Email { get; set; }
        //[Required(ErrorMessage = "Username  est requis")]
        //public string UserName { get; set; }
        [Required(ErrorMessage = "Mot de passe est requis")]
        public string Password { get; set; }

    }

    public class AuthenticatedUserModel
    {
        public string Access_Token { get; set; }
        public string UserName { get; set; }

    }
}

