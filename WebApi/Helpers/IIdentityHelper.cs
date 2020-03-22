using System;

namespace WebApi.Helpers
{
    public interface IIdentityHelper
    {
        Guid TenantId { get; }
        string Username { get; }
    }
}
