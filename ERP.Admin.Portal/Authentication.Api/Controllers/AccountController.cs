using Authentication.Core.DTOs;
using Authentication.DataService.IConfiguration;
using Authentication.jwt;
using AutoMapper;

using ERP.Authentication.Core.DTOs;
using ERP.Authentication.Core.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Drawing.Printing;

namespace Authentication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : BaseController
    {
        public AccountController(IJwtTokenHandler jwtTokenHandler, UserManager<UserModel> userManager, IMapper mapper) : base(jwtTokenHandler, userManager, mapper)
        {
        }



        //Login User
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AuthenticationRequestDTO authenticationRequest)
        {
            if(ModelState.IsValid)
            {
                //check user is exist
                var existing_user = await _userManager.FindByEmailAsync(authenticationRequest.UserName);
                if (existing_user == null)
                {
                    return Unauthorized(
                          new AuthenticationResponseDTO()
                          {
                              Message = "Username is not Exist"
                          });
                 }

               
                //check is user deleted
                if(existing_user.Status != 1)
                {
                    return Unauthorized(
                         new AuthenticationResponseDTO()
                         {
                             Message = "This user is Deleted"
                         });
                }

                //check is user Locked
                var isLocked = await _userManager.IsLockedOutAsync(existing_user);
                if (isLocked==true)
                {
                    return Unauthorized(
                         new AuthenticationResponseDTO()
                         {
                             IsLocked=true,
                             Message = "This user is Locked"
                         });
                }

                //check is user Email is conformed
                if (existing_user.EmailConfirmed ==false)
                {
                    return Unauthorized(
                         new AuthenticationResponseDTO()
                         {
                             EmailConfirmed = await _userManager.IsEmailConfirmedAsync(existing_user),
                             Message = "Your Email is not Confirmed"
                         });
                }



                //check password is match
                var isCorrect = await _userManager.CheckPasswordAsync(existing_user,authenticationRequest.Password);
                if (isCorrect==false)
                {
                    return Unauthorized(
                      new AuthenticationResponseDTO()
                      {
                          Message = "Password is Incorrect"
                      });
                }



                //Get user Role from database
               
                var roles= await _userManager.GetRolesAsync(existing_user);


                //Generate token

                TokenRequestDTO tokenRequest = new TokenRequestDTO();
                tokenRequest.UserName = authenticationRequest.UserName;
                if(!roles.IsNullOrEmpty() )
                {
                    tokenRequest.Role = "Reguler";
                }
                tokenRequest.Role = roles[0];
                tokenRequest.UserId = existing_user.Id;

              
               
                var result = await _jwtTokenHandler.GenerateJwtToken(tokenRequest);

                return Ok(
                    new AuthenticationResponseDTO
                    {
                        JwtToken = result!.JwtToken,
                        RefreshToken = result!.RefreshToken,
                        ExpiresIn = result.ExpiresIn,
                        UserName = result.UserName,
                        Message = "User Login Successfully",
                        IsLocked =await _userManager.IsLockedOutAsync(existing_user),
                        EmailConfirmed = await _userManager.IsEmailConfirmedAsync(existing_user),

                    });


            }

            return Unauthorized(
              new AuthenticationResponseDTO()
              {
                  Message = "Invalid User Credentials"
              });
        }

        //Register User
        // need to change
        [HttpPost]
        [Route("Create")]
        public async Task<IActionResult> Register([FromBody] AuthenticationRequestDTO authenticationRequest)
        {
            if(ModelState.IsValid)
            {

                
                var user_exist = await _userManager.FindByEmailAsync(authenticationRequest.UserName);


                //Check Email is already taken
                if (user_exist != null)
                {

                    //check added email is contain in deleted account
                    if (user_exist.Status != 1)
                    {
                        return BadRequest(
                             new AuthenticationResponseDTO()
                             {
                                 Message = "You cant user this email"
                             });
                    }

                    return BadRequest(
                        new AuthenticationResponseDTO()
                        {
                            Message = "Email is Already Exist"
                        });
                }

                //Create User

                var new_user = new UserModel()
                {
                    Email = authenticationRequest.UserName,
                    UserName =authenticationRequest.UserName,
                    Status=1
                };  

                var is_created =await _userManager.CreateAsync(new_user,authenticationRequest.Password);
                var get_created_user = await _userManager.FindByEmailAsync(authenticationRequest.UserName);


                // Add Default Role as Reguler
  
               // await _roleManager.CreateAsync(new IdentityRole("Reguler"));
                await _userManager.AddToRoleAsync(get_created_user, "Reguler");



                if (is_created.Succeeded)
                {
                    TokenRequestDTO tokenRequest = new TokenRequestDTO();
                    tokenRequest.UserName = authenticationRequest.UserName;
                    tokenRequest.Role = "Reguler";
                    tokenRequest.UserId= get_created_user.Id;

                    
                    //Generate token

                    AuthenticationResponseDTO result = await _jwtTokenHandler.GenerateJwtToken(tokenRequest);

                    return Ok(
                        new AuthenticationResponseDTO
                        {
                            JwtToken=result!.JwtToken,
                            ExpiresIn=result.ExpiresIn,
                            UserName=result.UserName,
                            Message="User Create Successfully",
                            IsLocked = await _userManager.IsLockedOutAsync(new_user),
                            EmailConfirmed = await _userManager.IsEmailConfirmedAsync(new_user), 

                        });

                }
                return BadRequest(
                    new AuthenticationResponseDTO() {
                        Message="Server Error"
                    });
            }

            return BadRequest(
                new AuthenticationResponseDTO()
                {
                    Message = "Invalid User Credentials"
                });

        }

        /// <summary>
        /// SHOULD UPDATE
        /// </summary>
        /// <param name="lockOutDetailsInfo"></param>
        /// <returns></returns>
        [HttpPost("Security")]
        public async Task<IActionResult> ChangeSecurity([FromBody] LockOutDetailsInfoDTO lockOutDetailsInfo)
        {
            if(ModelState.IsValid) { 
                var exist_user = await _userManager.FindByEmailAsync(lockOutDetailsInfo.Email);

                if( exist_user != null)
                {
                   
                    var result = await _userManager.SetLockoutEnabledAsync(exist_user,lockOutDetailsInfo.LockoutEnable);
                    if(result.Succeeded) {
                        return Ok();
                    }
                    

                    

                }


            
            }

            return BadRequest();
        }

        [HttpPost]
        [Route("Request-RefreshToken")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenInfoDTO tokenInfoDTO)
        {
            if (ModelState.IsValid) {

                //check is token is valid
                var result = await _jwtTokenHandler.VerifyToken(tokenInfoDTO);

                if(result != null)
                {
                    return Ok(
                        result);
                }
                return BadRequest(
                    new AuthenticationResponseDTO
                    {
                        Message="Token Request is failed"
                    });
            }

            return BadRequest();
        }

        
        [HttpGet]
        [Route("Get-User-Details")]
        //[Authorize]
        public async Task<IActionResult> GetUserDetails()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            if (currentUser != null)
            {
                var mappedUser =_mapper.Map<UserModelResponseDTO>(currentUser);
                return Ok(mappedUser);
            }
            return Unauthorized("Fetch user details is faild");
        }
    
        [HttpPost]
        [Route("Update")]
        //[Authorize] should change
        public async Task<IActionResult> UpdateUser(UserModel user) {
            if (ModelState.IsValid)
            {
                var result =await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return Ok("Sucessfully Updated");
                }
                return BadRequest();
            }
            return BadRequest();

        
        }

        [HttpPost]
        [Route("ChangePassword")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePassword)
        {
            // Retrieve the current user
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);

            // Ensure the current user exists and model state is valid
            if (currentUser == null || !ModelState.IsValid)
            {
                return BadRequest("Invalid request");
            }

            // Validate input parameters
            if (string.IsNullOrWhiteSpace(changePassword.NewPassword) || string.IsNullOrWhiteSpace(changePassword.OldPassword))
            {
                return BadRequest("New password or old password is missing");
            }

            // Check if the old password matches
            var isOldPasswordCorrect = await IsPasswordCorrectAsync(changePassword.OldPassword, currentUser);
            if (!isOldPasswordCorrect)
            {
                return BadRequest("The old password is incorrect");
            }

            // Change password
            var result = await _userManager.ChangePasswordAsync(currentUser, changePassword.OldPassword, changePassword.NewPassword);
            if (result.Succeeded)
            {
                return Ok("Password changed successfully");
            }

            // Handle password change failure
            var errorMessage = string.Join(", ", result.Errors.Select(error => error.Description));
            return BadRequest(errorMessage);
        }


            // Check if the password is correct
        private async Task<bool> IsPasswordCorrectAsync(string password, UserModel user)
        {
            if (user != null)
            {
                return await _userManager.CheckPasswordAsync(user, password);
            }
            return false;
        }

    }

}
