using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Model;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {

        private static readonly List<AppUser> Users =
       [
           new AppUser
            {
                Username = "admin",
                Password = "password",

                Claims =
                [
                    "admin:true"
                ]
            },

            new AppUser
            {
                Username = "hruser",
                Password = "password",

                Claims =
                [
                    "department:hr"
                ]
            },

            new AppUser
            {
                Username = "hrmanager",
                Password = "password",

                Claims =
                [
                    "department:hr",
                    "role:manager"
                ]
            }
       ];

        //LOGIN
        //public Credentials Credentials { get; set; }=new Credentials();
        [HttpPost(Name ="Login")]
        public async Task<IActionResult> Login([FromBody] Credentials credentials)
        {

            var user = Users.FirstOrDefault(u => u.Username == credentials.Username && u.Password == credentials.Password);


            if (user == null)
            {
                return Unauthorized(new
                {
                    message= "Invalid username or password."
                });
            }

            var claims = new List<Claim>
             {
                 new Claim(ClaimTypes.Name,user.Username)
             };

            foreach(var claim in user.Claims)
            {
                var parts = claim.Split(':');
                if (parts.Length == 2)
                {
                    claims.Add(new Claim(parts[0], parts[1]));
                }
            }
            var identity=new ClaimsIdentity(claims, "MyCookieAuth");
            var principal = new ClaimsPrincipal(identity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(30)
               ,AllowRefresh=false
            }; 

            await HttpContext.SignInAsync("MyCookieAuth", principal, authProperties);

            return Ok(new
            {
                message =
                   "Login successful",

                name =
                   user.Username
            });

        }

        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                name =
                    User.Identity?.Name,

                isAdmin =
                    User.HasClaim(
                        "admin",
                        "true"),

                isHr =
                    User.HasClaim(
                        "department",
                        "hr"),

                isHrManager =
                    User.HasClaim(
                        "department",
                        "hr")
                    &&
                    User.HasClaim(
                        "role",
                        "manager")
            });
        }


        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(
                "MyCookieAuth");

            return Ok(new
            {
                message =
                    "Logout successful"
            });
        }
    }
}
