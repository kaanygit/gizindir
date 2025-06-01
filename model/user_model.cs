using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gizindir.model
{
    public class UserModel
    {
        public int Id { get; set; }

        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string FullName { get; set; } = "";
        public string Gender { get; set; } = "";
        public string InterestedIn { get; set; } = "";
        public DateTime BirthDate { get; set; }
        public string Bio { get; set; } = "";
        public string ProfileImageUrl { get; set; } = "";
        public DateTime CreatedAt { get; set; }
    }

}
