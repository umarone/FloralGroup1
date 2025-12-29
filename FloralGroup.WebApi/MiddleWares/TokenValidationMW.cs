using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;

namespace FloralGroup.WebApi.MiddleWares
{
    public class TokenValidationMW
    {
        private readonly RequestDelegate _requestDelegate;
        private readonly string _secretKey;
        public TokenValidationMW(RequestDelegate requestDelegate, string secretKey)
        {
            _requestDelegate = requestDelegate;
            _secretKey = secretKey;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            var jwtToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (jwtToken != null)
            {
                try
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var secKey = Encoding.UTF8.GetBytes(_secretKey);
                    tokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secKey),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out _);
                }
                catch (SecurityTokenExpiredException)
                {
                    await WriteReponse(context, "Token has expired");
                }
                catch (SecurityTokenException)
                {
                    await WriteReponse(context, "Token is not valid");
                }
                catch (Exception)
                {
                    await WriteReponse(context, "Authentication failed");
                }
            }

            await _requestDelegate(context);
        }
        private static async Task WriteReponse(HttpContext context, string message)
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            var response = new
            {
                success = false,
                message = message
            };

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}
