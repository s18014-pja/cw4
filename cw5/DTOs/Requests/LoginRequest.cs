using System;
using System.ComponentModel.DataAnnotations;

namespace cw5.DTOs.Requests
{
    public class LoginRequest : Attribute
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Haslo { get; set; }

    }
}