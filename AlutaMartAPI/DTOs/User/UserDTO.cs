using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.DTOs;

public class  UserDTO
{
	public Guid Id { get; set; }
	public long Code { get; set; }
	public string Email { get; set; }
	public string Phone { get; set; }
	public bool IsActive { get; set; }
	public string ProfilePicUrl { get; set; }
	public string FullName => $"{FirstName} {LastName}";
	public string FirstName { get; set; }
	public string LastName { get; set; }
	public Roles Access { get; set; }
	public string AccessName => Access.Name();
	public Guid? VendorId { get; set; }
}