using HotelManagementSystem.Models;
using HotelManagementSystem.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HotelManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
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

        [HttpGet]
        public async Task<ActionResult> EditRoom(int id)
        {
            var Rooms = await _roomRepository.GetRoomDetailsById(id);
            return View(Rooms);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoom(int id, Room room, IFormFile RoomImage)
        {
            if (id != room.Id)
            {
                return NotFound();
            }
            ModelState.Remove("RoomImage");

            if (ModelState.IsValid)
            {
                try
                {

                    var existingRoom = await _roomRepository.findRoom(id);

                    if (existingRoom == null)
                        return NotFound();

                    // Update properties
                    existingRoom.RoomNumber = room.RoomNumber;
                    existingRoom.Type = room.Type;
                    existingRoom.Price = room.Price;
                    existingRoom.IsAvailable = room.IsAvailable;

                    if (RoomImage != null && RoomImage.Length > 0)
                    {
                        if (!string.IsNullOrEmpty(existingRoom.ImageUrl))
                        {
                            var oldImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", existingRoom.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Upload new image if selected
                        if (RoomImage != null && RoomImage.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString("N") + Path.GetExtension(RoomImage.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/RoomImages", fileName);

                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await RoomImage.CopyToAsync(stream);
                            }

                            existingRoom.ImageUrl = "/RoomImages/" + fileName;
                        }
                    }
                    var result = _roomRepository.UpdateRoom(existingRoom);

                    return RedirectToAction("Rooms");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RoomExists(room.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return View(room);
        }

        private bool RoomExists(int id)
        {
            return _roomRepository.RoomExists(id);
        }


        [HttpPost]
        public async Task<IActionResult> DeleteRoom(int? id)
        {
            var room = await _roomRepository.findRoom(id);

            if (room == null)
            {
                return NotFound();
            }
            int result = await _roomRepository.DeleteRoom(room);
            if (result > 0)
            {
                if (!string.IsNullOrEmpty(room.ImageUrl))
                {
                    var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", room.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
            }
            return Json(new { success = true });
        }
    }
}
