using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace WebApi.Helpers
{
    public class IdentityHelper : IIdentityHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid TenantId => GetClaimGuid("TenantId");
        public string Username => GetClaimString("Username");


        public Guid GetClaimGuid(string claimName)
        {
            if (Guid.TryParse(GetClaims()?.Where(c => c.Type == claimName).FirstOrDefault()?.Value, out var result))
                return result;
            else
                return default;
        }

        public string GetClaimString(string claimName)
        {
            return GetClaims()?.Where(c => c.Type == claimName).FirstOrDefault()?.Value;
        }

        private IEnumerable<Claim> GetClaims()
        {
            return _httpContextAccessor.HttpContext.User.Claims;
        }
    }
}
