using Chatroom.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatroom.Services.Interfaces
{
    public interface IMessageService
    {
        Task CreateMessageAsync(Message message);
        Task<IEnumerable<Message>> GetLatestMessagesAsync(int count = 50);
        Task DeleteAllAsync();
    }
}
