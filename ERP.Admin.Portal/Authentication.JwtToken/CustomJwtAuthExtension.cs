using Authentication.jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ERP.Authentication.Jwt
{
    public static class CustomJwtAuthExtension
    {
        public static void AddCustomJwtAuthenticaion(this IServiceCollection services)
        {

            string key = "yyAhYj6LYNzoL8bRVKbuF2EfKMKN05WComWtIVa5AUSScmiNWBFam8jFcwvZ54lR";

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)),
                ValidateLifetime = true,
                RequireExpirationTime = false,

            };

            //Inject into ouy DI container  
            services.AddSingleton(tokenValidationParameters);


            services.AddAuthentication(o =>
            {
                o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer( o =>
            {
                o.RequireHttpsMetadata = false;
                o.SaveToken = true;
                o.TokenValidationParameters = tokenValidationParameters;
            });
        } 
    }
}
