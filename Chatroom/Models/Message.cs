using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chatroom.Models
{
    public class Message
    {
        [Key]
        public Guid MessageId { get; set; }

        public DateTime Date { get; set; }

        [Required]
        [MaxLength(10000)]
        public string Content { get; set; }

        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }

        [Required]
        public Credential User { get; set; }

        public Message(string content, Credential user)
        {
            this.Content = content;
            this.User = user;
            this.Date = DateTime.UtcNow;
        }

        public Message() { }
    }
}
