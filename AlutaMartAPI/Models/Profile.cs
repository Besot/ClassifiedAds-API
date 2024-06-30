using AlutaMartAPI.Utilities;
using Microsoft.AspNetCore.Identity;

namespace AlutaMartAPI.Models;
    public class Profile : IdentityUser<Guid>
    {
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public string Faculty { get; set; }
	public int Token { get; set; }
	public DateTimeOffset? TokenResetTime { get; set; }
	public TokenType? TokenType { get; set; }
	public Roles Role { get; set; }
	public DateTimeOffset Created { get; set; }
	public DateTimeOffset? Modified { get; set; }
	public bool IsActive { get; set; }
    }
