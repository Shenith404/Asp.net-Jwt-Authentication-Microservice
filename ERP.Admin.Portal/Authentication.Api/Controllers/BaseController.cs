using Authentication.jwt;
using AutoMapper;
using ERP.Authentication.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers
{
    public class BaseController : ControllerBase
    {
        protected readonly IJwtTokenHandler _jwtTokenHandler;
        protected readonly UserManager<UserModel> _userManager;
        protected readonly IMapper _mapper;

        public BaseController(IJwtTokenHandler jwtTokenHandler, UserManager<UserModel> userManager, IMapper mapper)
        {
            _jwtTokenHandler = jwtTokenHandler;
            _userManager = userManager;
            _mapper = mapper;
        }
    }
}
