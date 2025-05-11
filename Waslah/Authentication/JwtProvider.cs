using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json;

namespace Waslah.Authentication
{
    public class JwtProvider(IOptions<JwtOptions> JwtOptions) : IJwtProvider
    {
        private readonly JwtOptions _jwtOptions = JwtOptions.Value;

        public (string Token, int ExpiresIn) GenerateToken(ApplicationUser user , IEnumerable<string> Roles,
            IEnumerable<string> Permissions )
        {
            Claim[] claims = [
                new(JwtRegisteredClaimNames.Sub,user.Id),
            new(JwtRegisteredClaimNames.GivenName,user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName,user.LastName),
            new(JwtRegisteredClaimNames.Email,user.Email!),
            new(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
            new Claim(nameof(Roles), JsonSerializer.Serialize(Roles), JsonClaimValueTypes.JsonArray),
            new Claim(nameof(Permissions), JsonSerializer.Serialize(Permissions), JsonClaimValueTypes.JsonArray)
                ];

            var SymmetricSequrityKey = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            var SigningCredintials = new SigningCredentials(SymmetricSequrityKey, SecurityAlgorithms.HmacSha256);

            var ExpiryTime = _jwtOptions.ExpiresIn;

            var Token = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(ExpiryTime),
                signingCredentials: SigningCredintials
                );

            return (Token: new JwtSecurityTokenHandler().WriteToken(Token), ExpiresIn: ExpiryTime * 60);

        }

        public Result<string> ValidateToken(string Token)
        {
            var TokenHandler = new JwtSecurityTokenHandler();
            var SynmmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Key));

            try
            {
                TokenHandler.ValidateToken(Token, new TokenValidationParameters
                {
                    IssuerSigningKey = SynmmetricKey,
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                var userId = jwtToken.Claims.First(x => x.Type == JwtRegisteredClaimNames.Sub).Value;

                return Result.Success(userId);

            }
            catch
            {
                return Result.Failure<string>(UserErrors.InvalidJwtToken);
            }
        }
    }
}
