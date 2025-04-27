﻿using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public DateTime CreatedAt { get; set; }
        [Required]
        public string Role { get; set; }

    }
}
