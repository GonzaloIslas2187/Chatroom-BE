using AutoMapper;
using Chatroom.DTOs;
using Chatroom.Models;
using Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Chatroom.Services
{
    public class ChatroomService : Hub, IChatroomService
    {
        private readonly IMessageService _messageService;
        private readonly UserManager<Credential> _userManager;
        private readonly IMapper _mapper;

        public ChatroomService(
            IMessageService messageService,
            UserManager<Credential> userManager,
            IMapper mapper)
        {
            _messageService = messageService;
            _userManager = userManager;
            _mapper = mapper;
        }

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task PublishMessage(string content)
        {
            var user = await _userManager.FindByNameAsync(Context.User.Identity.Name);

            var message = await SaveMessage(content, user);

            await SendMessages(message);
        }

        private async Task SendMessages(Message message)
        {
            var messageDto = _mapper.Map<MessageDTO>(message);

            if (messageDto.Content.StartsWith("/"))
            {
                await Clients.All.SendAsync("MessageBot", messageDto);
            }

            await Clients.All.SendAsync("SendMessages", messageDto);
        }

        private async Task<Message> SaveMessage(string content, Credential user)
        {
            var message = new Message(content, user);
            await _messageService.CreateMessageAsync(message);
            return message;
        }
    }
}
