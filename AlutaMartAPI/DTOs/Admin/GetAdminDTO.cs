using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.ModelObjects;
    public class GetAdminDTO
    {
	public Guid? ProfileId { get; set; }
	public string FullName { get; set; }
	public string Email { get; set; }
    public string ProfilePictureUrl { get; set; }
    public Roles Role { get; set; }
    public string Designation { get; set; }
    public bool IsActive { get; set; }
}