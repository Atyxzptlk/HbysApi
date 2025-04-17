namespace HbysApi.Domain.Entities;

public class Contact : BaseEntity
{
    public required string BirimAdi { get; set; }
    public required string PhoneNumber { get; set; }
}
