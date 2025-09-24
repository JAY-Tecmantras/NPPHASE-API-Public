using Microsoft.IdentityModel.Tokens;
using NPPHASE.Data.Model;
using NPPHASE.Data.ViewModel;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NPPHASE.Services.Repositories
{
    public class TokenGenerator
    {
        public TokenGenerator() { }
        public string GenerateToken(TokenRequestViewModel? model)
        {
            try
            { 

                var userName = model.UserName != null ? model.UserName : "";
                var deviceId = model.DeviceId != null ? model.DeviceId : "";
                var claims = new List<Claim>{
                    new Claim(JwtRegisteredClaimNames.Email, model.Email),
                    new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                    new Claim("Username",userName),
                    new Claim("Email", model.Email),
                    new Claim(ClaimTypes.NameIdentifier,model.UserId),
                    new Claim("UserId",model.UserId),
                    new Claim(ClaimTypes.Role, model.Role),
                    new Claim("DeviceId", deviceId)
                };
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(model.JwtKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
                var expires = DateTime.UtcNow.AddDays(2);
                var token = new JwtSecurityToken(
                           claims: claims,
                           expires: expires,
                           signingCredentials: creds
                       );
                //return new AuthenticationResponse
                //{
                //    Token = new JwtSecurityTokenHandler().WriteToken(token),
                //    ExpiresIn = (int)expires.Subtract(DateTime.Now).TotalSeconds,
                //};
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
