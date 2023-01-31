using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace app_backend.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        public string Username { get; set; } = string.Empty;

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public Role Role { get; set; }

        public bool isBlocked { get; set; } = false;

        public List<RentedMovie> RentedMovies { get; set; }
    }

    public enum Role
    {
        USER,
        ADMIN
    }
}
