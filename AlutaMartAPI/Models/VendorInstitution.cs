namespace AlutaMartAPI.Models;
    public class VendorInstitution : BaseEntity
    {
    public string Name { get; set; }
    public string Abbrev { get; set; }
    public string State { get; set; }
    public Guid? ParentInstitutionId { get; set; }
}