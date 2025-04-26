using HotelManagementSystem.Context;
using HotelManagementSystem.Models;
using Microsoft.EntityFrameworkCore;


namespace HotelManagementSystem.Repository
{
    public class RoomRepository : IRoomRepository
    {
        private readonly ApplicationDbContext _context;
        public RoomRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddNewRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            int result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<List<Room>> GetAllRooms()
        {
            List<Room> rooms = await _context.Rooms.ToListAsync();
            return rooms;
        }
    }
}
