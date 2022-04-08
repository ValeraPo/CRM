using CRM.APILayer.Models;
using CRM.BusinessLayer;
using CRM.BusinessLayer.Exceptions;
using Marvelous.Contracts.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.APILayer.Extensions
{
    public static class ControllerExtensions
    {

        public static LeadIdentity GetLeadFromToken(this Controller controller)
        {
            var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
            var leadIdentity = new LeadIdentity();
            leadIdentity.Id = int.Parse(identity.Claims.ToList()
                .Where(c => c.Type == ClaimTypes.UserData)
                .Select(c => c.Value)
                .SingleOrDefault());
            leadIdentity.Role = Enum.Parse<Role>(identity.Claims.ToList()
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .SingleOrDefault());
            return leadIdentity;
        }

        
        public static void CheckToken(this Controller controller, IRequestHelper requestHelper)
        {
            var token = controller.HttpContext.Request.Headers.Authorization[0];
            if (!requestHelper.CheckToken(token).Result || !requestHelper.CheckTokenMicroservice(token).Result)
                throw new ForbiddenException("Invalid token.");
        }
    }
}
