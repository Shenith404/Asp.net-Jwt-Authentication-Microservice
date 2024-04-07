


using Authentication.Core.DTOs;
using Authentication.Core.Entity;
using Authentication.DataService.IConfiguration;
using ERP.Authentication.Core.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;


namespace Authentication.jwt
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        string key = "yyAhYj6LYNzoL8bRVKbuF2EfKMKN05WComWtIVa5AUSScmiNWBFam8jFcwvZ54lR";


        private const int JWT_VALIDITY_MINS = 1;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserManager<UserModel> _userManager;

        public JwtTokenHandler(IUnitOfWorks unitOfWorks,
            TokenValidationParameters tokenValidationParameters,
            UserManager<UserModel> userManager)
        {
            _userManager = userManager;
            _unitOfWorks = unitOfWorks;
            _tokenValidationParameters = tokenValidationParameters;
        }

        public  async Task<AuthenticationResponseDTO ?>  GenerateJwtToken(TokenRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.UserName) )
            {
                return null;
            }

            /*var userAccount = _userAccounts.Where(x => x.UserName.Equals(request.UserName) && x.Password.Equals(request.Password))
                .FirstOrDefault();

            if (userAccount == null)
                return null;*/

            var tokenExpiryTimeStamp = DateTime.Now.AddMinutes(JWT_VALIDITY_MINS);
            var tokenKey = Encoding.ASCII.GetBytes(key);
      

            var claimsIdentity = new ClaimsIdentity(
                new List<Claim>
                {
                  new Claim("Id", request.UserId),
                  new Claim(JwtRegisteredClaimNames.Name, request.UserName),
                  new Claim(ClaimTypes.NameIdentifier, request.UserId),
                  new Claim(ClaimTypes.Role, request.Role),
                  new Claim(JwtRegisteredClaimNames.Sub ,request.UserName),
                  new Claim(JwtRegisteredClaimNames.Jti ,Guid.NewGuid().ToString()),
                  new Claim(JwtRegisteredClaimNames.Iat,DateTime.Now.ToUniversalTime().ToString()),
                });


            var signinCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey),
                    SecurityAlgorithms.HmacSha256Signature
                );

            var securityTokenDescripter = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = tokenExpiryTimeStamp,
                SigningCredentials = signinCredentials
            };

            //create jwt token
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var securityToken = jwtSecurityTokenHandler.CreateToken(securityTokenDescripter);
            var token = jwtSecurityTokenHandler.WriteToken(securityToken);


            //create refresh token
            var refreshtoken = new RefreshToken
            {
                Token = $"{RandomStringGenerator(25)}_{Guid.NewGuid()}" ,
                UserId = request.UserId,
                IsRevoked = false,
                IsUsed = false,
                Status=1,
                JwtId= securityToken.Id,
                ExpiredDate= DateTime.UtcNow.AddMonths(1),
            };
            await _unitOfWorks.RefreshToknes.Add(refreshtoken);
            await _unitOfWorks.CompleteAsync();


            return new AuthenticationResponseDTO
            {
                UserName = request.UserName,
                ExpiresIn = (int)tokenExpiryTimeStamp.Subtract(DateTime.Now).TotalSeconds,
                JwtToken = token,
                RefreshToken =refreshtoken.Token,
            };

        }

        public async Task<AuthenticationResponseDTO?>  VerifyToken(TokenInfoDTO tokenInfoDTO)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            try
            {
                //check validity of the token
                var principle = tokenhandler.ValidateToken(tokenInfoDTO.JwtToken, _tokenValidationParameters, out var ValidateToken);
                if(ValidateToken is JwtSecurityToken jwtSecurityToken)
                {
                    //check the algorithm
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,StringComparison.InvariantCultureIgnoreCase);
                    if(result==false) { return null; }
                }

                //check the expire data
                var utcExpireDate = long.Parse(principle.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                //conver date to check
                var expDate = UnixTimeStampToDateTime(utcExpireDate);

                if (expDate > DateTime.UtcNow)
                {

                    //JwtToken is not expired
                    Console.WriteLine("JwtToken is not expired");
                    return null;
                }

                //check the refresh token is exist 

                var refreshTokenExist = await _unitOfWorks.RefreshToknes.GetByRefreshToken(tokenInfoDTO.RefreshToken);
                if(refreshTokenExist == null) {
                 //invalid refreshToken
                    return null; 
                };

                //check expire date of refreshToken
                if (refreshTokenExist.ExpiredDate < DateTime.UtcNow)
                {
                    //refreshToken is expired
                    return null;


                }

                //check is refresh token is use or not
                if (refreshTokenExist.IsUsed)
                {
                    //Token is used
                    return null;
                }

                //check refreshToken if it has been revoked
                if (refreshTokenExist.IsRevoked)
                {
                  //refreshToken is revoked
                    return null;
                }
                
                var jti =principle.Claims.SingleOrDefault(x=>x.Type == JwtRegisteredClaimNames.Jti).Value;


                if (refreshTokenExist.JwtId !=jti)
                {
                    //refreshToken is reference does not match jwt Token
                    return null;
                }

                //start processing and get new token
                refreshTokenExist.IsUsed = true;
                var updateResult =await _unitOfWorks.RefreshToknes.MarkRefreshTokenAsUser(refreshTokenExist);

                if (updateResult)
                {
                    await _unitOfWorks.CompleteAsync();

                    //get the user to generate new token 
                    var dbUser = await _userManager.FindByIdAsync(refreshTokenExist.UserId);


                    if(dbUser == null) { return null; }
                    TokenRequestDTO tokenRequest = new TokenRequestDTO();
                    tokenRequest.UserName = dbUser.UserName!;
                    tokenRequest.Role = (await _userManager.GetRolesAsync(dbUser))[0];
                    tokenRequest.UserId = dbUser.Id;
                    //generate Token
                    var result =  await GenerateJwtToken(tokenRequest);
                    if(result != null)
                    {
                        result.EmailConfirmed=dbUser.EmailConfirmed;
                        result.IsLocked = await _userManager.IsLockedOutAsync(dbUser);
                        return result;
                    }

                    return null;
                }


                return null;


            }
            catch (Exception ex)
            {
                return null;

            }

            return null;
        }

        private DateTime UnixTimeStampToDateTime(long unixDate)
        {
            //set the time to 1 jan 1970
            var dateTime = new DateTime(1970 ,1,1 ,0,0,0,0,DateTimeKind.Utc);
            //Add the number of second from 1 jan 1970
            dateTime = dateTime.AddSeconds(unixDate).ToUniversalTime();
            return dateTime;
        }

        private string  RandomStringGenerator(int length)
        {
            var random =new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());
        }

      
    }
}
