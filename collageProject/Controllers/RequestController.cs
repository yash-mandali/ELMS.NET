using collageProject.Data;
using collageProject.Services;
using Microsoft.AspNetCore.Mvc;

namespace collageProject.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class RequestController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        public RequestController(AppDbContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
    }
}
