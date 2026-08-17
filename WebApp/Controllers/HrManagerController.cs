using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "HrManagerOnly")]
    public class HrManagerController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetHrManagerData()
        {
            return Ok(new
            {
                title =
                    "HR Manager Dashboard",

                message =
                    "Welcome to the HR Manager section.",

                department =
                    "Human Resources",

                role =
                    "Manager",

                requestedBy =
                    User.Identity?.Name,
                asOf = DateTime.UtcNow,

                employees = new[]
                {
                    new
                    {
                        id = 1,
                        name = "Ravi",
                        status = "Active"
                    },

                    new
                    {
                        id = 2,
                        name = "Priya",
                        status = "Active"
                    },

                    new
                    {
                        id = 3,
                        name = "Suresh",
                        status = "On Leave"
                    }
                }
            });
        }
    }
}