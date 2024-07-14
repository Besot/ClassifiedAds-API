namespace AlutaMartAPI.DTOs;
    public class GetInstitutionDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Abbrev { get; set; }
        public string State { get; set; }
    }