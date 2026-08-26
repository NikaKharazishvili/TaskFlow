using Microsoft.AspNetCore.Identity;

namespace TaskFlow.Models;

/// <summary>Represents an application user. Extends ASP.NET Identity's IdentityUser (Id, Email, UserName, PasswordHash, etc.).</summary>
public class User : IdentityUser { }