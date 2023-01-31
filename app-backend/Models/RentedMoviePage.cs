using app_backend.Models;

namespace app_backend
{
    public class RentedMoviePage : Page
    {
        public List<RentedMovieDto> RentedMovies { get; set; } = new List<RentedMovieDto>();
    }
}
