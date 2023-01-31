using app_backend.Data;
using app_backend.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using app_backend.Services;
using Microsoft.AspNetCore.Authorization;

namespace app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration Configuration;
        private readonly DataContext DataContext;
        private readonly ITokenService TokenService;
        private readonly IUserService UserService;
        private readonly IMovieService MovieService;

        public UserController(IConfiguration _Configuration, ITokenService _TokenService, IUserService _UserService, DataContext _DataContext, IMovieService _MovieService)
        {
            Configuration = _Configuration;
            TokenService = _TokenService;
            UserService = _UserService;
            DataContext = _DataContext;
            MovieService = _MovieService;
        }

        [HttpPost("SignUp")]
        public async Task<ActionResult<User>> SignUp(UserDto request)
        {
            try
            {
                User? user = await UserService.AddUser(request);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("SignIn")]
        public async Task<ActionResult<string>> SignIn(UserDto request)
        {
            try
            {
                string? token = await UserService.SignInUser(request);
                return Ok(token);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{Id}")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<User>> GetUser(int Id)
        {
            try
            {
                User? user = await UserService.GetUserById(Id);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("Verify")]
        [Authorize(Roles = $"{nameof(Role.USER)},{nameof(Role.ADMIN)}")]
        public async Task<ActionResult<User>> Verify()
        {
            User user = TokenService.GetCurrentUser();
            return Ok(user);
        }

        [HttpGet("Rent/{movieId}")]
        [Authorize(Roles = nameof(Role.USER))]
        public async Task<ActionResult<User>> RentMovie(int movieId)
        {
            try
            {
                User? user = await UserService.AddMovieToUser(movieId);
                return Ok(user);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Movies/{page}")]
        [Authorize(Roles = $"{nameof(Role.USER)},{nameof(Role.ADMIN)}")]
        public async Task<ActionResult<MoviePage>> GetSearchedMoviePage(int page, [FromBody] string input)
        {
            try
            {
                MoviePage moviePage = await MovieService.GetSearchedMoviePage(page, input, true);
                return Ok(moviePage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("History/{page}")]
        [Authorize(Roles = nameof(Role.USER))]
        public async Task<ActionResult<RentedMoviePage>> GetHistory(int page)
        {
            try
            {
                RentedMoviePage rentedMoviePage = await MovieService.GetUsersHistoryPage(page);
                return Ok(rentedMoviePage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Users/{page}")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<RentedMoviePage>> GetUsers(int page)
        {
            try
            {
                UserPage userPage = await UserService.GetUserPage(page);
                return Ok(userPage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}/Block")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<bool>> BlockUnblockUser(int id, [FromBody] bool block)
        {
            try
            {
                User user = await UserService.BlockUnblockUser(id, block);
                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
