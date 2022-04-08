using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Marvelous.Contracts.RequestModels;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;

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

            ExceptionsHelper.ThrowIfEmailNotFound(auth.Email, entity);
            ExceptionsHelper.ThrowIfLeadWasBanned(entity.Id, entity);
            var token = await _requestHelper.GetToken(auth);


            //ExceptionsHelper.ThrowIfPasswordIsIncorrected(pass, entity.Password);

            //List<Claim> claims = new List<Claim> {
            //    new Claim(ClaimTypes.Email, entity.Email),
            //    new Claim(ClaimTypes.UserData, entity.Id.ToString()),
            //    new Claim(ClaimTypes.Role, entity.Role.ToString())
            //};
            //_logger.LogInformation($"Received a token for a lead with email {email.Encryptor()}.");
            //var jwt = new JwtSecurityToken(
            //                issuer: AuthOptions.Issuer,
            //                audience: AuthOptions.Audience,
            //                claims: claims,
            //                expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
            //                signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            //_logger.LogInformation($"Authorization of lead with email {email.Encryptor()} was successful.");

            return token.Content;

        }
    }
}
