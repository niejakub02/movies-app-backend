using app_backend.Data;
using app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace app_backend.Services
{
    public interface IMovieService
    {
        Task<Movie> AddMovie(MovieDto movieDto);
        Task<Movie> GetMovieById(int Id);
        Task<bool> DeleteMovie(int id);
        Task<bool> EditMovie(int id, MovieDto request);
        Task<MoviePage> GetSearchedMoviePage(int page, string input, bool usersIncluded, double moviesPerPage = 8f);
        Task<RentedMoviePage> GetUsersHistoryPage(int page, double moviesPerPage = 6f);
    }

    public class MovieService : IMovieService
    {
        private readonly DataContext DataContext;
        private readonly ITokenService TokenService;

        public MovieService(DataContext _DataContext, ITokenService _TokenService)
        {
            DataContext = _DataContext;
            TokenService = _TokenService;
        }

        public async Task<Movie> AddMovie(MovieDto movieDto)
        {
            if (movieDto.Price < 0 || movieDto.Price > 100) 
                throw new Exception("Incorrect price. Price must be a value between 0,- and 100,-");

            Movie movie = new Movie
            {
                Title = movieDto.Title,
                Description = movieDto.Description,
                Category = movieDto.Category,
                Poster = movieDto.Poster,
                Price = movieDto.Price,
            };
            DataContext.Movies.Add(movie);
            await DataContext.SaveChangesAsync();
            return movie;
        }

        public async Task<Movie> GetMovieById(int id)
        {
            Movie? movie = await DataContext.Movies.FindAsync(id);

            if (movie == null) throw new Exception("Movie not found");

            return movie;
        }

        public async Task<bool> DeleteMovie(int id)
        {
            Movie? movie = await GetMovieById(id);

            if (movie == null) throw new Exception("Movie not found");

            DataContext.Movies.Remove(movie);
            await DataContext.SaveChangesAsync();

            return true;
        }

        public async Task<bool> EditMovie(int id, MovieDto movieUpdates)
        {
            Movie? movie = await GetMovieById(id);

            if (movie == null) throw new Exception("Movie not found");

            DataContext.Entry(movie).CurrentValues.SetValues(movieUpdates);
            await DataContext.SaveChangesAsync();

            return true;
        }

        public async Task<MoviePage> GetSearchedMoviePage(int page, string input, bool usersIncluded, double moviesPerPage = 8f)
        {
            User userDetails = TokenService.GetCurrentUser();

            User? user = await DataContext.Users.Where(u => u.Id == userDetails.Id)
            .Include(u => u.RentedMovies)
            .FirstOrDefaultAsync();

            if (user == null) throw new Exception("User not found");

            var movieIds = user.RentedMovies
                .Where(m => m.EndDate > DateTime.Now)
                .Select(m => m.MovieId).ToList();

            List<Movie> filtredMovies = await DataContext.Movies
                .Where(m => usersIncluded ? movieIds.Contains(m.Id) : !movieIds.Contains(m.Id))
                .Where(m => m.Title.ToLower().Contains(input.ToLower()))
                .Include(m => m.Reviews).ThenInclude(r => r.User)
                .ToListAsync();

            List<Movie> movies = filtredMovies.Skip((page - 1) * (int)moviesPerPage)
                .Take((int)moviesPerPage)
                .ToList();

            var pagesCount = Math.Ceiling(filtredMovies.Count() / moviesPerPage);

            var moviePage = new MoviePage
            {
                Movies = movies,
                CurrentPage = page,
                Pages = (int)pagesCount
            };

            return moviePage;
        }

        public async Task<RentedMoviePage> GetUsersHistoryPage(int page, double moviesPerPage = 8f)
        {
            User user = TokenService.GetCurrentUser();

            List<RentedMovieDto> filtredRentedMovies = await DataContext.Users
                .Where(u => u.Id == user.Id)
                .SelectMany(u => u.RentedMovies)
                .Include(m => m.Movie)
                .Select(m => new RentedMovieDto
                {
                    Title = m.Movie.Title,
                    EndDate = m.EndDate,
                    StartDate = m.StartDate
                })
                .OrderBy(u => u.StartDate)
                .ToListAsync();

            List<RentedMovieDto> rentedMovies = filtredRentedMovies.Skip((page - 1) * (int)moviesPerPage)
                .Take((int)moviesPerPage)
                .ToList();

            var pagesCount = Math.Ceiling(filtredRentedMovies.Count() / moviesPerPage);

            var rentedMoviePage = new RentedMoviePage
            {
                RentedMovies = rentedMovies,
                CurrentPage = page,
                Pages = (int)pagesCount
            };

            return rentedMoviePage;
        }
    }
}
