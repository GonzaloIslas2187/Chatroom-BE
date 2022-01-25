using System;
using System.ComponentModel.DataAnnotations;

namespace Chatroom.DTOs
{
    public class MessageDTO
    {
        [Key]
        public Guid MessageId { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Content { get; set; }

        public string UserName { get; set; }

        public DateTime Date { get; set; }
    }
}
