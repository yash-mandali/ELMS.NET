using collageProject.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace collageProject
{
    public class GetJwtToken
    {
        private readonly IConfiguration _config;

        public GetJwtToken(IConfiguration config)
        {
            _config = config;
        }
        public string GenerateJwtToken(User users)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, users.Id.ToString()),
                new Claim(ClaimTypes.Email,users.Email),
                new Claim(ClaimTypes.Role,users.Role)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _config["jwt:Issuer"],
                audience: _config["jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_config["jwt:ExpireMinutes"])),
                signingCredentials: cred
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

