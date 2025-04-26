
using HotelManagementSystem.Models;

namespace HotelManagementSystem.Repository
{
    public interface IRoomRepository
    {
        Task<int> AddNewRoom(Room room);
        Task<List<Room>> GetAllRooms();
    }
}
