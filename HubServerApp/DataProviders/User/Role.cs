using Microsoft.AspNetCore.Identity;

namespace HubServerApp.DataProviders;

public class Role : IdentityRole
{
    public virtual ICollection<UserRole> UserRoles { get; } = new HashSet<UserRole>();
}
