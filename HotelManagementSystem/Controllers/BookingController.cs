using HotelManagementSystem.Context;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagementSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;
        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult AvailableRooms()
        {
            var rooms = _context.Rooms.Where(r => r.IsAvailable).ToList();
            return View(rooms);
        }

        // GET
        public IActionResult BookRoom(int roomId)
        {
            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }

        // POST
        [HttpPost]
        public IActionResult BookRoom(int roomId, DateTime checkIn, DateTime checkOut)
        {
            if (checkIn >= checkOut)
            {
                ModelState.AddModelError("", "Check-out must be after check-in date.");
                return View();
            }

            var booking = new Booking
            {
                RoomId = roomId,
                CustomerId = Convert.ToInt32(HttpContext.Session.GetString("UserId")),  // Assuming we saved user id in session at login
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                PaymentStatus = "Pending"
            };

            _context.Bookings.Add(booking);

            // Mark room unavailable
            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);
            room.IsAvailable = false;

            _context.SaveChanges();

            return RedirectToAction("BookingSuccess");
        }

        public IActionResult BookingSuccess()
        {
            return View();
        }

    }
}
