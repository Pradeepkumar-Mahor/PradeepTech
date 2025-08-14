using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PradeepTech.Auth.API.Services;

namespace PradeepTech.Auth.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JWTController : ControllerBase
    {
        private IJwtService _jwtService;

        public JWTController(IJwtService jwtService)
        {
            _jwtService = jwtService;
        }
    }
}