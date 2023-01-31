using app_backend.Models;

namespace app_backend
{
    public class MoviePage : Page
    {
        public List<Movie> Movies { get; set; } = new List<Movie>();
    }
}
