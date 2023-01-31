using app_backend.Data;
using app_backend.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace app_backend.Services
{
    public interface IUserService
    {
        Task<User> AddUser(UserDto userDto);
        Task<string> SignInUser(UserDto userDto);
        Task<User> GetUserById(int Id);
        Task<User> AddMovieToUser(int movieId);
        Task<UserPage> GetUserPage(int page, double moviesPerPage = 6f);
        Task<User> BlockUnblockUser(int id, bool block);
    }

    public class UserService : IUserService
    {
        private readonly DataContext DataContext;
        private readonly ITokenService TokenService;

        public UserService(DataContext _DataContext, ITokenService _TokenService)
        {
            DataContext = _DataContext;
            TokenService = _TokenService;
        }

        public async Task<User> AddUser(UserDto userDto)
        {
            if (userDto.Password.Length < 6) throw new Exception("Password is too short. Password must be at least 6 characters long");

            CreatePasswordHash(userDto.Password, out byte[] passwordHash, out byte[] passwordSalt);
            User user = new User
            {
                Username = userDto.Username,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = Role.USER
            };
            DataContext.Users.Add(user);
            await DataContext.SaveChangesAsync();
            return user;
        }

        public async Task<string> SignInUser(UserDto userDto)
        {
            User? user = await DataContext.Users.FirstOrDefaultAsync(u => u.Username == userDto.Username);
            if (user == null) throw new Exception("User not found");

            bool isPasswordCorrect = CheckPassword(userDto.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordCorrect) throw new Exception("Incorrect password");

            if (user.isBlocked) throw new Exception("BLOCKED");

            string token = TokenService.CreateToken(user);
            return token;
        }

        public async Task<User> GetUserById(int Id)
        {
            User? user = await DataContext.Users.Where(u => u.Id == Id)
            .Include(u => u.RentedMovies)
            .FirstOrDefaultAsync();

            if (user == null) throw new Exception("User not found");

            return user;
        }

        public async Task<User> AddMovieToUser(int movieId)
        {
            User userDetails = TokenService.GetCurrentUser();

            User? user = await DataContext.Users.Where(u => u.Id == userDetails.Id)
                .Include(u => u.RentedMovies)
                .FirstOrDefaultAsync();

            if (user == null) throw new Exception("User not found");

            Movie? movie = await DataContext.Movies.FindAsync(movieId);

            if (movie == null) throw new Exception("Movie not found");

            RentedMovie rentedMovie = new RentedMovie
            {
                MovieId = movieId,
                User = user,
                UserId = userDetails.Id,
                Movie = movie,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMinutes(1)
            };

            user.RentedMovies.Add(rentedMovie);
            await DataContext.SaveChangesAsync();

            return user;
        }

        public async Task<UserPage> GetUserPage(int page, double moviesPerPage = 6f)
        {
            User user = TokenService.GetCurrentUser(); // admin

            List<User> filtredUsers = await DataContext.Users
                .Where(u => u.Role == Role.USER)
                .OrderBy(u => u.Username)
                .ToListAsync();

            List<User> users = filtredUsers.Skip((page - 1) * (int)moviesPerPage)
                .Take((int)moviesPerPage)
                .ToList();

            var pagesCount = Math.Ceiling(filtredUsers.Count() / moviesPerPage);

            var rentedMoviePage = new UserPage
            {
                Users = users,
                CurrentPage = page,
                Pages = (int)pagesCount
            };

            return rentedMoviePage;
        }

        public async Task<User> BlockUnblockUser(int id, bool block)
        {
            User? user = await DataContext.Users
                .FindAsync(id);

            if (user == null) throw new Exception("User not found");

            user.isBlocked = block;
            await DataContext.SaveChangesAsync();

            return user;
        }

        static private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        static private bool CheckPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
