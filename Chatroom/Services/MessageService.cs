using Chatroom.Models;
using Chatroom.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Chatroom.Services
{
    public class MessageService : IMessageService
    {
        private readonly DbContext.AppDbContext _dbContext;

        public MessageService(DbContext.AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task CreateMessageAsync(Message message)
        {
            await _dbContext.Messages.AddAsync(message);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Entry(message).Reference(m => m.User).LoadAsync();
        }

        public async Task DeleteAllAsync()
        {
            _dbContext.RemoveRange(_dbContext.Messages);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<Message>> GetLatestMessagesAsync(int count = 50)
        {
            return await _dbContext.Messages
                .Include(m => m.User)
                .OrderByDescending(o => o.Date)
                .Take(count)
                .OrderBy(o => o.Date)
                .ToListAsync();
        }
    }
}
