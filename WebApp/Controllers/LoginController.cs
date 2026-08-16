using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApp.Model;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        //public Credentials Credentials { get; set; }=new Credentials();
        [HttpPost(Name ="Login")]
        public async Task<IActionResult> Login([FromBody] Credentials credentials)
        {
           
            if(credentials.Username == "admin" && credentials.Password == "password")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,credentials.Username),
                    new Claim(ClaimTypes.Email,"admin@example.com")
                };
                var identity=new ClaimsIdentity(claims, "MyCookieAuth");
                var claimsPrincipal = new ClaimsPrincipal(identity);
                HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);
                return Ok(new
                {
                    message= "Login successful",
                    Name=credentials.Username
                });

            }
            return Unauthorized(new
            {
                message = "Invalid username or password."
            });
        }

        [HttpPost("logout", Name = "Logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return Ok(new
            {
                message = "Logout successful"
            });
        }
    }
}
