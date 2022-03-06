using CRM.DataLayer.Entities;
using CRM.DataLayer.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace CRM.BusinessLayer.Services
{
    public class AuthService
    {
        private readonly ILeadRepository _leadRepo;


        public AuthService(ILeadRepository leadRepo)
        {
            _leadRepo = leadRepo;
        }

        public string GetToken(string email, string pass)
        {
            Lead entity = _leadRepo.GetByEmail(email);

            ExceptionsHelper.ThrowIfEntityNotFound(entity.Id, entity);
            ExceptionsHelper.ThrowIfLeadWasBanned(entity.Id, entity);

            ExceptionsHelper.ThrowIfPasswordIsIncorrected(pass, entity.Password);

            List<Claim> claims = new List<Claim> {
                new Claim(ClaimTypes.Email, entity.Email),
                new Claim(ClaimTypes.UserData, entity.Id.ToString()),
                new Claim(ClaimTypes.Role, entity.Role.ToString())
            };


            var jwt = new JwtSecurityToken(
                            issuer: AuthOptions.Issuer,
                            audience: AuthOptions.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.Add(TimeSpan.FromMinutes(30)),
                            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));

            return new JwtSecurityTokenHandler().WriteToken(jwt);

        }
    }
}
