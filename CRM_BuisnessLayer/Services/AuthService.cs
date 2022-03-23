using BearGoodbyeKolkhozProject.Business.Configuration;
using CRM.DataLayer.Entities;
using CRM.DataLayer.Extensions;
using CRM.DataLayer.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NLog;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace CRM.BusinessLayer.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILeadRepository _leadRepo;
        private readonly ILogger<AuthService> _logger;


        public AuthService(ILeadRepository leadRepo, ILogger<AuthService> logger)
        {
            _leadRepo = leadRepo;
            _logger = logger;
        }

        public async Task<string> GetToken(string email, string pass)
        {
            _logger.LogInformation($"Popytka avtorizacii pol'zovatelya c email = {email.Encryptor()}.");
            Lead entity = await _leadRepo.GetByEmail(email);

            ExceptionsHelper.ThrowIfEmailNotFound(email, entity);
            ExceptionsHelper.ThrowIfLeadWasBanned(entity.Id, entity);
            ExceptionsHelper.ThrowIfPasswordIsIncorrected(pass, entity.Password);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, entity.Email),
                new Claim(ClaimTypes.UserData, entity.Id.ToString()),
                new Claim(ClaimTypes.Role, entity.Role.ToString())
            };
            _logger.LogInformation($"Poluxhenie tokena  pol'zovatelya c email = {email.Encryptor()}.");
            var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.Issuer,
                            audience: AuthOptions.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
            _logger.LogInformation($"Avtorizaciya pol'zovatelya c email = {email.Encryptor()} proshla uspeshno.");

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
