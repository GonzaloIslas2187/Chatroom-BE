using Chatroom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace Chatroom.DbContext
{
    public class AppDbContext : IdentityDbContext<Credential, IdentityRole<Guid>, Guid>
    {
        protected AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
                
        public virtual DbSet<Message> Messages { get; set; }
    }
}
