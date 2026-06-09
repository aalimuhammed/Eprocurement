namespace EPROCUREMENT.DTO
{
    public record UserAssignedDTO
    {
        public int UserId { get; init; }
        public string TaxId { get; init; }
        public string CompanyName { get; init; }
    }
}
