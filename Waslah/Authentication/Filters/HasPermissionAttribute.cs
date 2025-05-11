using Microsoft.AspNetCore.Authorization;

namespace Waslah.Authentication.Filters;

public class HasPermissionAttribute(string Permission) : AuthorizeAttribute(Permission)
{
}
