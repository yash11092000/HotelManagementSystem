
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Repository
{
    public interface IRoomRepository
    {
        Task<int> AddNewRoom(Room room);
        Task<int> DeleteRoom(Room room);
        Task<Room> findRoom(int? id);
        Task<List<Room>> GetAllRooms();
        Task<Room> GetRoomDetailsById(int id);
        bool RoomExists(int id);
        Task<int> UpdateRoom(Room room);
    }
}
