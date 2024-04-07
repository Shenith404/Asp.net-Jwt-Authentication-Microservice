using Authentication.Core.DTOs;
using Authentication.jwt;
using AutoMapper;
using ERP.Authentication.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Authentication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseController
    {
        public AdminController(IJwtTokenHandler jwtTokenHandler, UserManager<UserModel> userManager, IMapper mapper) : base(jwtTokenHandler, userManager, mapper)
        {
        }

        [HttpPost]
        [Route("getUsers-details")]
        //[Authorize] should change
        public async Task<IActionResult> GetUsers([FromBody] string? searchString)
        {
            var users = _userManager.Users.ToList();

            if (users == null || !users.Any())
            {
                return BadRequest("User List is Empty");
            }

            if (string.IsNullOrEmpty(searchString))
            {
                return Ok(users);
            }

            var searchResult = users.Where(u =>
                u.UserName!.Contains(searchString, StringComparison.OrdinalIgnoreCase) || // Search by username
                u.Email!.Contains(searchString, StringComparison.OrdinalIgnoreCase)   // Search by email
                  ).ToList();

            //map the result
            var mapResutls =_mapper.Map<List<UserModelResponseDTO>>(searchResult);
          
            return Ok(mapResutls);
        }

        [HttpGet]
        [Route("test")]
        public async Task<IActionResult> test()
        {
            return Ok("ok");

        }

        [HttpGet]
        [Route("Delete-User")]
        //[Authorize] should change
        public async Task<IActionResult> DeleteUser([FromBody] string email)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);

                //check user is exist or not
                if (user == null)
                {
                    return BadRequest("User is not exist");

                }
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return Ok("User is Deleted");

                }
                return BadRequest("Can't Delete User");


            }
            return BadRequest("Invalid email");
        }


    }


}
