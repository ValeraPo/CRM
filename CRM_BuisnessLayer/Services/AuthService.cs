using BearGoodbyeKolkhozProject.Business.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CRM.BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILeadRepository _leadRepo;
        private readonly ILogger<AuthService> _logger;
        private readonly IRequestHelper _requestHelper;

       public AuthService(ILeadRepository leadRepo, ILogger<AuthService> logger, IRequestHelper requestHelper)
        {
            _leadRepo = leadRepo;
            _logger = logger;
            _requestHelper = requestHelper;
        }

        public async Task<string> GetToken(AuthRequestModel auth)
        {
            _logger.LogInformation($"Authorization attempt with email {auth.Email.Encryptor()}.");
            Lead entity = await _leadRepo.GetByEmail(auth.Email);

            
            //var token = await _requestHelper.GetToken(auth);


            //ExceptionsHelper.ThrowIfPasswordIsIncorrected(pass, entity.Password);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, entity.Email),
                new Claim(ClaimTypes.UserData, entity.Id.ToString()),
                new Claim(ClaimTypes.Role, entity.Role.ToString())
            };
            _logger.LogInformation($"Received a token for a lead with email .");
            var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.Issuer,
                            audience: AuthOptions.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            _logger.LogInformation($"Authorization of lead with email  was successful.");

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
