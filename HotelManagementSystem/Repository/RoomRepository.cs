using HotelManagementSystem.Context;
using HotelManagementSystem.Models;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async  Task<int> DeleteRoom(Room room)
        {
            _context.Rooms.Remove(room);
            return await _context.SaveChangesAsync();

        }

        public async Task<Room> findRoom(int? id)
        {
            var room =  await _context.Rooms.FindAsync(id);
            if (room == null) {
                return null;
            }
            return room;
        }

        //public Task<int> DeleteRoom(int? id)
        //{
        //    _context.Rooms.FirstOrDefaultAsync(m => m.Id == id);
        //}

        public async Task<List<Room>> GetAllRooms()
        {
            List<Room> rooms = await _context.Rooms.ToListAsync();
            return rooms;
        }

        public async Task<Room> GetRoomDetailsById(int id)
        {
            Room? room = await _context.Rooms.Where(x => x.Id == id).FirstOrDefaultAsync();
            return room;
        }

        public bool RoomExists(int id)
        {
            return _context.Rooms.Any(e => e.Id == id);
        }

        public async Task<int> UpdateRoom(Room room)
        {
            _context.Update(room);
            int result = await _context.SaveChangesAsync();
            return result;
        }
    }
}
