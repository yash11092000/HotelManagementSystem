using HotelManagementSystem.Context;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
                UserId = Convert.ToInt32(HttpContext.Session.GetString("UserId")),  // Assuming we saved user id in session at login
                CheckInDate = checkIn,
                CheckOutDate = checkOut,
                PaymentStatus = "Pending"
            };

            _context.Bookings.Add(booking);
            _context.SaveChanges();

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
        public IActionResult MyBookings()
        {
            var userId = HttpContext.Session.GetString("UserId"); // Get UserId from session
            if (userId == null)
            {
                return RedirectToAction("Login", "Account"); // Redirect to Login if UserId not found
            }

            var bookings = _context.Bookings
                                   .Where(b => b.UserId == int.Parse(userId))
                                   .Include(b => b.Users)
                                   .Include(b => b.Room)
                                   .ToList();

            return View(bookings);
        }
        public IActionResult CancelBooking(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            booking.PaymentStatus = "Canceled"; // Update the booking status
            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }
        public IActionResult MakePayment(int bookingId)
        {
            var booking = _context.Bookings.FirstOrDefault(b => b.Id == bookingId);
            if (booking == null)
            {
                return NotFound();
            }

            // Implement payment gateway integration here

            booking.PaymentStatus = "Paid"; // Update payment status after successful transaction
            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }
    }
}
