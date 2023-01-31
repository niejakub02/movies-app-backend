using System.Text.Json.Serialization;

namespace app_backend.Models
{
    public class RentedMovie
    {
        public int Id { get; set; }

        public int MovieId { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; }

        public int UserId { get; set; }
        [JsonIgnore]
        public User User { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
