using Authentication.jwt;
using ERP.Authentication.Core.Entity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers
{

    //[Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {

        private readonly UserManager<UserModel> _userManager;




        public HomeController(
            UserManager<UserModel> userManager
           )
        {
            _userManager = userManager;

        }

        [HttpGet]
        public async Task<string> Index()
        {

            
            return $"this is home";
        } 
        
        [HttpGet]
        [Route("test")]
        public async Task<string> Index2()
        {

          
            return "sdfsdf";
        }
    }
}
