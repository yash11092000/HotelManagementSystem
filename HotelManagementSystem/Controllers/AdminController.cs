using HotelManagementSystem.Context;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Dashboard()
        {
            var totalBookings = _context.Bookings.Count();
            var activeUsers = _context.Users.Count(x => x.Role != "Admin"); // Assuming you have an IsActive flag
            var canceledBookings = _context.Bookings.Count(b => b.PaymentStatus == "Canceled");

            // Passing dynamic data to the view using ViewBag
            ViewBag.TotalBookings = totalBookings;
            ViewBag.ActiveUsers = activeUsers;
            ViewBag.CanceledBookings = canceledBookings;
            return View();
        }
        public IActionResult ManageBookings()
        {
            var bookings = _context.Bookings
                .Include(b => b.Users) // Include the related User
                .Include(b => b.Room)  // Include the related Room
                .ToList();  // Fetch all bookings

            return View(bookings);
        }

        // Action to update the booking status (or any other booking detail)
        [HttpPost]
        public IActionResult UpdateBookingStatus(int bookingId, string status)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                booking.PaymentStatus = status; // Change booking status
                _context.SaveChanges();  // Save changes to the database
            }

            return RedirectToAction("ManageBookings");  // Redirect to the bookings page
        }

        // Action to delete a booking (Optional)
        [HttpPost]
        public IActionResult DeleteBooking(int bookingId)
        {
            var booking = _context.Bookings.Find(bookingId);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);  // Remove the booking
                _context.SaveChanges();            // Save changes to the database
            }

            return RedirectToAction("ManageBookings");  // Redirect to the bookings page
        }
    }
}
