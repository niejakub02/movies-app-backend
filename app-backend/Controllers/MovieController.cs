using app_backend.Data;
using app_backend.Models;
using app_backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace app_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        private readonly DataContext DataContext;
        private readonly IConfiguration Configuration;
        private readonly ITokenService TokenService;
        private readonly IMovieService MovieService;
        private readonly IReviewService ReviewService;

        public MovieController(IConfiguration _Configuration, ITokenService _TokenService, DataContext _DataContext, IMovieService _MovieService, IReviewService _ReviewService)
        {
            DataContext = _DataContext;
            Configuration = _Configuration;
            TokenService = _TokenService;
            MovieService = _MovieService;
            ReviewService = _ReviewService;
        }

        [HttpPost]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<Movie>> AddMovie([FromHeader] string authorization, MovieDto request)
        {
            try
            {
                Movie? movie = await MovieService.AddMovie(request);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Role.USER)},{nameof(Role.ADMIN)}")]
        public async Task<ActionResult<Movie>> GetMovieById(int id)
        {
            try
            {
                Movie? movie = await MovieService.GetMovieById(id);
                return Ok(movie);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("Search/{page}")]
        [Authorize(Roles = $"{nameof(Role.USER)},{nameof(Role.ADMIN)}")]
        public async Task<ActionResult<MoviePage>> GetSearchedMoviePage([FromHeader] string authorization, int page, [FromBody] string input)
        {
            try
            {
                MoviePage moviePage = await MovieService.GetSearchedMoviePage(page, input, false);
                return Ok(moviePage);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<bool>> DeleteMovie(int id)
        {
            try
            {
                bool response = await MovieService.DeleteMovie(id);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        [Authorize(Roles = nameof(Role.ADMIN))]
        public async Task<ActionResult<bool>> EditMovie(int id, MovieDto request)
        {
            try
            {
                bool response = await MovieService.EditMovie(id, request);
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{id}/Review")]
        [Authorize(Roles = nameof(Role.USER))]
        public async Task<ActionResult<Review>> AddReview(int id, ReviewDto request)
        {
            try
            {
                Review? review = await ReviewService.AddReview(id, request);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}/Review")]
        [Authorize(Roles = nameof(Role.USER))]
        public async Task<ActionResult<Review>> GetReview(int id)
        {
            try
            {
                Review? review = await ReviewService.GetReview(id);
                return Ok(review);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
