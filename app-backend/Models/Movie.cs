using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace app_backend.Models
{
    public class Movie
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Poster { get; set; } = string.Empty;

        public double Price { get; set; }

        [JsonIgnore]
        public List<RentedMovie> RentedMovies { get; set; }

        public List<Review> Reviews { get; set; }
    }
}
