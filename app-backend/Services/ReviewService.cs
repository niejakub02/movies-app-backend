using app_backend.Data;
using app_backend.Models;
using Microsoft.EntityFrameworkCore;

namespace app_backend.Services
{
    public interface IReviewService
    {
        Task<Review> AddReview(int movieId, ReviewDto reviewDto);
        Task<Review> GetReview(int movieId);
    }

    public class ReviewService : IReviewService
    {
        private readonly DataContext DataContext;
        private readonly IMovieService MovieService;
        private readonly ITokenService TokenService;

        public ReviewService(DataContext _DataContext, IMovieService _MovieService, ITokenService _TokenService)
        {
            DataContext = _DataContext;
            MovieService = _MovieService;
            TokenService = _TokenService;
        }

        public async Task<Review> AddReview(int movieId, ReviewDto reviewDto)
        {
            if (reviewDto.Rating < 0 || reviewDto.Rating > 5) throw new Exception("Incorrect rating value");

            User userDetails = TokenService.GetCurrentUser();

            Movie? movie = await DataContext.Movies.Where(m => m.Id == movieId)
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync();
            if (movie == null) throw new Exception("Movie not found");

            User? user = await DataContext.Users.FindAsync(userDetails.Id);
            if (user == null) throw new Exception("User not found");

            Review? review = await GetReview(movie.Id);

            Review newReview = new Review
            {
                Rating = reviewDto.Rating,
                Description = reviewDto.Description,
                User = user,
                Movie = movie,
                Date = DateTime.Now
            };

            if (review == null)
            {
                movie.Reviews.Add(newReview);
            }
            else
            {
                newReview = review;
                newReview.Description = reviewDto.Description;
                newReview.Rating = reviewDto.Rating;
                newReview.Date = DateTime.Now;
                DataContext.Entry(review).CurrentValues.SetValues(newReview);
            }

            await DataContext.SaveChangesAsync();

            return newReview;
        }

        public async Task<Review> GetReview(int movieId)
        {
            User userDetails = TokenService.GetCurrentUser();

            Review? review = await DataContext.Reviews
                .Where(r => (r.UserId == userDetails.Id && r.MovieId == movieId))
                .FirstOrDefaultAsync();

            if (review == null) return null;

            return review;
        }
    }
}
