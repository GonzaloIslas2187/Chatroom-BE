using Microsoft.AspNetCore.Identity;
using System;

namespace Chatroom.Models
{
    public class Credential : IdentityUser<Guid>
    {
        public Credential(string userName) : base(userName) { }
    }
}
