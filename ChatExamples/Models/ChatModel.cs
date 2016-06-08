using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ChatExamples.Models
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext() : base("name=DefaultConnection")
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Message> Messages { get; set; }
    }

    public class Message
    {
        [Key]
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string MessageContent { get; set; }
        public DateTime CreatedDate { get; set; }

        public virtual User User { get; set; }
    }

    public class User
    {
        [Key]
        public string UserName { get; set; }
        public ICollection<Connection> Connections { get; set; }
        public virtual ICollection<Group> Groups { get; set; }
        public virtual ICollection<Message> Message { get; set; }
    }

    public class Connection
    {
        [Key]
        public string Id { get; set; }
        public string UserAgent { get; set; }
        public bool Connected { get; set; }
    }

    public class Group
    {
        [Key]
        public string GroupName { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}