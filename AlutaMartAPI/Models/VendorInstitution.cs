namespace AlutaMartAPI.Models;
    public class VendorInstitution : BaseEntity
    {
    public string Name { get; set; }
    public Guid? ParentInstitutionId { get; set; }
}