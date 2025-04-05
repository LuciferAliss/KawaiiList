using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KawaiiList.Models
{
    public class User
    {
        public int Id { get; set; }
        public required string Login { get; set; }
        public required string PasswordHash { get; set; }
        public required string Salt { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastLogin { get; set; }

        public List<AuthToken> Tokens { get; set; } = new();
    }
}
