﻿using CRM.BusinessLayer.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CRM.APILayer.Attribites
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeEnumAttribute : AuthorizeAttribute
    {
        public AuthorizeEnumAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType().BaseType != typeof(Enum)))
                throw new TypeMismatchException("The passed argument is not an enum.");

            Roles = string.Join(",", roles.Select(r => Enum.GetName(r.GetType(), r)));

        }
    }
}
