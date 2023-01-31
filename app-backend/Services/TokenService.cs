using app_backend.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace app_backend.Services
{
    public interface ITokenService
    {
        string CreateToken(User user);
        Token? ValidateToken(string authorization);
        User GetCurrentUser();
    }

    public class TokenService : ITokenService
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor HttpContextAccessor;

        public TokenService(IConfiguration _Configuration, IHttpContextAccessor _HttpContextAccessor)
        {
            Configuration = _Configuration;
            HttpContextAccessor = _HttpContextAccessor;
        }

        public string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:Key")));
            System.Diagnostics.Debug.WriteLine(Configuration.GetValue<string>("Jwt:Key"));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var jwt = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            string token = new JwtSecurityTokenHandler().WriteToken(jwt);

            return token;
        }

        public Token? ValidateToken(string authorization)
        {
            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                try
                {
                    var parameter = headerValue.Parameter;
                    var jwt = new JwtSecurityTokenHandler();
                    System.Diagnostics.Debug.WriteLine(parameter);
                    var validationParams = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Configuration.GetValue<string>("Jwt:Key"))),
                    ValidateIssuer = false,
                        ValidateAudience = false,
                    };

                    jwt.ValidateToken(parameter, validationParams, out SecurityToken validatedToken);

                    var jwtToken = (JwtSecurityToken)validatedToken;

                    int userId = int.Parse(jwtToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                    Role role = (Role)Enum.Parse(typeof(Role), jwtToken.Claims.First(c => c.Type == ClaimTypes.Role).Value);

                    //Token token = new(userId, role);
                    Token token = new Token
                    {
                        Id = userId,
                        Role = role
                    };

                    return token;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public User GetCurrentUser()
        {
            var identity = HttpContextAccessor.HttpContext.User.Identity as ClaimsIdentity;

            if (identity != null)
            {
                var userClaims = identity.Claims;

                return new User
                {
                    Id = int.Parse(userClaims.FirstOrDefault(o => o.Type == ClaimTypes.NameIdentifier)?.Value),
                    Role = (Role)Enum.Parse(typeof(Role), userClaims.FirstOrDefault(o => o.Type == ClaimTypes.Role)?.Value)
                };
            }
            return null;
        }
    }

}
