using System.Threading.Tasks;

namespace Chatroom.Services.Interfaces
{
    public interface IChatroomService
    {
        Task PublishMessage(string content);
    }
}
