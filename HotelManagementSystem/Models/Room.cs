using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        public string Type { get; set; }

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }
        public string? ImageUrl { get; set; }

    }
}
