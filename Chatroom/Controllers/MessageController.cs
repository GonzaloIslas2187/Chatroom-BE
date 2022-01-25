using AutoMapper;
using Chatroom.DTOs;
using Chatroom.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chatroom.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly IMapper _mapper;

        public MessageController(IMessageService messageService, IMapper mapper)
        {
            _messageService = messageService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var messages = await _messageService.GetLatestMessagesAsync();

            return Ok(_mapper.Map<IEnumerable<MessageDTO>>(messages));
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteAll()
        {
            await _messageService.DeleteAllAsync();

            return NoContent();
        }
    }
}
