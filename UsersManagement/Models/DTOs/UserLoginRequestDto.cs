﻿using System.ComponentModel.DataAnnotations;

namespace UsersManagement.Models.DTOs
{
    public class UserLoginRequestDto
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
