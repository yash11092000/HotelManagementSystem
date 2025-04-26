using HotelManagementSystem.Models;
using HotelManagementSystem.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    public class RoomController : Controller
    {
        private readonly IRoomRepository _roomRepository;
        public RoomController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }
        public async Task<IActionResult> Rooms()
        {
            var Rooms = await _roomRepository.GetAllRooms();
            return View(Rooms);
        }
        [HttpGet]
        public async Task<ActionResult> AddRooms()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddNewRooms(Room room, IFormFile RoomImage)
        {
            if (ModelState.IsValid)
            {
                if (RoomImage != null && RoomImage.Length > 0)
                {
                    var fileName = Path.GetFileName(RoomImage.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RoomImages", fileName);
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "RoomImages");

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await RoomImage.CopyToAsync(stream);
                    }

                    room.ImageUrl = "/RoomImages/" + fileName; // You must add ImagePath field in Room model
                }

                var response = await _roomRepository.AddNewRoom(room);
                if (response > 0)
                {
                    return RedirectToAction("Rooms", "Room");  // Redirect to list after adding
                }
                ViewBag.errorMsg = "Error Occured";
                return View();
            }

            return View(room); // If error, return same page
        }
    }
}
