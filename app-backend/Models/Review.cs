using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace app_backend.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public byte Rating { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        
        public int UserId { get; set; }
        //[JsonIgnore]
        public User User { get; set; }
        
        public int MovieId { get; set; }
        [JsonIgnore]
        public Movie Movie { get; set; }
    }
}
