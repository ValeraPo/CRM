using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CRM.APILayer.Extensions
{
    public static class ControllerExtensions
    {

        private static List<Claim> GetInfoFromToken(this Controller controller)
        {
            var identity = controller.HttpContext.User.Identity as ClaimsIdentity;
            return identity.Claims.ToList();
        }

        public static int GetLeadRole(this Controller controller) =>
            int.Parse(GetInfoFromToken(controller)
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .SingleOrDefault());

        public static int GetLeadId(this Controller controller) =>
            int.Parse(GetInfoFromToken(controller)
                .Where(c => c.Type == ClaimTypes.UserData)
                .Select(c => c.Value)
                .SingleOrDefault());

    }
}
