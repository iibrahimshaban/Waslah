using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Waslah.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> JwtOptions) : IJwtProvider
    {
        private readonly JwtOptions _options = JwtOptions.Value;

        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user)
        {
            Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub,user.Id),
            new(JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.GivenName,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
        ];

            var SymmetricSequrityKey = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            var SigningCredintials = new SigningCredentials(SymmetricSequrityKey, SecurityAlgorithms.HmacSha256);


            var ExpirationDate = DateTime.UtcNow.AddMinutes(_options.ExpiryMinutes);

            var Token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                expires: ExpirationDate,
                signingCredentials: SigningCredintials
                );

            return (Token: new JwtSecurityTokenHandler().WriteToken(Token), ExpiresIn: _options.ExpiryMinutes * 60);

        }
        public string? ValidateToken(string token)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var SymmetricSequrityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));

            try
            {
                TokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    IssuerSigningKey = SymmetricSequrityKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var JwtToken = (JwtSecurityToken)validatedToken;

                return JwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;
            }
            catch
            {
                return null;
            }
        }
    }
}
