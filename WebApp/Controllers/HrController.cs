using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "HrOnly")]
    public class HrController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHrData()
        {
            return Ok(new
            {
                title =
                    "HR Dashboard",

                message =
                    "Welcome to the HR section.",

                department =
                    "Human Resources",

                requestedBy =
                    User.Identity?.Name,
                asOf = DateTime.UtcNow,

                employees = new[]
                {
                    new
                    {
                        id = 1,
                        name = "Ravi",
                        role = "HR Executive"
                    },

                    new
                    {
                        id = 2,
                        name = "Priya",
                        role = "HR Executive"
                    },

                    new
                    {
                        id = 3,
                        name = "Suresh",
                        role = "Recruiter"
                    }
                }
            });
        }
    }
}