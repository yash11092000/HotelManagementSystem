using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Staff
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Role { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }
    }
}
